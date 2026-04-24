-- ==============================================================================
-- MÓDULO CARRITO - ESTÁNDAR PROFESIONAL MUEBLERÍA ALPES
-- Optimizado para .NET 8, Dapper y Oracle 21c (ACID + Seguridad)
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
    -- 1. IDENTIFICACIÓN DE USUARIO (Auditoría)
    BEGIN
        SELECT USU_USUARIO INTO v_usuario_id
        FROM ALP_USUARIO WHERE USU_USERNAME = USER;
    EXCEPTION WHEN NO_DATA_FOUND THEN v_usuario_id := NULL;
    END;

    -- 2. VALIDACIÓN DE PARÁMETROS
    IF p_cantidad <= 0 THEN
        p_mensaje := 'Cantidad debe ser mayor a cero';
        RAISE ex_negocio;
    END IF;

    -- 3. VALIDACIÓN DE EXISTENCIA DE PRODUCTO Y PRECIO
    BEGIN
        SELECT PPR_PRECIO INTO v_precio
        FROM ALP_PRODUCTO_PRECIO
        WHERE PRO_PRODUCTO = p_pro_producto AND PPR_ESTADO = 'ACTIVO'
        FETCH FIRST 1 ROWS ONLY;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'El producto no existe o no tiene un precio activo configurado.';
        RAISE ex_negocio;
    END;

    -- 4. VALIDACIÓN DE STOCK (Bloqueo Pesimista)
    SELECT NVL(SUM(EXI_CANTIDAD_DISPONIBLE), 0) INTO v_stock
    FROM ALP_EXISTENCIA
    WHERE PRO_PRODUCTO = p_pro_producto;

    -- Bloqueo pesimista para evitar condiciones de carrera en stock
    UPDATE ALP_EXISTENCIA SET EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
    WHERE PRO_PRODUCTO = p_pro_producto;

    IF v_stock < p_cantidad THEN
        p_mensaje := 'Stock insuficiente. Disponible: ' || v_stock || ', Solicitado: ' || p_cantidad;
        RAISE ex_negocio;
    END IF;

    -- 5. GESTIÓN DEL CABECERA DEL CARRITO (Existencia/Creación)
    BEGIN
        SELECT CAR_CARRITO INTO v_carrito_id
        FROM ALP_CARRITO
        WHERE CLI_CLIENTE = p_cli_cliente
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        INSERT INTO ALP_CARRITO (CLI_CLIENTE, CAR_SUBTOTAL)
        VALUES (p_cli_cliente, 0)
        RETURNING CAR_CARRITO INTO v_carrito_id;
    END;

    -- 6. GESTIÓN DEL DETALLE (Update si existe, Insert si es nuevo)
    BEGIN
        SELECT CAD_CARRITO_DETALLE INTO v_detalle_id
        FROM ALP_CARRITO_DETALLE
        WHERE CAR_CARRITO = v_carrito_id AND PRO_PRODUCTO = p_pro_producto
        FOR UPDATE;

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

    -- 7. ACTUALIZACIÓN AUTOMÁTICA DE TOTALES
    UPDATE ALP_CARRITO
    SET CAR_SUBTOTAL             = (SELECT NVL(SUM(CAD_SUBTOTAL), 0) FROM ALP_CARRITO_DETALLE WHERE CAR_CARRITO = v_carrito_id),
        CAR_FECHA_ACTUALIZACION  = CURRENT_TIMESTAMP
    WHERE CAR_CARRITO = v_carrito_id;

    -- 8. AUDITORÍA
    INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
    VALUES (v_usuario_id, 'ALP_CARRITO_DETALLE', 'INSERT/UPDATE', v_detalle_id,
            JSON_OBJECT('producto' VALUE p_pro_producto, 'cantidad' VALUE p_cantidad, 'precio' VALUE v_precio));

    COMMIT;
    p_resultado  := 'EXITO';
    p_mensaje    := 'Producto agregado al carrito con éxito';
    p_carrito_id := v_carrito_id;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado  := 'ERROR';
        p_carrito_id := NULL;
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado  := 'ERROR';
        p_mensaje    := 'Error Técnico: ' || SQLERRM;
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
    v_carrito_id        NUMBER;
    v_stock             NUMBER;
    v_precio            NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. VALIDACIÓN DE EXISTENCIA DEL DETALLE
    BEGIN
        SELECT PRO_PRODUCTO, CAR_CARRITO INTO v_producto_id, v_carrito_id
        FROM ALP_CARRITO_DETALLE
        WHERE CAD_CARRITO_DETALLE = p_detalle_id
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'El ítem del carrito solicitado no existe.';
        RAISE ex_negocio;
    END;

    IF p_nueva_cantidad <= 0 THEN
        p_mensaje := 'La cantidad debe ser mayor a cero. Use el servicio de eliminación si desea quitar el producto.';
        RAISE ex_negocio;
    END IF;

    -- 2. VALIDACIÓN DE STOCK
    SELECT NVL(SUM(EXI_CANTIDAD_DISPONIBLE), 0) INTO v_stock
    FROM ALP_EXISTENCIA
    WHERE PRO_PRODUCTO = v_producto_id;
    
    IF v_stock < p_nueva_cantidad THEN
        p_mensaje := 'Stock insuficiente para la nueva cantidad. Disponible: ' || v_stock;
        RAISE ex_negocio;
    END IF;

    -- 3. OBTENER PRECIO ACTUALIZADO
    BEGIN
        SELECT PPR_PRECIO INTO v_precio
        FROM ALP_PRODUCTO_PRECIO
        WHERE PRO_PRODUCTO = v_producto_id AND PPR_ESTADO = 'ACTIVO'
        FETCH FIRST 1 ROWS ONLY;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'No se pudo obtener el precio del producto.';
        RAISE ex_negocio;
    END;

    -- 4. ACTUALIZACIÓN DEL DETALLE Y CABECERA
    UPDATE ALP_CARRITO_DETALLE
    SET CAD_CANTIDAD = p_nueva_cantidad,
        CAD_SUBTOTAL = p_nueva_cantidad * CAD_PRECIO_UNITARIO
    WHERE CAD_CARRITO_DETALLE = p_detalle_id;

    UPDATE ALP_CARRITO
    SET CAR_SUBTOTAL             = (SELECT NVL(SUM(CAD_SUBTOTAL), 0) FROM ALP_CARRITO_DETALLE WHERE CAR_CARRITO = v_carrito_id),
        CAR_FECHA_ACTUALIZACION  = CURRENT_TIMESTAMP
    WHERE CAR_CARRITO = v_carrito_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje := 'Cantidad actualizada correctamente';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje := 'Error Técnico: ' || SQLERRM;
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
    v_carrito_id        NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    BEGIN
        SELECT CAR_CARRITO INTO v_carrito_id
        FROM ALP_CARRITO_DETALLE
        WHERE CAD_CARRITO_DETALLE = p_detalle_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'El producto no existe en el carrito.';
        RAISE ex_negocio;
    END;

    DELETE FROM ALP_CARRITO_DETALLE
    WHERE CAD_CARRITO_DETALLE = p_detalle_id;
    
    -- Actualizar cabecera
    UPDATE ALP_CARRITO
    SET CAR_SUBTOTAL             = (SELECT NVL(SUM(CAD_SUBTOTAL), 0) FROM ALP_CARRITO_DETALLE WHERE CAR_CARRITO = v_carrito_id),
        CAR_FECHA_ACTUALIZACION  = CURRENT_TIMESTAMP
    WHERE CAR_CARRITO = v_carrito_id;
    
    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje := 'Producto removido del carrito correctamente';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje := 'Error Técnico: ' || SQLERRM;
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
    v_existe            NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    SELECT COUNT(*) INTO v_existe FROM ALP_CARRITO WHERE CAR_CARRITO = p_carrito_id;
    
    IF v_existe = 0 THEN
        p_mensaje := 'El carrito solicitado no existe.';
        RAISE ex_negocio;
    END IF;

    DELETE FROM ALP_CARRITO_DETALLE
    WHERE CAR_CARRITO = p_carrito_id;
    
    UPDATE ALP_CARRITO
    SET CAR_SUBTOTAL             = 0,
        CAR_FECHA_ACTUALIZACION  = CURRENT_TIMESTAMP
    WHERE CAR_CARRITO = p_carrito_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje := 'Carrito vaciado con éxito';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje := 'Error Técnico: ' || SQLERRM;
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
    -- 1. VALIDACIÓN: Carrito no vacío
    SELECT COUNT(*) INTO v_count
    FROM ALP_CARRITO_DETALLE
    WHERE CAR_CARRITO = p_carrito_id;

    IF v_count = 0 THEN
        p_mensaje := 'No se puede procesar un carrito vacío.';
        RAISE ex_negocio;
    END IF;

    -- 2. VALIDACIÓN: Existencia de estados maestros
    BEGIN
        SELECT ESO_ESTADO_ORDEN INTO v_estado_id
        FROM ALP_ESTADO_ORDEN
        WHERE ESO_CODIGO = 'PENDIENTE'
        FETCH FIRST 1 ROWS ONLY;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'Error de configuración: Estado PENDIENTE no encontrado.';
        RAISE ex_negocio;
    END;

    -- 3. CABECERA DE LA ORDEN
    BEGIN
        SELECT CLI_CLIENTE INTO v_cliente_id
        FROM ALP_CARRITO
        WHERE CAR_CARRITO = p_carrito_id
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'Carrito no encontrado.';
        RAISE ex_negocio;
    END;

    -- Cálculos de totales
    SELECT NVL(SUM(CAD_SUBTOTAL), 0) INTO v_subtotal
    FROM ALP_CARRITO_DETALLE WHERE CAR_CARRITO = p_carrito_id;
    
    v_impuestos := ROUND(v_subtotal * 0.12, 2);
    v_total := v_subtotal + v_impuestos;
    v_numero := 'ORD-' || TO_CHAR(SYSDATE, 'YYYYMMDD') || '-' || LPAD(p_carrito_id, 6, '0');

    -- Insertar Cabecera
    INSERT INTO ALP_ORDEN_VENTA (CLI_CLIENTE, CVE_CANAL_VENTA, VEN_NUMERO_ORDEN,
                                  VEN_SUBTOTAL, VEN_IMPUESTOS, VEN_TOTAL, ESO_ESTADO_ORDEN)
    VALUES (v_cliente_id, p_canal_venta, v_numero, v_subtotal, v_impuestos, v_total, v_estado_id)
    RETURNING VEN_ORDEN_VENTA INTO p_orden_id;

    -- 4. DETALLE DE LA ORDEN (Migración desde Carrito)
    INSERT INTO ALP_ORDEN_VENTA_DETALLE (VEN_ORDEN_VENTA, PRO_PRODUCTO, VDE_CANTIDAD,
                                          VDE_PRECIO_UNITARIO, VDE_SUBTOTAL, VDE_TOTAL, VDE_ESTADO)
    SELECT p_orden_id, PRO_PRODUCTO, CAD_CANTIDAD, CAD_PRECIO_UNITARIO,
           CAD_SUBTOTAL, CAD_SUBTOTAL, 'ACTIVO'
    FROM ALP_CARRITO_DETALLE
    WHERE CAR_CARRITO = p_carrito_id;

    -- 5. LIMPIEZA AUTOMÁTICA DEL CARRITO
    DELETE FROM ALP_CARRITO_DETALLE WHERE CAR_CARRITO = p_carrito_id;
    
    UPDATE ALP_CARRITO
    SET CAR_SUBTOTAL            = 0,
        CAR_FECHA_ACTUALIZACION = CURRENT_TIMESTAMP
    WHERE CAR_CARRITO = p_carrito_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Orden generada correctamente: ' || v_numero;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_orden_id  := NULL;
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := 'Error Técnico: ' || SQLERRM;
        p_orden_id  := NULL;
END;
/
