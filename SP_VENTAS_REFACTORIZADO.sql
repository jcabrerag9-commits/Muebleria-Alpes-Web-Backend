-- ==============================================================================
-- MÓDULO VENTAS - REFACTORIZADO ORACLE 21c
-- Estilo: ACID, Profesional, Optimizado
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
    IF p_productos_ids.COUNT != p_cantidades.COUNT THEN
        p_mensaje := 'Arrays deben tener igual tamaño';
        RAISE ex_negocio;
    END IF;

    SELECT ESO_ESTADO_ORDEN INTO v_estado_id
    FROM ALP_ESTADO_ORDEN
    WHERE ESO_CODIGO = 'PENDIENTE'
    FETCH FIRST 1 ROWS ONLY;

    -- FIX: FOR UPDATE no permitido sobre expresión con SUM/GROUP; bloqueo por UPDATE
    FOR i IN 1..p_productos_ids.COUNT LOOP
        SELECT NVL(SUM(EXI_CANTIDAD_DISPONIBLE), 0) INTO v_stock
        FROM ALP_EXISTENCIA
        WHERE PRO_PRODUCTO = p_productos_ids(i);

        UPDATE ALP_EXISTENCIA SET EXI_ULTIMA_ACTUALIZACION = EXI_ULTIMA_ACTUALIZACION
        WHERE PRO_PRODUCTO = p_productos_ids(i);

        IF v_stock < p_cantidades(i) THEN
            p_mensaje := 'Stock insuficiente producto #' || p_productos_ids(i);
            RAISE ex_negocio;
        END IF;

        SELECT PPR_PRECIO INTO v_precio
        FROM ALP_PRODUCTO_PRECIO
        WHERE PRO_PRODUCTO = p_productos_ids(i) AND PPR_ESTADO = 'ACTIVO'
        FETCH FIRST 1 ROWS ONLY;

        v_subtotal := v_subtotal + (p_cantidades(i) * v_precio);
    END LOOP;

    v_impuestos := ROUND(v_subtotal * c_iva, 2);
    p_total     := v_subtotal + v_impuestos;
    v_numero    := 'ORD-' || TO_CHAR(SYSDATE, 'YYYYMMDD-HH24MISS');

    -- FIX: ALP_ORDEN_VENTA no tiene VEN_ESTADO
    INSERT INTO ALP_ORDEN_VENTA (CLI_CLIENTE, CVE_CANAL_VENTA, VEN_NUMERO_ORDEN,
                                  VEN_SUBTOTAL, VEN_IMPUESTOS, VEN_TOTAL, ESO_ESTADO_ORDEN)
    VALUES (p_cliente_id, p_canal_venta, v_numero, v_subtotal, v_impuestos, p_total, v_estado_id)
    RETURNING VEN_ORDEN_VENTA INTO p_orden_id;

    FOR i IN 1..p_productos_ids.COUNT LOOP
        SELECT PPR_PRECIO INTO v_precio
        FROM ALP_PRODUCTO_PRECIO
        WHERE PRO_PRODUCTO = p_productos_ids(i) AND PPR_ESTADO = 'ACTIVO'
        FETCH FIRST 1 ROWS ONLY;

        -- FIX: VDE_TOTAL es NOT NULL
        INSERT INTO ALP_ORDEN_VENTA_DETALLE (VEN_ORDEN_VENTA, PRO_PRODUCTO, VDE_CANTIDAD,
                                              VDE_PRECIO_UNITARIO, VDE_SUBTOTAL, VDE_TOTAL, VDE_ESTADO)
        VALUES (p_orden_id, p_productos_ids(i), p_cantidades(i), v_precio,
                p_cantidades(i) * v_precio, p_cantidades(i) * v_precio, 'ACTIVO');

        UPDATE ALP_EXISTENCIA
        SET EXI_CANTIDAD_RESERVADA   = EXI_CANTIDAD_RESERVADA + p_cantidades(i),
            EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
        WHERE PRO_PRODUCTO = p_productos_ids(i)
          AND ROWID = (SELECT MIN(ROWID) FROM ALP_EXISTENCIA WHERE PRO_PRODUCTO = p_productos_ids(i));
    END LOOP;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Orden creada: ' || v_numero;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_orden_id  := NULL;
        p_total     := 0;
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := SQLERRM;
        p_orden_id  := NULL;
        p_total     := 0;
END;
/

-- ------------------------------------------------------------------------------

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
BEGIN
    SELECT ESO_ESTADO_ORDEN INTO v_estado_anterior
    FROM ALP_ORDEN_VENTA
    WHERE VEN_ORDEN_VENTA = p_orden_id
    FOR UPDATE;

    UPDATE ALP_ORDEN_VENTA
    SET ESO_ESTADO_ORDEN = p_nuevo_estado
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    -- FIX: columnas correctas según DDL: ESO_ESTADO_ORDEN_ANTERIOR, ESO_ESTADO_ORDEN_NUEVO, VEH_COMENTARIO, USU_USUARIO
    INSERT INTO ALP_ORDEN_VENTA_ESTADO_HISTORIAL
        (VEN_ORDEN_VENTA, ESO_ESTADO_ORDEN_ANTERIOR, ESO_ESTADO_ORDEN_NUEVO, VEH_COMENTARIO, USU_USUARIO)
    VALUES (p_orden_id, v_estado_anterior, p_nuevo_estado, p_comentario, p_usuario_id);

    -- FIX: columna es TRL_ENTIDAD (no TRL_TABLA)
    INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
    VALUES (p_usuario_id, 'ALP_ORDEN_VENTA', 'UPDATE', p_orden_id,
            JSON_OBJECT('estado_anterior' VALUE v_estado_anterior, 'estado_nuevo' VALUE p_nuevo_estado));

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Estado actualizado';

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := SQLERRM;
END;
/

-- ------------------------------------------------------------------------------

CREATE OR REPLACE PROCEDURE SP_CANCELAR_ORDEN (
    p_orden_id          IN  NUMBER,
    p_motivo            IN  VARCHAR2,
    p_usuario_id        IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_estado_cancelado  NUMBER;
    v_estado_actual     NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    SELECT ESO_ESTADO_ORDEN INTO v_estado_cancelado
    FROM ALP_ESTADO_ORDEN
    WHERE ESO_CODIGO = 'CANCELADO'
    FETCH FIRST 1 ROWS ONLY;

    SELECT ESO_ESTADO_ORDEN INTO v_estado_actual
    FROM ALP_ORDEN_VENTA
    WHERE VEN_ORDEN_VENTA = p_orden_id
    FOR UPDATE;

    IF v_estado_actual = v_estado_cancelado THEN
        p_mensaje := 'Orden ya cancelada';
        RAISE ex_negocio;
    END IF;

    FOR det IN (
        SELECT PRO_PRODUCTO, VDE_CANTIDAD
        FROM ALP_ORDEN_VENTA_DETALLE
        WHERE VEN_ORDEN_VENTA = p_orden_id AND VDE_ESTADO = 'ACTIVO'
    ) LOOP
        UPDATE ALP_EXISTENCIA
        SET EXI_CANTIDAD_RESERVADA   = EXI_CANTIDAD_RESERVADA   - det.VDE_CANTIDAD,
            EXI_CANTIDAD_DISPONIBLE  = EXI_CANTIDAD_DISPONIBLE  + det.VDE_CANTIDAD,
            EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
        WHERE PRO_PRODUCTO = det.PRO_PRODUCTO
          AND ROWID = (SELECT MIN(ROWID) FROM ALP_EXISTENCIA WHERE PRO_PRODUCTO = det.PRO_PRODUCTO);
    END LOOP;

    UPDATE ALP_ORDEN_VENTA_DETALLE
    SET VDE_ESTADO = 'CANCELADO'
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    -- FIX: ALP_ORDEN_VENTA no tiene VEN_ESTADO
    UPDATE ALP_ORDEN_VENTA
    SET ESO_ESTADO_ORDEN = v_estado_cancelado
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    -- FIX: columnas correctas: VEH_COMENTARIO, USU_USUARIO
    INSERT INTO ALP_ORDEN_VENTA_ESTADO_HISTORIAL
        (VEN_ORDEN_VENTA, ESO_ESTADO_ORDEN_ANTERIOR, ESO_ESTADO_ORDEN_NUEVO, VEH_COMENTARIO, USU_USUARIO)
    VALUES (p_orden_id, v_estado_actual, v_estado_cancelado, p_motivo, p_usuario_id);

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Orden cancelada, stock devuelto';

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := SQLERRM;
END;
/

-- ------------------------------------------------------------------------------

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
    c_iva               CONSTANT NUMBER := 0.12;
BEGIN
    -- FIX: columna correcta es PRM_VALOR (no PRM_DESCUENTO_PORCENTAJE); tipo PORCENTAJE
    SELECT PRM_VALOR INTO v_descuento_pct
    FROM ALP_PROMOCION
    WHERE PRM_PROMOCION = p_promocion_id
      AND PRM_TIPO      = 'PORCENTAJE'
      AND PRM_ESTADO    = 'ACTIVO'
      AND SYSDATE BETWEEN PRM_FECHA_INICIO AND PRM_FECHA_FIN;

    SELECT VEN_SUBTOTAL INTO v_subtotal
    FROM ALP_ORDEN_VENTA
    WHERE VEN_ORDEN_VENTA = p_orden_id
    FOR UPDATE;

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
    p_mensaje   := 'Descuento Q' || v_descuento || ' (' || v_descuento_pct || '%)';

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := 'Promoción no válida o expirada';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := SQLERRM;
END;
/

-- ------------------------------------------------------------------------------

CREATE OR REPLACE PROCEDURE SP_CALCULAR_TOTALES_ORDEN (
    p_orden_id          IN  NUMBER,
    p_subtotal          OUT NUMBER,
    p_impuestos         OUT NUMBER,
    p_total             OUT NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_descuento         NUMBER;
    v_envio             NUMBER;
    c_iva               CONSTANT NUMBER := 0.12;
BEGIN
    SELECT NVL(SUM(VDE_SUBTOTAL), 0) INTO p_subtotal
    FROM ALP_ORDEN_VENTA_DETALLE
    WHERE VEN_ORDEN_VENTA = p_orden_id AND VDE_ESTADO = 'ACTIVO';

    SELECT NVL(VEN_DESCUENTO, 0), NVL(VEN_ENVIO, 0)
    INTO v_descuento, v_envio
    FROM ALP_ORDEN_VENTA
    WHERE VEN_ORDEN_VENTA = p_orden_id
    FOR UPDATE;

    p_impuestos := ROUND((p_subtotal - v_descuento) * c_iva, 2);
    p_total     := (p_subtotal - v_descuento) + p_impuestos + v_envio;

    UPDATE ALP_ORDEN_VENTA
    SET VEN_SUBTOTAL  = p_subtotal,
        VEN_IMPUESTOS = p_impuestos,
        VEN_TOTAL     = p_total
    WHERE VEN_ORDEN_VENTA = p_orden_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Totales recalculados';

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := SQLERRM;
        p_subtotal  := 0;
        p_impuestos := 0;
        p_total     := 0;
END;
/