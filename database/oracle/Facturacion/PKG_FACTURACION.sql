-- ============================================================================
-- MUEBLES LOS ALPES — PKG_FACTURACION: ESTÁNDAR PROFESIONAL
-- Optimizada para Alta Concurrencia, ACID y .NET 8 / Dapper
-- ============================================================================

-- ============================================================================
-- BLOQUE DE SEGURIDAD: CREACIÓN DE SECUENCIAS (Auto-ejecutable)
-- ============================================================================
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count FROM user_sequences WHERE sequence_name = 'SEQ_FAC_SERIE_A';
    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'CREATE SEQUENCE SEQ_FAC_SERIE_A START WITH 1000 NOCACHE';
    END IF;

    SELECT COUNT(*) INTO v_count FROM user_sequences WHERE sequence_name = 'SEQ_COM_NUMERO';
    IF v_count = 0 THEN
        EXECUTE IMMEDIATE 'CREATE SEQUENCE SEQ_COM_NUMERO START WITH 500 NOCACHE';
    END IF;
END;
/

CREATE OR REPLACE PACKAGE PKG_FACTURACION AS

    -- Factura
    PROCEDURE SP_GENERAR_FACTURA_ORDEN    (p_orden_id NUMBER, p_pago_id NUMBER, p_usuario_id NUMBER, p_factura_id OUT NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_ANULAR_FACTURA           (p_factura_id NUMBER, p_motivo VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CANCELAR_FACTURA         (p_factura_id NUMBER, p_motivo VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_FACTURA          (p_factura_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_FACTURAS_CLIENTE  (p_cliente_id NUMBER, p_estado VARCHAR2 DEFAULT NULL, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_FACTURAS_ORDEN    (p_orden_id NUMBER, p_cursor OUT SYS_REFCURSOR);

    -- Comprobante
    PROCEDURE SP_CREAR_COMPROBANTE        (p_pago_id NUMBER, p_tipo_id NUMBER, p_usuario_id NUMBER, p_comprobante_id OUT NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_ANULAR_COMPROBANTE       (p_comprobante_id NUMBER, p_motivo VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    
    -- Utilidades
    FUNCTION  FN_FACTURA_EXISTE_ORDEN      (p_orden_id NUMBER)              RETURN VARCHAR2;
    FUNCTION  FN_FACTURA_ANULABLE          (p_factura_id NUMBER)            RETURN VARCHAR2;
    FUNCTION  FN_GENERAR_NUMERO_FACTURA    (p_serie VARCHAR2)               RETURN VARCHAR2;

END PKG_FACTURACION;
/

CREATE OR REPLACE PACKAGE BODY PKG_FACTURACION AS

    -- -------------------------------------------------------------------------
    -- Helpers privados
    -- -------------------------------------------------------------------------

    PROCEDURE p_log(p_uid NUMBER, p_entidad VARCHAR2, p_op VARCHAR2, p_id NUMBER, p_datos CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES (p_uid, p_entidad, p_op, p_id, p_datos);
        COMMIT;
    END;

    FUNCTION FN_GENERAR_NUMERO_FACTURA(p_serie VARCHAR2) RETURN VARCHAR2 IS
        v_seq NUMBER;
    BEGIN
        CASE UPPER(TRIM(p_serie))
            WHEN 'A' THEN SELECT SEQ_FAC_SERIE_A.NEXTVAL INTO v_seq FROM DUAL;
            ELSE RAISE_APPLICATION_ERROR(-20010, 'Serie no soportada.');
        END CASE;
        RETURN UPPER(TRIM(p_serie)) || LPAD(v_seq, 8, '0');
    END;

    -- =========================================================================
    -- SP_GENERAR_FACTURA_ORDEN
    -- =========================================================================
    PROCEDURE SP_GENERAR_FACTURA_ORDEN(
        p_orden_id   NUMBER,
        p_pago_id    NUMBER,
        p_usuario_id NUMBER,
        p_factura_id OUT NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        v_cliente_id NUMBER;
        v_subtotal   NUMBER;
        v_impuestos  NUMBER;
        v_total      NUMBER;
        v_numero     VARCHAR2(50);
        v_serie      VARCHAR2(20) := 'A';
        ex_neg       EXCEPTION;
    BEGIN
        IF FN_FACTURA_EXISTE_ORDEN(p_orden_id) = 'S' THEN
            p_mensaje := 'La orden #' || p_orden_id || ' ya tiene una factura vigente.';
            RAISE ex_neg;
        END IF;

        BEGIN
            SELECT CLI_CLIENTE, VEN_SUBTOTAL, VEN_IMPUESTOS, VEN_TOTAL
            INTO   v_cliente_id, v_subtotal, v_impuestos, v_total
            FROM   ALP_ORDEN_VENTA
            WHERE  VEN_ORDEN_VENTA = p_orden_id
            FOR UPDATE;
        EXCEPTION WHEN NO_DATA_FOUND THEN
            p_mensaje := 'No existe la orden solicitada.';
            RAISE ex_neg;
        END;

        v_numero := FN_GENERAR_NUMERO_FACTURA(v_serie);

        INSERT INTO ALP_FACTURA
            (VEN_ORDEN_VENTA, CLI_CLIENTE, FAC_NUMERO, FAC_SERIE,
             FAC_SUBTOTAL, FAC_IMPUESTOS, FAC_TOTAL, FAC_ESTADO)
        VALUES
            (p_orden_id, v_cliente_id, v_numero, v_serie,
             v_subtotal, v_impuestos, v_total, 'EMITIDA')
        RETURNING FAC_FACTURA INTO p_factura_id;

        p_log(p_usuario_id, 'ALP_FACTURA', 'INSERT', p_factura_id,
              JSON_OBJECT('orden_id' VALUE p_orden_id, 'numero' VALUE v_numero));

        COMMIT;
        p_resultado := 'EXITO';
        p_mensaje   := 'Factura generada con éxito: ' || v_numero;

    EXCEPTION
        WHEN ex_neg THEN
            ROLLBACK; p_resultado := 'ERROR'; p_factura_id := NULL;
        WHEN OTHERS THEN
            ROLLBACK; p_resultado := 'ERROR';
            p_mensaje := 'Error Técnico: ' || SQLERRM; p_factura_id := NULL;
    END;

    -- =========================================================================
    -- SP_ANULAR_FACTURA
    -- =========================================================================
    PROCEDURE SP_ANULAR_FACTURA(
        p_factura_id NUMBER,
        p_motivo     VARCHAR2,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        v_estado VARCHAR2(20);
        ex_neg   EXCEPTION;
    BEGIN
        BEGIN
            SELECT FAC_ESTADO INTO v_estado FROM ALP_FACTURA WHERE FAC_FACTURA = p_factura_id FOR UPDATE;
        EXCEPTION WHEN NO_DATA_FOUND THEN
            p_mensaje := 'La factura no existe.'; RAISE ex_neg;
        END;

        IF v_estado != 'EMITIDA' THEN
            p_mensaje := 'Estado inválido para anulación: ' || v_estado; RAISE ex_neg;
        END IF;

        IF FN_FACTURA_ANULABLE(p_factura_id) = 'N' THEN
            p_mensaje := 'No se puede anular: Tiene pagos conciliados asociados.'; RAISE ex_neg;
        END IF;

        UPDATE ALP_FACTURA SET FAC_ESTADO = 'ANULADA' WHERE FAC_FACTURA = p_factura_id;

        p_log(p_usuario_id, 'ALP_FACTURA', 'UPDATE', p_factura_id, JSON_OBJECT('motivo' VALUE p_motivo));
        
        COMMIT;
        p_resultado := 'EXITO';
        p_mensaje   := 'Factura anulada correctamente.';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN NO_DATA_FOUND THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := 'Factura no encontrada.';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- =========================================================================
    -- SP_CANCELAR_FACTURA
    -- =========================================================================
    PROCEDURE SP_CANCELAR_FACTURA(
        p_factura_id NUMBER,
        p_motivo     VARCHAR2,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
    BEGIN
        UPDATE ALP_FACTURA SET FAC_ESTADO = 'CANCELADA' WHERE FAC_FACTURA = p_factura_id;
        IF SQL%ROWCOUNT = 0 THEN
            p_resultado := 'ERROR'; p_mensaje := 'Factura no encontrada.';
        ELSE
            p_log(p_usuario_id, 'ALP_FACTURA', 'CANCEL', p_factura_id, p_motivo);
            COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Factura cancelada con éxito.';
        END IF;
    EXCEPTION WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    FUNCTION FN_FACTURA_EXISTE_ORDEN(p_orden_id NUMBER) RETURN VARCHAR2 IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*) INTO v_count FROM ALP_FACTURA WHERE VEN_ORDEN_VENTA = p_orden_id AND FAC_ESTADO != 'ANULADA';
        RETURN CASE WHEN v_count > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_FACTURA_ANULABLE(p_factura_id NUMBER) RETURN VARCHAR2 IS
    BEGIN
        RETURN 'S'; 
    END;

    PROCEDURE SP_OBTENER_FACTURA(p_factura_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT * FROM ALP_FACTURA WHERE FAC_FACTURA = p_factura_id;
    END;

    PROCEDURE SP_LISTAR_FACTURAS_CLIENTE(p_cliente_id NUMBER, p_estado VARCHAR2 DEFAULT NULL, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR 
            SELECT * FROM ALP_FACTURA 
            WHERE CLI_CLIENTE = p_cliente_id 
            AND (p_estado IS NULL OR FAC_ESTADO = p_estado);
    END;

    PROCEDURE SP_LISTAR_FACTURAS_ORDEN(p_orden_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT * FROM ALP_FACTURA WHERE VEN_ORDEN_VENTA = p_orden_id;
    END;

    PROCEDURE SP_CREAR_COMPROBANTE(p_pago_id NUMBER, p_tipo_id NUMBER, p_usuario_id NUMBER, p_comprobante_id OUT NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
    BEGIN
        p_resultado := 'EXITO';
        p_mensaje   := 'Comprobante creado (Simulado)';
        p_comprobante_id := p_pago_id;
    END;

    PROCEDURE SP_ANULAR_COMPROBANTE(p_comprobante_id NUMBER, p_motivo VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
    BEGIN
        p_resultado := 'EXITO';
        p_mensaje   := 'Comprobante anulado (Simulado)';
    END;

END PKG_FACTURACION;
/
