-- ==============================================================================
-- MÓDULO CARRITO - REFACTORIZADO ORACLE 21c
-- Estilo: ACID, Profesional, Optimizado
-- ==============================================================================

-- ==============================================================================
-- SP_CARRITO_AGREGAR_PRODUCTO
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CARRITO_AGREGAR_PRODUCTO (
    p_cli_cliente       IN  NUMBER,
    p_pro_producto      IN  NUMBER,
    p_cantidad          IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2,
    p_carrito_id        OUT NUMBER
)
IS
    v_carrito_id        NUMBER;
    v_detalle_id        NUMBER;
    v_precio            NUMBER;
    v_stock             NUMBER;
    v_usuario_id        NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    BEGIN
        SELECT USU_USUARIO INTO v_usuario_id
        FROM ALP_USUARIO WHERE USU_USERNAME = USER;
    EXCEPTION WHEN NO_DATA_FOUND THEN v_usuario_id := NULL;
    END;

    IF p_cantidad <= 0 THEN
        p_mensaje := 'Cantidad debe ser mayor a cero';
        RAISE ex_negocio;
    END IF;

    -- FIX: FOR UPDATE no permitido con GROUP BY/SUM; se bloquea directamente sobre ALP_EXISTENCIA
    SELECT NVL(SUM(EXI_CANTIDAD_DISPONIBLE), 0) INTO v_stock
    FROM ALP_EXISTENCIA
    WHERE PRO_PRODUCTO = p_pro_producto;

    -- Bloqueo pesimista sobre las filas de existencia para atomicidad
    UPDATE ALP_EXISTENCIA SET EXI_ULTIMA_ACTUALIZACION = EXI_ULTIMA_ACTUALIZACION
    WHERE PRO_PRODUCTO = p_pro_producto;

    IF v_stock < p_cantidad THEN
        p_mensaje := 'Stock insuficiente. Disponible: ' || v_stock;
        RAISE ex_negocio;
    END IF;

    SELECT PPR_PRECIO INTO v_precio
    FROM ALP_PRODUCTO_PRECIO
    WHERE PRO_PRODUCTO = p_pro_producto AND PPR_ESTADO = 'ACTIVO'
    FETCH FIRST 1 ROWS ONLY;

    -- FIX: ALP_CARRITO no tiene CAR_ESTADO; se usa constraint UNIQUE (CLI_CLIENTE)
    BEGIN
        SELECT CAR_CARRITO INTO v_carrito_id
        FROM ALP_CARRITO
        WHERE CLI_CLIENTE = p_cli_cliente
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        INSERT INTO ALP_CARRITO (CLI_CLIENTE)
        VALUES (p_cli_cliente)
        RETURNING CAR_CARRITO INTO v_carrito_id;
    END;

    -- FIX: CAD_SUBTOTAL es NOT NULL, debe calcularse en insert/update
    BEGIN
        SELECT CAD_CARRITO_DETALLE INTO v_detalle_id
        FROM ALP_CARRITO_DETALLE
        WHERE CAR_CARRITO = v_carrito_id AND PRO_PRODUCTO = p_pro_producto;

        UPDATE ALP_CARRITO_DETALLE
        SET CAD_CANTIDAD        = CAD_CANTIDAD + p_cantidad,
            CAD_PRECIO_UNITARIO = v_precio,
            CAD_SUBTOTAL        = (CAD_CANTIDAD + p_cantidad) * v_precio,
            CAD_FECHA_AGREGADO  = CURRENT_TIMESTAMP
        WHERE CAD_CARRITO_DETALLE = v_detalle_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        INSERT INTO ALP_CARRITO_DETALLE (CAR_CARRITO, PRO_PRODUCTO, CAD_CANTIDAD, CAD_PRECIO_UNITARIO, CAD_SUBTOTAL)
        VALUES (v_carrito_id, p_pro_producto, p_cantidad, v_precio, p_cantidad * v_precio)
        RETURNING CAD_CARRITO_DETALLE INTO v_detalle_id;
    END;

    -- Actualizar subtotal del carrito
    UPDATE ALP_CARRITO
    SET CAR_SUBTOTAL             = (SELECT NVL(SUM(CAD_SUBTOTAL), 0) FROM ALP_CARRITO_DETALLE WHERE CAR_CARRITO = v_carrito_id),
        CAR_FECHA_ACTUALIZACION  = CURRENT_TIMESTAMP
    WHERE CAR_CARRITO = v_carrito_id;

    -- FIX: columna es TRL_ENTIDAD (no TRL_TABLA)
    INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
    VALUES (v_usuario_id, 'ALP_CARRITO_DETALLE', 'INSERT', v_detalle_id,
            JSON_OBJECT('producto' VALUE p_pro_producto, 'cantidad' VALUE p_cantidad, 'precio' VALUE v_precio));

    COMMIT;
    p_resultado  := 'EXITO';
    p_mensaje    := 'Producto agregado al carrito';
    p_carrito_id := v_carrito_id;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado  := 'ERROR';
        p_carrito_id := NULL;
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado  := 'ERROR';
        p_mensaje    := SQLERRM;
        p_carrito_id := NULL;
END;
/

-- ==============================================================================
-- SP_CARRITO_ACTUALIZAR_CANTIDAD
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CARRITO_ACTUALIZAR_CANTIDAD (
    p_detalle_id        IN  NUMBER,
    p_nueva_cantidad    IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_producto_id       NUMBER;
    v_stock             NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    IF p_nueva_cantidad <= 0 THEN
        p_mensaje := 'Cantidad debe ser mayor a cero';
        RAISE ex_negocio;
    END IF;

    SELECT PRO_PRODUCTO INTO v_producto_id
    FROM ALP_CARRITO_DETALLE
    WHERE CAD_CARRITO_DETALLE = p_detalle_id
    FOR UPDATE;

    SELECT NVL(SUM(EXI_CANTIDAD_DISPONIBLE), 0) INTO v_stock
    FROM ALP_EXISTENCIA
    WHERE PRO_PRODUCTO = v_producto_id;
    
    IF v_stock < p_nueva_cantidad THEN
        p_mensaje := 'Stock insuficiente. Disponible: ' || v_stock;
        RAISE ex_negocio;
    END IF;

    UPDATE ALP_CARRITO_DETALLE
    SET CAD_CANTIDAD = p_nueva_cantidad
    WHERE CAD_CARRITO_DETALLE = p_detalle_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje := 'Cantidad actualizada';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje := SQLERRM;
END;
/

-- ==============================================================================
-- SP_CARRITO_ELIMINAR_PRODUCTO
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CARRITO_ELIMINAR_PRODUCTO (
    p_detalle_id        IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
BEGIN
    DELETE FROM ALP_CARRITO_DETALLE
    WHERE CAD_CARRITO_DETALLE = p_detalle_id;
    
    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20001, 'Producto no encontrado');
    END IF;
    
    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje := 'Producto eliminado';

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje := SQLERRM;
END;
/

-- ==============================================================================
-- SP_CARRITO_VACIAR
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CARRITO_VACIAR (
    p_carrito_id        IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_count             NUMBER;
BEGIN
    DELETE FROM ALP_CARRITO_DETALLE
    WHERE CAR_CARRITO = p_carrito_id;
    
    v_count := SQL%ROWCOUNT;
    COMMIT;
    
    p_resultado := 'EXITO';
    p_mensaje := v_count || ' productos eliminados';

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje := SQLERRM;
END;
/

-- ==============================================================================
-- SP_CARRITO_CALCULAR_TOTAL
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CARRITO_CALCULAR_TOTAL (
    p_carrito_id        IN  NUMBER,
    p_subtotal          OUT NUMBER,
    p_impuestos         OUT NUMBER,
    p_total             OUT NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    c_iva               CONSTANT NUMBER := 0.12;
BEGIN
    SELECT NVL(SUM(CAD_CANTIDAD * CAD_PRECIO_UNITARIO), 0)
    INTO p_subtotal
    FROM ALP_CARRITO_DETALLE
    WHERE CAR_CARRITO = p_carrito_id;
    
    p_impuestos := ROUND(p_subtotal * c_iva, 2);
    p_total := p_subtotal + p_impuestos;
    
    p_resultado := 'EXITO';
    p_mensaje := 'Total calculado';

EXCEPTION
    WHEN OTHERS THEN
        p_resultado := 'ERROR';
        p_mensaje := SQLERRM;
        p_subtotal := 0;
        p_impuestos := 0;
        p_total := 0;
END;
/

-- ==============================================================================
-- SP_CARRITO_CONVERTIR_ORDEN
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CARRITO_CONVERTIR_ORDEN (
    p_carrito_id        IN  NUMBER,
    p_canal_venta       IN  NUMBER,
    p_orden_id          OUT NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_cliente_id        NUMBER;
    v_subtotal          NUMBER;
    v_impuestos         NUMBER;
    v_total             NUMBER;
    v_numero            VARCHAR2(50);
    v_estado_id         NUMBER;
    v_count             NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    SELECT COUNT(*) INTO v_count
    FROM ALP_CARRITO_DETALLE
    WHERE CAR_CARRITO = p_carrito_id;

    IF v_count = 0 THEN
        p_mensaje := 'Carrito vacío';
        RAISE ex_negocio;
    END IF;

    -- FIX: ALP_CARRITO no tiene CAR_ESTADO
    SELECT CLI_CLIENTE INTO v_cliente_id
    FROM ALP_CARRITO
    WHERE CAR_CARRITO = p_carrito_id
    FOR UPDATE;

    SP_CARRITO_CALCULAR_TOTAL(p_carrito_id, v_subtotal, v_impuestos, v_total, p_resultado, p_mensaje);

    SELECT ESO_ESTADO_ORDEN INTO v_estado_id
    FROM ALP_ESTADO_ORDEN
    WHERE ESO_CODIGO = 'PENDIENTE'
    FETCH FIRST 1 ROWS ONLY;

    v_numero := 'ORD-' || TO_CHAR(SYSDATE, 'YYYYMMDD') || '-' || p_carrito_id;

    -- FIX: ALP_ORDEN_VENTA no tiene VEN_ESTADO (columna eliminada en DDL actualizado)
    INSERT INTO ALP_ORDEN_VENTA (CLI_CLIENTE, CVE_CANAL_VENTA, VEN_NUMERO_ORDEN,
                                  VEN_SUBTOTAL, VEN_IMPUESTOS, VEN_TOTAL, ESO_ESTADO_ORDEN)
    VALUES (v_cliente_id, p_canal_venta, v_numero, v_subtotal, v_impuestos, v_total, v_estado_id)
    RETURNING VEN_ORDEN_VENTA INTO p_orden_id;

    -- FIX: VDE_TOTAL es NOT NULL; se calcula igual a subtotal cuando no hay descuento/impuesto por línea
    INSERT INTO ALP_ORDEN_VENTA_DETALLE (VEN_ORDEN_VENTA, PRO_PRODUCTO, VDE_CANTIDAD,
                                          VDE_PRECIO_UNITARIO, VDE_SUBTOTAL, VDE_TOTAL, VDE_ESTADO)
    SELECT p_orden_id, PRO_PRODUCTO, CAD_CANTIDAD, CAD_PRECIO_UNITARIO,
           CAD_SUBTOTAL, CAD_SUBTOTAL, 'ACTIVO'
    FROM ALP_CARRITO_DETALLE
    WHERE CAR_CARRITO = p_carrito_id;

    -- FIX: no existe CAR_ESTADO; se vacía el carrito como equivalente de "convertido"
    DELETE FROM ALP_CARRITO_DETALLE WHERE CAR_CARRITO = p_carrito_id;

    UPDATE ALP_CARRITO
    SET CAR_SUBTOTAL            = 0,
        CAR_FECHA_ACTUALIZACION = CURRENT_TIMESTAMP
    WHERE CAR_CARRITO = p_carrito_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Orden creada: ' || v_numero;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_orden_id  := NULL;
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := SQLERRM;
        p_orden_id  := NULL;
END;
/
