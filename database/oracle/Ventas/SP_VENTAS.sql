-- ==============================================================================
-- MÓDULO VENTAS - ESTÁNDAR PROFESIONAL MUEBLERÍA ALPES
-- Robustez, ACID y Auditoría Integrada
-- ==============================================================================

-- ==============================================================================
-- SP_CREAR_ORDEN_COMPLETA
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CREAR_ORDEN_COMPLETA (
    p_cliente_id        IN  NUMBER,
    p_canal_venta       IN  NUMBER,
    p_productos_ids     IN  SYS.ODCINUMBERLIST,
    p_cantidades        IN  SYS.ODCINUMBERLIST,
    p_orden_id          OUT NUMBER,
    p_total             OUT NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_subtotal          NUMBER := 0;
    v_impuestos         NUMBER;
    v_precio            NUMBER;
    v_stock             NUMBER;
    v_numero            VARCHAR2(50);
    v_estado_id         NUMBER;
    c_iva               CONSTANT NUMBER := 0.12;
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. VALIDACIÓN DE PARÁMETROS
    IF p_productos_ids.COUNT = 0 OR p_productos_ids.COUNT != p_cantidades.COUNT THEN
        p_mensaje := 'Error en estructura de productos o cantidades.';
        RAISE ex_negocio;
    END IF;

    -- 2. VALIDACIÓN DE CATÁLOGOS (Estado Inicial)
    BEGIN
        SELECT ESO_ESTADO_ORDEN INTO v_estado_id
        FROM ALP_ESTADO_ORDEN
        WHERE ESO_CODIGO = 'PENDIENTE'
        FETCH FIRST 1 ROWS ONLY;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'Error de configuración: Estado PENDIENTE no encontrado.';
        RAISE ex_negocio;
    END;

    -- 3. VALIDACIÓN DE PRODUCTOS Y STOCK (Pre-calculo)
    FOR i IN 1..p_productos_ids.COUNT LOOP
        -- Bloqueo pesimista para proteger el stock durante la transacción
        UPDATE ALP_EXISTENCIA 
        SET EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
        WHERE PRO_PRODUCTO = p_productos_ids(i);

        SELECT NVL(SUM(EXI_CANTIDAD_DISPONIBLE), 0) INTO v_stock
        FROM ALP_EXISTENCIA
        WHERE PRO_PRODUCTO = p_productos_ids(i);

        IF v_stock < p_cantidades(i) THEN
            p_mensaje := 'Stock insuficiente para producto ID: ' || p_productos_ids(i) || '. Disponible: ' || v_stock;
            RAISE ex_negocio;
        END IF;

        BEGIN
            SELECT PPR_PRECIO INTO v_precio
            FROM ALP_PRODUCTO_PRECIO
            WHERE PRO_PRODUCTO = p_productos_ids(i) AND PPR_ESTADO = 'ACTIVO'
            FETCH FIRST 1 ROWS ONLY;
        EXCEPTION WHEN NO_DATA_FOUND THEN
            p_mensaje := 'El producto #' || p_productos_ids(i) || ' no tiene un precio activo configurado.';
            RAISE ex_negocio;
        END;

        v_subtotal := v_subtotal + (p_cantidades(i) * v_precio);
    END LOOP;

    -- 4. GENERACIÓN DE CABECERA
    v_impuestos := ROUND(v_subtotal * c_iva, 2);
    p_total     := v_subtotal + v_impuestos;
    v_numero    := 'ORD-' || TO_CHAR(SYSDATE, 'YYYYMMDD-HH24MISS');

    INSERT INTO ALP_ORDEN_VENTA (CLI_CLIENTE, CVE_CANAL_VENTA, VEN_NUMERO_ORDEN,
                                  VEN_SUBTOTAL, VEN_IMPUESTOS, VEN_TOTAL, ESO_ESTADO_ORDEN)
    VALUES (p_cliente_id, p_canal_venta, v_numero, v_subtotal, v_impuestos, p_total, v_estado_id)
    RETURNING VEN_ORDEN_VENTA INTO p_orden_id;

    -- 5. GENERACIÓN DE DETALLES Y RESERVA DE STOCK
    FOR i IN 1..p_productos_ids.COUNT LOOP
        SELECT PPR_PRECIO INTO v_precio
        FROM ALP_PRODUCTO_PRECIO
        WHERE PRO_PRODUCTO = p_productos_ids(i) AND PPR_ESTADO = 'ACTIVO'
        FETCH FIRST 1 ROWS ONLY;

        INSERT INTO ALP_ORDEN_VENTA_DETALLE (VEN_ORDEN_VENTA, PRO_PRODUCTO, VDE_CANTIDAD,
                                              VDE_PRECIO_UNITARIO, VDE_SUBTOTAL, VDE_TOTAL, VDE_ESTADO)
        VALUES (p_orden_id, p_productos_ids(i), p_cantidades(i), v_precio,
                p_cantidades(i) * v_precio, p_cantidades(i) * v_precio, 'ACTIVO');

        -- Descontar de disponible y mover a reservado
        UPDATE ALP_EXISTENCIA
        SET EXI_CANTIDAD_RESERVADA   = EXI_CANTIDAD_RESERVADA + p_cantidades(i),
            EXI_CANTIDAD_DISPONIBLE  = EXI_CANTIDAD_DISPONIBLE - p_cantidades(i),
            EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
        WHERE PRO_PRODUCTO = p_productos_ids(i)
          AND ROWID = (SELECT MIN(ROWID) FROM ALP_EXISTENCIA WHERE PRO_PRODUCTO = p_productos_ids(i));
    END LOOP;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Orden #' || v_numero || ' creada y stock reservado.';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_orden_id  := NULL;
        p_total     := 0;
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := 'Error Técnico: ' || SQLERRM;
        p_orden_id  := NULL;
        p_total     := 0;
END;
/

-- ==============================================================================
-- SP_ACTUALIZAR_ESTADO_ORDEN
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_ACTUALIZAR_ESTADO_ORDEN (
    p_orden_id          IN  NUMBER,
    p_nuevo_estado      IN  NUMBER,
    p_usuario_id        IN  NUMBER,
    p_comentario        IN  VARCHAR2,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_estado_anterior   NUMBER;
    v_codigo_nuevo      VARCHAR2(50);
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. VALIDACIÓN DE EXISTENCIA DE LA ORDEN
    BEGIN
        SELECT ESO_ESTADO_ORDEN INTO v_estado_anterior
        FROM ALP_ORDEN_VENTA
        WHERE VEN_ORDEN_VENTA = p_orden_id
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'La orden #' || p_orden_id || ' no existe.';
        RAISE ex_negocio;
    END;

    -- 2. VALIDACIÓN DEL NUEVO ESTADO
    BEGIN
        SELECT ESO_CODIGO INTO v_codigo_nuevo
        FROM ALP_ESTADO_ORDEN
        WHERE ESO_ESTADO_ORDEN = p_nuevo_estado;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'El estado destino solicitado no es válido.';
        RAISE ex_negocio;
    END;

    -- 3. ACTUALIZACIÓN Y LOGS
    UPDATE ALP_ORDEN_VENTA
    SET ESO_ESTADO_ORDEN = p_nuevo_estado
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    INSERT INTO ALP_ORDEN_VENTA_ESTADO_HISTORIAL
        (VEN_ORDEN_VENTA, ESO_ESTADO_ORDEN_ANTERIOR, ESO_ESTADO_ORDEN_NUEVO, VEH_COMENTARIO, USU_USUARIO)
    VALUES (p_orden_id, v_estado_anterior, p_nuevo_estado, p_comentario, p_usuario_id);

    INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
    VALUES (p_usuario_id, 'ALP_ORDEN_VENTA', 'ESTADO_UPDATE', p_orden_id,
            JSON_OBJECT('anterior' VALUE v_estado_anterior, 'nuevo' VALUE p_nuevo_estado, 'comentario' VALUE p_comentario));

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Estado de orden actualizado a: ' || v_codigo_nuevo;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := 'Error Técnico: ' || SQLERRM;
END;
/

-- ==============================================================================
-- SP_CANCELAR_ORDEN
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_CANCELAR_ORDEN (
    p_orden_id          IN  NUMBER,
    p_motivo            IN  VARCHAR2,
    p_usuario_id        IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_estado_cancelado  NUMBER;
    v_estado_actual_id  NUMBER;
    v_cod_actual        VARCHAR2(50);
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. OBTENER ESTADO DESTINO
    BEGIN
        SELECT ESO_ESTADO_ORDEN INTO v_estado_cancelado
        FROM ALP_ESTADO_ORDEN
        WHERE ESO_CODIGO = 'CANCELADO'
        FETCH FIRST 1 ROWS ONLY;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'Configuración fallida: Estado CANCELADO no existe.';
        RAISE ex_negocio;
    END;

    -- 2. VALIDAR ORDEN Y SU ESTADO ACTUAL
    BEGIN
        SELECT ov.ESO_ESTADO_ORDEN, eo.ESO_CODIGO
        INTO v_estado_actual_id, v_cod_actual
        FROM ALP_ORDEN_VENTA ov
        JOIN ALP_ESTADO_ORDEN eo ON ov.ESO_ESTADO_ORDEN = eo.ESO_ESTADO_ORDEN
        WHERE ov.VEN_ORDEN_VENTA = p_orden_id
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'La orden no existe.';
        RAISE ex_negocio;
    END;

    IF v_cod_actual IN ('CANCELADO', 'ENTREGADO') THEN
        p_mensaje := 'No se puede cancelar una orden que ya está ' || v_cod_actual;
        RAISE ex_negocio;
    END IF;

    -- 3. REVERSA DE STOCK RESERVADO
    FOR det IN (
        SELECT PRO_PRODUCTO, VDE_CANTIDAD
        FROM ALP_ORDEN_VENTA_DETALLE
        WHERE VEN_ORDEN_VENTA = p_orden_id AND VDE_ESTADO = 'ACTIVO'
    ) LOOP
        UPDATE ALP_EXISTENCIA
        SET EXI_CANTIDAD_RESERVADA   = EXI_CANTIDAD_RESERVADA - det.VDE_CANTIDAD,
            EXI_CANTIDAD_DISPONIBLE  = EXI_CANTIDAD_DISPONIBLE + det.VDE_CANTIDAD,
            EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
        WHERE PRO_PRODUCTO = det.PRO_PRODUCTO
          AND ROWID = (SELECT MIN(ROWID) FROM ALP_EXISTENCIA WHERE PRO_PRODUCTO = det.PRO_PRODUCTO);
    END LOOP;

    -- 4. ACTUALIZACIÓN DE ESTADOS
    UPDATE ALP_ORDEN_VENTA_DETALLE
    SET VDE_ESTADO = 'CANCELADO'
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    UPDATE ALP_ORDEN_VENTA
    SET ESO_ESTADO_ORDEN = v_estado_cancelado
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    INSERT INTO ALP_ORDEN_VENTA_ESTADO_HISTORIAL
        (VEN_ORDEN_VENTA, ESO_ESTADO_ORDEN_ANTERIOR, ESO_ESTADO_ORDEN_NUEVO, VEH_COMENTARIO, USU_USUARIO)
    VALUES (p_orden_id, v_estado_actual_id, v_estado_cancelado, p_motivo, p_usuario_id);

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Orden cancelada y stock devuelto a inventario.';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := 'Error Técnico: ' || SQLERRM;
END;
/

-- ==============================================================================
-- SP_APLICAR_PROMOCION
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_APLICAR_PROMOCION (
    p_orden_id          IN  NUMBER,
    p_promocion_id      IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_descuento_pct     NUMBER;
    v_subtotal          NUMBER;
    v_descuento         NUMBER;
    v_impuestos         NUMBER;
    v_total             NUMBER;
    v_cod_estado        VARCHAR2(50);
    c_iva               CONSTANT NUMBER := 0.12;
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. VALIDACIÓN DE PROMOCIÓN
    BEGIN
        SELECT PRM_VALOR INTO v_descuento_pct
        FROM ALP_PROMOCION
        WHERE PRM_PROMOCION = p_promocion_id
          AND PRM_TIPO      = 'PORCENTAJE'
          AND PRM_ESTADO    = 'ACTIVO'
          AND CURRENT_TIMESTAMP BETWEEN PRM_FECHA_INICIO AND PRM_FECHA_FIN;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'La promoción no es válida, no está activa o ya expiró.';
        RAISE ex_negocio;
    END;

    -- 2. VALIDACIÓN DE ESTADO DE LA ORDEN
    BEGIN
        SELECT ov.VEN_SUBTOTAL, eo.ESO_CODIGO
        INTO v_subtotal, v_cod_estado
        FROM ALP_ORDEN_VENTA ov
        JOIN ALP_ESTADO_ORDEN eo ON ov.ESO_ESTADO_ORDEN = eo.ESO_ESTADO_ORDEN
        WHERE ov.VEN_ORDEN_VENTA = p_orden_id
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'La orden no existe.';
        RAISE ex_negocio;
    END;

    IF v_cod_estado != 'PENDIENTE' THEN
        p_mensaje := 'Solo se pueden aplicar promociones a órdenes en estado PENDIENTE.';
        RAISE ex_negocio;
    END IF;

    -- 3. CÁLCULO Y ACTUALIZACIÓN
    v_descuento := ROUND(v_subtotal * (v_descuento_pct / 100), 2);
    v_impuestos := ROUND((v_subtotal - v_descuento) * c_iva, 2);
    v_total     := (v_subtotal - v_descuento) + v_impuestos;

    UPDATE ALP_ORDEN_VENTA
    SET VEN_DESCUENTO = v_descuento,
        VEN_IMPUESTOS = v_impuestos,
        VEN_TOTAL     = v_total
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Promoción aplicada. Descuento: ' || v_descuento_pct || '% (Q' || v_descuento || ')';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := 'Error Técnico: ' || SQLERRM;
END;
/