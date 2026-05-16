-- ==============================================================================
-- MÓDULO PAGOS - ESTÁNDAR PROFESIONAL MUEBLERÍA ALPES
-- Robustez, Seguridad ACID y Auditoría Integrada
-- ==============================================================================

-- ==============================================================================
-- SP_PROCESAR_PAGO (Versión Final)
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_PROCESAR_PAGO (
    p_orden_id          IN  NUMBER,
    p_forma_pago        IN  NUMBER,
    p_monto             IN  NUMBER,
    p_moneda_id         IN  NUMBER,
    p_referencia        IN  VARCHAR2,
    p_pago_id           OUT NUMBER,
    p_factura_id        OUT NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_total             NUMBER;
    v_estado_aprobado   NUMBER;
    v_estado_procesando NUMBER;
    v_estado_actual_id  NUMBER;
    v_codigo_actual     VARCHAR2(50);
    v_resultado         VARCHAR2(50);
    v_mensaje           VARCHAR2(500);
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. VALIDACIÓN PREVENTIVA: Existencia y Estado de la Orden
    BEGIN
        SELECT ov.VEN_TOTAL, ov.ESO_ESTADO_ORDEN, eo.ESO_CODIGO
        INTO   v_total, v_estado_actual_id, v_codigo_actual
        FROM   ALP_ORDEN_VENTA ov
        JOIN   ALP_ESTADO_ORDEN eo ON ov.ESO_ESTADO_ORDEN = eo.ESO_ESTADO_ORDEN
        WHERE  ov.VEN_ORDEN_VENTA = p_orden_id
        FOR UPDATE;
    EXCEPTION 
        WHEN NO_DATA_FOUND THEN
            p_mensaje := 'Error: La orden #' || p_orden_id || ' no existe en el sistema.';
            RAISE ex_negocio;
    END;

    -- 2. VALIDACIÓN DE REGLA DE NEGOCIO: Estado permitido
    IF v_codigo_actual NOT IN ('PENDIENTE', 'PROCESANDO') THEN
        p_mensaje := 'Operación no permitida: La orden se encuentra en estado ' || v_codigo_actual;
        RAISE ex_negocio;
    END IF;

    -- 3. VALIDACIÓN DE MONTO
    IF p_monto < v_total THEN
        p_mensaje := 'Monto insuficiente. El total de la orden es Q' || v_total || ' y se recibió Q' || p_monto;
        RAISE ex_negocio;
    END IF;

    -- 4. OBTENCIÓN SEGURA DE ESTADOS MAESTROS
    BEGIN
        SELECT ESP_ESTADO_PAGO INTO v_estado_aprobado
        FROM ALP_ESTADO_PAGO WHERE ESP_CODIGO = 'APROBADO' FETCH FIRST 1 ROWS ONLY;

        SELECT ESO_ESTADO_ORDEN INTO v_estado_procesando
        FROM ALP_ESTADO_ORDEN WHERE ESO_CODIGO = 'PROCESANDO' FETCH FIRST 1 ROWS ONLY;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            p_mensaje := 'Error de configuración: No se encontraron los códigos de estado APROBADO o PROCESANDO.';
            RAISE ex_negocio;
    END;

    -- 5. TRANSICIÓN AUTOMÁTICA
    IF v_codigo_actual = 'PENDIENTE' THEN
        UPDATE ALP_ORDEN_VENTA SET ESO_ESTADO_ORDEN = v_estado_procesando WHERE VEN_ORDEN_VENTA = p_orden_id;
    END IF;

    -- 6. REGISTRO DEL PAGO
    INSERT INTO ALP_PAGO (VEN_ORDEN_VENTA, FPA_FORMA_PAGO, PAG_MONTO, PAG_MONEDA,
                          PAG_REFERENCIA, ESP_ESTADO_PAGO)
    VALUES (p_orden_id, p_forma_pago, p_monto, p_moneda_id,
            p_referencia, v_estado_aprobado)
    RETURNING PAG_PAGO INTO p_pago_id;

    INSERT INTO ALP_PAGO_DETALLE (PAG_PAGO, VEN_ORDEN_VENTA, PDE_MONTO_APLICADO)
    VALUES (p_pago_id, p_orden_id, p_monto);

    INSERT INTO ALP_INTENTO_PAGO (VEN_ORDEN_VENTA, FPA_FORMA_PAGO, IPA_MONTO, IPA_RESULTADO, IPA_MENSAJE)
    VALUES (p_orden_id, p_forma_pago, p_monto, 'EXITOSO', 'Pago aprobado y procesado');

    -- 7. ACTUALIZACIÓN DE EXISTENCIAS (Venta Real)
    FOR det IN (
        SELECT PRO_PRODUCTO, VDE_CANTIDAD
        FROM ALP_ORDEN_VENTA_DETALLE
        WHERE VEN_ORDEN_VENTA = p_orden_id AND VDE_ESTADO = 'ACTIVO'
    ) LOOP
        UPDATE ALP_EXISTENCIA
        SET EXI_CANTIDAD_RESERVADA   = EXI_CANTIDAD_RESERVADA - det.VDE_CANTIDAD,
            EXI_ULTIMA_ACTUALIZACION = CURRENT_TIMESTAMP
        WHERE PRO_PRODUCTO = det.PRO_PRODUCTO
          AND ROWID = (SELECT MIN(ROWID) FROM ALP_EXISTENCIA WHERE PRO_PRODUCTO = det.PRO_PRODUCTO);
    END LOOP;

    -- 8. INTEGRACIÓN CON FACTURACIÓN
    SP_GENERAR_FACTURA_ELECTRONICA(p_orden_id, p_pago_id, p_factura_id, v_resultado, v_mensaje);

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Pago exitoso #' || p_pago_id || '. Factura generada: ' || p_factura_id;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado  := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado  := 'ERROR';
        p_mensaje    := 'Error Técnico: ' || SQLERRM;
END;
/

-- ==============================================================================
-- SP_REGISTRAR_INTENTO_PAGO
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_REGISTRAR_INTENTO_PAGO (
    p_orden_id          IN  NUMBER,
    p_forma_pago        IN  NUMBER,
    p_monto             IN  NUMBER,
    p_resultado_pago    IN  VARCHAR2,
    p_mensaje_pago      IN  VARCHAR2,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
BEGIN
    INSERT INTO ALP_INTENTO_PAGO (VEN_ORDEN_VENTA, FPA_FORMA_PAGO, IPA_MONTO, IPA_RESULTADO, IPA_MENSAJE)
    VALUES (p_orden_id, p_forma_pago, p_monto, p_resultado_pago, p_mensaje_pago);

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Intento de pago registrado.';
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado := 'ERROR';
        p_mensaje   := SQLERRM;
END;
/

-- ==============================================================================
-- SP_GENERAR_FACTURA_ELECTRONICA
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_GENERAR_FACTURA_ELECTRONICA (
    p_orden_id          IN  NUMBER,
    p_pago_id           IN  NUMBER,
    p_factura_id        OUT NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_cliente_id        NUMBER;
    v_subtotal          NUMBER;
    v_impuestos         NUMBER;
    v_total             NUMBER;
    v_numero            VARCHAR2(50);
    v_serie             VARCHAR2(20) := 'A';
    v_tipo_id           NUMBER;
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. VALIDACIÓN DE EXISTENCIA DE ORDEN
    BEGIN
        SELECT CLI_CLIENTE, VEN_SUBTOTAL, VEN_IMPUESTOS, VEN_TOTAL
        INTO v_cliente_id, v_subtotal, v_impuestos, v_total
        FROM ALP_ORDEN_VENTA
        WHERE VEN_ORDEN_VENTA = p_orden_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'No se puede facturar una orden inexistente.';
        RAISE ex_negocio;
    END;

    v_numero := v_serie || LPAD(p_orden_id, 8, '0');

    -- 2. VALIDACIÓN DE CATÁLOGO
    BEGIN
        SELECT TCO_TIPO_COMPROBANTE INTO v_tipo_id
        FROM ALP_TIPO_COMPROBANTE
        WHERE TCO_CODIGO = 'FACTURA_ELECTRONICA'
        FETCH FIRST 1 ROWS ONLY;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'Error de configuración: Tipo de comprobante FACTURA_ELECTRONICA no encontrado.';
        RAISE ex_negocio;
    END;

    -- 3. INSERCIÓN DE DOCUMENTOS
    INSERT INTO ALP_COMPROBANTE (PAG_PAGO, TCO_TIPO_COMPROBANTE, COM_NUMERO, COM_SERIE, COM_MONTO)
    VALUES (p_pago_id, v_tipo_id, v_numero, v_serie, v_total);

    INSERT INTO ALP_FACTURA (VEN_ORDEN_VENTA, CLI_CLIENTE, FAC_NUMERO, FAC_SERIE,
                             FAC_SUBTOTAL, FAC_IMPUESTOS, FAC_TOTAL, FAC_ESTADO)
    VALUES (p_orden_id, v_cliente_id, v_numero, v_serie,
            v_subtotal, v_impuestos, v_total, 'EMITIDA')
    RETURNING FAC_FACTURA INTO p_factura_id;

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Factura electrónica generada: ' || v_numero;

EXCEPTION
    WHEN ex_negocio THEN
        ROLLBACK;
        p_resultado := 'ERROR';
    WHEN OTHERS THEN
        ROLLBACK;
        p_resultado  := 'ERROR';
        p_mensaje    := 'Error Técnico: ' || SQLERRM;
        p_factura_id := NULL;
END;
/

-- ==============================================================================
-- SP_PROCESAR_REEMBOLSO
-- ==============================================================================
CREATE OR REPLACE PROCEDURE SP_PROCESAR_REEMBOLSO (
    p_pago_id           IN  NUMBER,
    p_motivo            IN  VARCHAR2,
    p_usuario_id        IN  NUMBER,
    p_resultado         OUT VARCHAR2,
    p_mensaje           OUT VARCHAR2
)
IS
    v_monto             NUMBER;
    v_orden_id          NUMBER;
    v_forma_pago        NUMBER;
    v_estado_reembolso  NUMBER;
    v_res_cancel        VARCHAR2(50);
    v_msg_cancel        VARCHAR2(500);
    ex_negocio          EXCEPTION;
BEGIN
    -- 1. VALIDACIÓN DE EXISTENCIA DE PAGO
    BEGIN
        SELECT PAG_MONTO, VEN_ORDEN_VENTA, FPA_FORMA_PAGO
        INTO v_monto, v_orden_id, v_forma_pago
        FROM ALP_PAGO
        WHERE PAG_PAGO = p_pago_id
        FOR UPDATE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'El pago solicitado para reembolso no existe.';
        RAISE ex_negocio;
    END;

    -- 2. VALIDACIÓN DE CATÁLOGO
    BEGIN
        SELECT ESP_ESTADO_PAGO INTO v_estado_reembolso
        FROM ALP_ESTADO_PAGO
        WHERE ESP_CODIGO = 'REEMBOLSADO'
        FETCH FIRST 1 ROWS ONLY;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        p_mensaje := 'Error de configuración: Estado REEMBOLSADO no encontrado.';
        RAISE ex_negocio;
    END;

    -- 3. ACTUALIZACIONES Y REVERSA
    UPDATE ALP_PAGO SET ESP_ESTADO_PAGO = v_estado_reembolso WHERE PAG_PAGO = p_pago_id;

    UPDATE ALP_FACTURA SET FAC_ESTADO = 'ANULADA' WHERE VEN_ORDEN_VENTA = v_orden_id;

    -- Llamada a cancelación de orden con su propia lógica de reversa de stock
    SP_CANCELAR_ORDEN(v_orden_id, p_motivo, p_usuario_id, v_res_cancel, v_msg_cancel);

    IF v_res_cancel = 'ERROR' THEN
        p_mensaje := 'Error al cancelar la orden: ' || v_msg_cancel;
        RAISE ex_negocio;
    END IF;

    INSERT INTO ALP_INTENTO_PAGO (VEN_ORDEN_VENTA, FPA_FORMA_PAGO, IPA_MONTO, IPA_RESULTADO, IPA_MENSAJE)
    VALUES (v_orden_id, v_forma_pago, v_monto, 'REEMBOLSADO', p_motivo);

    COMMIT;
    p_resultado := 'EXITO';
    p_mensaje   := 'Reembolso procesado por monto de Q' || v_monto;

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
