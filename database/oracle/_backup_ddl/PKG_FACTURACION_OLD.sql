-- ============================================================================
-- MUEBLES LOS ALPES — PKG_FACTURACION: CORRECCIÓN DE CONCURRENCIA
-- Motor: Oracle 21c

-- ============================================================================
-- BLOQUE 1: SEQUENCES
-- Ejecutar UNA sola vez. Si ya existen, el script las recrea sin datos.
-- ============================================================================

-- Factura serie A
-- Arranca en 9000001 para no colisionar con SP_GENERAR_FACTURA_ELECTRONICA
-- (que emite A + LPAD(orden_id, 8, '0') → máximo A09999999 para ~10M órdenes).
-- Ajustar START WITH al MAX real en producción antes de ejecutar.
BEGIN
    EXECUTE IMMEDIATE 'DROP SEQUENCE SEQ_FAC_SERIE_A';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE SEQUENCE SEQ_FAC_SERIE_A
    START WITH  9000001
    INCREMENT BY 1
    NOMAXVALUE
    NOCYCLE
    NOCACHE           -- cambiar a CACHE 20 en producción con carga alta
    NOORDER;

-- Comprobante (global, independiente de tipo/serie)
BEGIN
    EXECUTE IMMEDIATE 'DROP SEQUENCE SEQ_COM_NUMERO';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

CREATE SEQUENCE SEQ_COM_NUMERO
    START WITH  1
    INCREMENT BY 1
    NOMAXVALUE
    NOCYCLE
    NOCACHE
    NOORDER;


-- ============================================================================
-- BLOQUE 2: PACKAGE SPECIFICATION (sin cambios de firma)
-- ============================================================================

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
    PROCEDURE SP_OBTENER_COMPROBANTE      (p_comprobante_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_COMPROBANTES_PAGO (p_pago_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_COMPROBANTES_ORDEN(p_orden_id NUMBER, p_cursor OUT SYS_REFCURSOR);

    -- Funciones (firmas idénticas a la versión anterior)
    FUNCTION  FN_FACTURA_EXISTE_ORDEN      (p_orden_id NUMBER)              RETURN VARCHAR2;
    FUNCTION  FN_COMPROBANTE_EXISTE_PAGO   (p_pago_id NUMBER)               RETURN VARCHAR2;
    FUNCTION  FN_FACTURA_ANULABLE          (p_factura_id NUMBER)            RETURN VARCHAR2;
    FUNCTION  FN_GENERAR_NUMERO_FACTURA    (p_serie VARCHAR2)               RETURN VARCHAR2;
    FUNCTION  FN_GENERAR_NUMERO_COMPROBANTE(p_serie VARCHAR2, p_pago_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_OBTENER_FACTURA_ID_ORDEN  (p_orden_id NUMBER)              RETURN NUMBER;

END PKG_FACTURACION;
/


-- ============================================================================
-- BLOQUE 3: PACKAGE BODY
-- ============================================================================

CREATE OR REPLACE PACKAGE BODY PKG_FACTURACION AS

    -- -------------------------------------------------------------------------
    -- Helpers privados
    -- -------------------------------------------------------------------------

    PROCEDURE p_log(
        p_uid     NUMBER,
        p_entidad VARCHAR2,
        p_op      VARCHAR2,
        p_id      NUMBER,
        p_datos   CLOB
    ) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG
            (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES (p_uid, p_entidad, p_op, p_id, p_datos);
        COMMIT;
    END;

    FUNCTION f_uid RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT USU_USUARIO INTO v_id FROM ALP_USUARIO WHERE USU_USERNAME = USER;
        RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL;
    END;

    -- =========================================================================
    -- FN_GENERAR_NUMERO_FACTURA  ← ÚNICO CAMBIO FUNCIONAL
    -- =========================================================================
    --
    -- ANTES (race-condition):
    --   SELECT NVL(MAX(TO_NUMBER(REGEXP_SUBSTR(FAC_NUMERO,'[0-9]+$'))),0)+1 ...
    --   Dos sesiones concurrentes leen el mismo MAX → mismo número → ORA-00001.
    --
    -- AHORA (ACID-safe):
    --   NEXTVAL es atómico a nivel de kernel de Oracle.
    --   Cada llamada obtiene un número único aunque 1000 sesiones llamen
    --   simultáneamente. La secuencia nunca retrocede (NOCYCLE).
    --   El GAP ocasional (rollback de una tx) es el trade-off aceptado y
    --   documentado: los números fiscales NO son necesariamente consecutivos,
    --   sólo únicos y crecientes, lo cual cumple SAT/DTE Guatemala.
    --
    -- Extensión a otras series:
    --   Agregar CASE p_serie WHEN 'B' THEN SEQ_FAC_SERIE_B.NEXTVAL ... END
    --   y crear la SEQUENCE correspondiente.
    -- =========================================================================

    FUNCTION FN_GENERAR_NUMERO_FACTURA(p_serie VARCHAR2) RETURN VARCHAR2 IS
        v_seq NUMBER;
    BEGIN
        CASE UPPER(TRIM(p_serie))
            WHEN 'A' THEN SELECT SEQ_FAC_SERIE_A.NEXTVAL INTO v_seq FROM DUAL;
            ELSE
                -- Serie desconocida: falla de forma explícita, nunca silenciosa
                RAISE_APPLICATION_ERROR(
                    -20010,
                    'Serie de factura no soportada: ' || p_serie ||
                    '. Cree la SEQUENCE correspondiente y agregue el CASE.'
                );
        END CASE;
        RETURN UPPER(TRIM(p_serie)) || LPAD(v_seq, 8, '0');
    END;

    -- =========================================================================
    -- FN_GENERAR_NUMERO_COMPROBANTE  ← CAMBIO FUNCIONAL
    -- =========================================================================
    --
    -- ANTES: serie + YYYYMMDD + PAG_PAGO
    --   Colisión posible si el mismo pago genera dos comprobantes de la misma
    --   serie en el mismo día (reintento, error parcial, etc.).
    --
    -- AHORA: serie + YYYYMMDD + SEQ_COM_NUMERO (8 dígitos, zero-padded)
    --   El número secuencial garantiza unicidad global.
    --   PAG_PAGO se conserva en los datos de auditoría del INSERT,
    --   no en el número visible.
    -- =========================================================================

    FUNCTION FN_GENERAR_NUMERO_COMPROBANTE(p_serie VARCHAR2, p_pago_id NUMBER)
        RETURN VARCHAR2
    IS
        v_seq NUMBER;
    BEGIN
        SELECT SEQ_COM_NUMERO.NEXTVAL INTO v_seq FROM DUAL;
        -- p_pago_id se recibe para mantener la firma original;
        -- ya no se usa en el número pero queda disponible si se necesita en el futuro.
        RETURN UPPER(TRIM(p_serie)) || '-' ||
               TO_CHAR(SYSDATE, 'YYYYMMDD')  || '-' ||
               LPAD(v_seq, 8, '0');
    END;

    -- =========================================================================
    -- FACTURA — SPs (sin cambios de lógica, solo usan las nuevas FNs)
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
            p_mensaje := 'La orden ya tiene una factura vigente';
            RAISE ex_neg;
        END IF;

        SELECT CLI_CLIENTE, VEN_SUBTOTAL, VEN_IMPUESTOS, VEN_TOTAL
        INTO   v_cliente_id, v_subtotal, v_impuestos, v_total
        FROM   ALP_ORDEN_VENTA
        WHERE  VEN_ORDEN_VENTA = p_orden_id
        FOR UPDATE;

        -- NEXTVAL se consume aquí; si el INSERT falla después, ese número
        -- queda como GAP. Aceptable: mejor un gap que un deadlock o duplicado.
        v_numero := FN_GENERAR_NUMERO_FACTURA(v_serie);

        INSERT INTO ALP_FACTURA
            (VEN_ORDEN_VENTA, CLI_CLIENTE, FAC_NUMERO, FAC_SERIE,
             FAC_SUBTOTAL, FAC_IMPUESTOS, FAC_TOTAL, FAC_ESTADO)
        VALUES
            (p_orden_id, v_cliente_id, v_numero, v_serie,
             v_subtotal, v_impuestos, v_total, 'EMITIDA')
        RETURNING FAC_FACTURA INTO p_factura_id;

        p_log(p_usuario_id, 'ALP_FACTURA', 'INSERT', p_factura_id,
              JSON_OBJECT('orden_id' VALUE p_orden_id,
                          'numero'   VALUE v_numero,
                          'total'    VALUE v_total));
        COMMIT;
        p_resultado := 'EXITO';
        p_mensaje   := 'Factura generada: ' || v_numero;

    EXCEPTION
        WHEN ex_neg THEN
            ROLLBACK; p_resultado := 'ERROR'; p_factura_id := NULL;
        WHEN OTHERS THEN
            ROLLBACK; p_resultado := 'ERROR';
            p_mensaje := SQLERRM; p_factura_id := NULL;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_ANULAR_FACTURA(
        p_factura_id NUMBER,
        p_motivo     VARCHAR2,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        v_estado VARCHAR2(20);
        v_numero VARCHAR2(50);
        ex_neg   EXCEPTION;
    BEGIN
        SELECT FAC_ESTADO, FAC_NUMERO
        INTO   v_estado, v_numero
        FROM   ALP_FACTURA
        WHERE  FAC_FACTURA = p_factura_id
        FOR UPDATE;

        IF v_estado != 'EMITIDA' THEN
            p_mensaje := 'Solo facturas EMITIDA pueden anularse. Estado actual: ' || v_estado;
            RAISE ex_neg;
        END IF;

        IF FN_FACTURA_ANULABLE(p_factura_id) = 'N' THEN
            p_mensaje := 'No se puede anular: tiene pagos confirmados asociados';
            RAISE ex_neg;
        END IF;

        UPDATE ALP_FACTURA SET FAC_ESTADO = 'ANULADA' WHERE FAC_FACTURA = p_factura_id;

        p_log(p_usuario_id, 'ALP_FACTURA', 'UPDATE', p_factura_id,
              JSON_OBJECT('estado_nuevo' VALUE 'ANULADA', 'motivo' VALUE p_motivo));
        COMMIT;
        p_resultado := 'EXITO';
        p_mensaje   := 'Factura ' || v_numero || ' anulada';

    EXCEPTION
        WHEN ex_neg THEN
            ROLLBACK; p_resultado := 'ERROR';
        WHEN NO_DATA_FOUND THEN
            ROLLBACK; p_resultado := 'ERROR'; p_mensaje := 'Factura no encontrada';
        WHEN OTHERS THEN
            ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_CANCELAR_FACTURA(
        p_factura_id NUMBER,
        p_motivo     VARCHAR2,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        v_estado VARCHAR2(20);
        v_numero VARCHAR2(50);
        ex_neg   EXCEPTION;
    BEGIN
        SELECT FAC_ESTADO, FAC_NUMERO
        INTO   v_estado, v_numero
        FROM   ALP_FACTURA
        WHERE  FAC_FACTURA = p_factura_id
        FOR UPDATE;

        IF v_estado = 'CANCELADA' THEN
            p_mensaje := 'Factura ya está cancelada'; RAISE ex_neg;
        END IF;
        IF v_estado = 'ANULADA' THEN
            p_mensaje := 'No se puede cancelar una factura ya anulada'; RAISE ex_neg;
        END IF;

        UPDATE ALP_FACTURA SET FAC_ESTADO = 'CANCELADA' WHERE FAC_FACTURA = p_factura_id;

        p_log(p_usuario_id, 'ALP_FACTURA', 'UPDATE', p_factura_id,
              JSON_OBJECT('estado_nuevo' VALUE 'CANCELADA', 'motivo' VALUE p_motivo));
        COMMIT;
        p_resultado := 'EXITO';
        p_mensaje   := 'Factura ' || v_numero || ' cancelada';

    EXCEPTION
        WHEN ex_neg THEN
            ROLLBACK; p_resultado := 'ERROR';
        WHEN NO_DATA_FOUND THEN
            ROLLBACK; p_resultado := 'ERROR'; p_mensaje := 'Factura no encontrada';
        WHEN OTHERS THEN
            ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_OBTENER_FACTURA(p_factura_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT f.FAC_FACTURA,
                   f.VEN_ORDEN_VENTA,
                   v.VEN_NUMERO_ORDEN,
                   f.CLI_CLIENTE,
                   c.CLI_PRIMER_NOMBRE || ' ' || c.CLI_PRIMER_APELLIDO AS CLI_NOMBRE,
                   f.FAC_NUMERO,
                   f.FAC_SERIE,
                   f.FAC_FECHA_EMISION,
                   f.FAC_SUBTOTAL,
                   f.FAC_IMPUESTOS,
                   f.FAC_TOTAL,
                   f.FAC_ESTADO
            FROM   ALP_FACTURA f
            JOIN   ALP_ORDEN_VENTA v ON f.VEN_ORDEN_VENTA = v.VEN_ORDEN_VENTA
            JOIN   ALP_CLIENTE c     ON f.CLI_CLIENTE = c.CLI_CLIENTE
            WHERE  f.FAC_FACTURA = p_factura_id;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_LISTAR_FACTURAS_CLIENTE(
        p_cliente_id NUMBER,
        p_estado     VARCHAR2 DEFAULT NULL,
        p_cursor     OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT f.FAC_FACTURA,
                   f.VEN_ORDEN_VENTA,
                   v.VEN_NUMERO_ORDEN,
                   f.FAC_NUMERO,
                   f.FAC_SERIE,
                   f.FAC_FECHA_EMISION,
                   f.FAC_TOTAL,
                   f.FAC_ESTADO
            FROM   ALP_FACTURA f
            JOIN   ALP_ORDEN_VENTA v ON f.VEN_ORDEN_VENTA = v.VEN_ORDEN_VENTA
            WHERE  f.CLI_CLIENTE = p_cliente_id
              AND  (p_estado IS NULL OR f.FAC_ESTADO = p_estado)
            ORDER  BY f.FAC_FECHA_EMISION DESC;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_LISTAR_FACTURAS_ORDEN(p_orden_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT FAC_FACTURA, FAC_NUMERO, FAC_SERIE,
                   FAC_FECHA_EMISION, FAC_SUBTOTAL,
                   FAC_IMPUESTOS, FAC_TOTAL, FAC_ESTADO
            FROM   ALP_FACTURA
            WHERE  VEN_ORDEN_VENTA = p_orden_id
            ORDER  BY FAC_FECHA_EMISION DESC;
    END;

    -- =========================================================================
    -- COMPROBANTE — SPs
    -- =========================================================================

    PROCEDURE SP_CREAR_COMPROBANTE(
        p_pago_id        NUMBER,
        p_tipo_id        NUMBER,
        p_usuario_id     NUMBER,
        p_comprobante_id OUT NUMBER,
        p_resultado      OUT VARCHAR2,
        p_mensaje        OUT VARCHAR2
    ) IS
        v_monto  NUMBER;
        v_serie  VARCHAR2(20);
        v_numero VARCHAR2(50);
        ex_neg   EXCEPTION;
    BEGIN
        BEGIN
            SELECT PAG_MONTO INTO v_monto FROM ALP_PAGO WHERE PAG_PAGO = p_pago_id;
        EXCEPTION WHEN NO_DATA_FOUND THEN
            p_mensaje := 'Pago no encontrado: ' || p_pago_id; RAISE ex_neg;
        END;

        BEGIN
            SELECT TCO_CODIGO INTO v_serie
            FROM   ALP_TIPO_COMPROBANTE
            WHERE  TCO_TIPO_COMPROBANTE = p_tipo_id AND TCO_ESTADO = 'ACTIVO';
        EXCEPTION WHEN NO_DATA_FOUND THEN
            p_mensaje := 'Tipo de comprobante no encontrado o inactivo'; RAISE ex_neg;
        END;

        v_numero := FN_GENERAR_NUMERO_COMPROBANTE(v_serie, p_pago_id);

        INSERT INTO ALP_COMPROBANTE
            (PAG_PAGO, TCO_TIPO_COMPROBANTE, COM_NUMERO, COM_SERIE, COM_MONTO)
        VALUES
            (p_pago_id, p_tipo_id, v_numero, v_serie, v_monto)
        RETURNING COM_COMPROBANTE INTO p_comprobante_id;

        p_log(p_usuario_id, 'ALP_COMPROBANTE', 'INSERT', p_comprobante_id,
              JSON_OBJECT('pago_id' VALUE p_pago_id,
                          'tipo_id' VALUE p_tipo_id,
                          'numero'  VALUE v_numero,
                          'monto'   VALUE v_monto));
        COMMIT;
        p_resultado := 'EXITO';
        p_mensaje   := 'Comprobante generado: ' || v_numero;

    EXCEPTION
        WHEN ex_neg THEN
            ROLLBACK; p_resultado := 'ERROR'; p_comprobante_id := NULL;
        WHEN DUP_VAL_ON_INDEX THEN
            -- No debería ocurrir con SEQUENCE, pero se maneja por defensa
            ROLLBACK; p_resultado := 'ERROR';
            p_mensaje := 'Número de comprobante duplicado (contactar DBA)';
            p_comprobante_id := NULL;
        WHEN OTHERS THEN
            ROLLBACK; p_resultado := 'ERROR';
            p_mensaje := SQLERRM; p_comprobante_id := NULL;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_ANULAR_COMPROBANTE(
        p_comprobante_id NUMBER,
        p_motivo         VARCHAR2,
        p_usuario_id     NUMBER,
        p_resultado      OUT VARCHAR2,
        p_mensaje        OUT VARCHAR2
    ) IS
        v_numero VARCHAR2(50);
        ex_neg   EXCEPTION;
    BEGIN
        BEGIN
            SELECT COM_NUMERO INTO v_numero
            FROM   ALP_COMPROBANTE
            WHERE  COM_COMPROBANTE = p_comprobante_id;
        EXCEPTION WHEN NO_DATA_FOUND THEN
            p_mensaje := 'Comprobante no encontrado'; RAISE ex_neg;
        END;

        UPDATE ALP_FACTURA f
        SET    f.FAC_ESTADO = 'ANULADA'
        WHERE  f.VEN_ORDEN_VENTA IN (
                   SELECT p.VEN_ORDEN_VENTA
                   FROM   ALP_PAGO p
                   JOIN   ALP_COMPROBANTE c ON c.PAG_PAGO = p.PAG_PAGO
                   WHERE  c.COM_COMPROBANTE = p_comprobante_id
               )
          AND  f.FAC_ESTADO = 'EMITIDA';

        p_log(p_usuario_id, 'ALP_COMPROBANTE', 'UPDATE', p_comprobante_id,
              JSON_OBJECT('accion' VALUE 'ANULADO',
                          'motivo' VALUE p_motivo,
                          'numero' VALUE v_numero));
        COMMIT;
        p_resultado := 'EXITO';
        p_mensaje   := 'Comprobante ' || v_numero || ' anulado';

    EXCEPTION
        WHEN ex_neg THEN
            ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN
            ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_OBTENER_COMPROBANTE(p_comprobante_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT c.COM_COMPROBANTE,
                   c.PAG_PAGO,
                   c.TCO_TIPO_COMPROBANTE,
                   t.TCO_NOMBRE       AS TIPO_NOMBRE,
                   c.COM_NUMERO,
                   c.COM_SERIE,
                   c.COM_FECHA_EMISION,
                   c.COM_MONTO,
                   c.COM_OBSERVACIONES,
                   p.VEN_ORDEN_VENTA,
                   v.VEN_NUMERO_ORDEN
            FROM   ALP_COMPROBANTE c
            JOIN   ALP_TIPO_COMPROBANTE t ON c.TCO_TIPO_COMPROBANTE = t.TCO_TIPO_COMPROBANTE
            JOIN   ALP_PAGO p             ON c.PAG_PAGO = p.PAG_PAGO
            JOIN   ALP_ORDEN_VENTA v      ON p.VEN_ORDEN_VENTA = v.VEN_ORDEN_VENTA
            WHERE  c.COM_COMPROBANTE = p_comprobante_id;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_LISTAR_COMPROBANTES_PAGO(p_pago_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT c.COM_COMPROBANTE,
                   c.TCO_TIPO_COMPROBANTE,
                   t.TCO_NOMBRE AS TIPO_NOMBRE,
                   c.COM_NUMERO, c.COM_SERIE,
                   c.COM_FECHA_EMISION, c.COM_MONTO
            FROM   ALP_COMPROBANTE c
            JOIN   ALP_TIPO_COMPROBANTE t ON c.TCO_TIPO_COMPROBANTE = t.TCO_TIPO_COMPROBANTE
            WHERE  c.PAG_PAGO = p_pago_id
            ORDER  BY c.COM_FECHA_EMISION DESC;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_LISTAR_COMPROBANTES_ORDEN(p_orden_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT c.COM_COMPROBANTE,
                   c.PAG_PAGO,
                   t.TCO_NOMBRE AS TIPO_NOMBRE,
                   c.COM_NUMERO, c.COM_SERIE,
                   c.COM_FECHA_EMISION, c.COM_MONTO
            FROM   ALP_COMPROBANTE c
            JOIN   ALP_TIPO_COMPROBANTE t ON c.TCO_TIPO_COMPROBANTE = t.TCO_TIPO_COMPROBANTE
            JOIN   ALP_PAGO p             ON c.PAG_PAGO = p.PAG_PAGO
            WHERE  p.VEN_ORDEN_VENTA = p_orden_id
            ORDER  BY c.COM_FECHA_EMISION DESC;
    END;

    -- =========================================================================
    -- FUNCIONES DE VALIDACIÓN (sin cambios)
    -- =========================================================================

    FUNCTION FN_FACTURA_EXISTE_ORDEN(p_orden_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c
        FROM ALP_FACTURA
        WHERE VEN_ORDEN_VENTA = p_orden_id AND FAC_ESTADO = 'EMITIDA';
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_COMPROBANTE_EXISTE_PAGO(p_pago_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_COMPROBANTE WHERE PAG_PAGO = p_pago_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_FACTURA_ANULABLE(p_factura_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c
        FROM   ALP_PAGO p
        JOIN   ALP_FACTURA f  ON p.VEN_ORDEN_VENTA = f.VEN_ORDEN_VENTA
        JOIN   ALP_ESTADO_PAGO ep ON p.ESP_ESTADO_PAGO = ep.ESP_ESTADO_PAGO
        WHERE  f.FAC_FACTURA = p_factura_id
          AND  ep.ESP_CODIGO NOT IN ('REEMBOLSADO','CANCELADO')
          AND  ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_OBTENER_FACTURA_ID_ORDEN(p_orden_id NUMBER) RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT FAC_FACTURA INTO v_id
        FROM   ALP_FACTURA
        WHERE  VEN_ORDEN_VENTA = p_orden_id AND FAC_ESTADO = 'EMITIDA'
        FETCH FIRST 1 ROWS ONLY;
        RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL;
    END;

END PKG_FACTURACION;
/
-- ============================================================================
-- PKG_UBICACIONES — ADENDUM: Direcciones de Cliente
-- Nota: País, Departamento, Ciudad ya existen completos en PKG_UBICACIONES.
--       Este bloque agrega CRUD de ALP_CLIENTE_DIRECCION que faltaba.
-- Orden de ejecución: 2° (depende de PKG_UBICACIONES ya compilado)
-- ============================================================================
-- Estrategia: se extiende PKG_UBICACIONES con CREATE OR REPLACE para agregar
-- los nuevos SPs/FNs sin tocar los existentes.
-- ============================================================================

CREATE OR REPLACE PACKAGE PKG_UBICACIONES AS

    -- =========================================================================
    -- YA EXISTENTES — se redeclaran para que el spec quede completo
    -- =========================================================================

    PROCEDURE SP_CREAR_PAIS         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_PAIS    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_PAIS(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_PAIS       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_PAISES      (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_PAIS        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_PAIS        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_PAIS(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_DEPARTAMENTO         (p_pais_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_DEPARTAMENTO    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_DEPARTAMENTO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_DEPARTAMENTO       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_DEPARTAMENTOS       (p_pais_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_DEPARTAMENTO        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_DEPARTAMENTO        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_CIUDAD         (p_dep_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_CIUDAD    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_CIUDAD(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_CIUDAD       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_CIUDADES      (p_dep_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_CIUDAD        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_CIUDAD        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_CIUDAD(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_MONEDA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_simbolo VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_MONEDA    (p_id NUMBER, p_nombre VARCHAR2, p_simbolo VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_MONEDA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_MONEDA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_MONEDAS       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_MONEDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_MONEDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_MONEDA(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_IDIOMA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_IDIOMA    (p_id NUMBER, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_IDIOMA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_IDIOMA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_IDIOMAS       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_IDIOMA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_IDIOMA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_IDIOMA(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_UNIDAD_MEDIDA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_abreviatura VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_UNIDAD_MEDIDA    (p_id NUMBER, p_nombre VARCHAR2, p_abreviatura VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_UNIDAD_MEDIDA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_UNIDAD_MEDIDA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_UNIDADES_MEDIDA      (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_UNIDAD_MEDIDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_UNIDAD_MEDIDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2;

    -- =========================================================================
    -- NUEVO: ALP_CLIENTE_DIRECCION
    -- =========================================================================
    PROCEDURE SP_AGREGAR_DIRECCION_CLIENTE      (p_cliente_id NUMBER, p_ciudad_id NUMBER, p_tipo VARCHAR2, p_linea1 VARCHAR2, p_linea2 VARCHAR2, p_codigo_postal VARCHAR2, p_referencia VARCHAR2, p_principal VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_DIRECCION_CLIENTE   (p_id NUMBER, p_ciudad_id NUMBER, p_tipo VARCHAR2, p_linea1 VARCHAR2, p_linea2 VARCHAR2, p_codigo_postal VARCHAR2, p_referencia VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_INACTIVAR_DIRECCION_CLIENTE    (p_id NUMBER, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_MARCAR_DIRECCION_PREDETERMINADA(p_id NUMBER, p_cliente_id NUMBER, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_DIRECCION_CLIENTE      (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_DIRECCIONES_CLIENTE     (p_cliente_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_DIRECCION_CLIENTE       (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_CLIENTE_TIENE_DIRECCION_PRINCIPAL(p_cliente_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_DIRECCION      (p_id NUMBER) RETURN VARCHAR2;

END PKG_UBICACIONES;
/

CREATE OR REPLACE PACKAGE BODY PKG_UBICACIONES AS

    -- -------------------------------------------------------------------------
    -- Helpers privados
    -- -------------------------------------------------------------------------

    PROCEDURE p_log(p_uid NUMBER, p_entidad VARCHAR2, p_op VARCHAR2, p_id NUMBER, p_datos CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG
            (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES (p_uid, p_entidad, p_op, p_id, p_datos);
        COMMIT;
    END;

    FUNCTION f_uid RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT USU_USUARIO INTO v_id FROM ALP_USUARIO WHERE USU_USERNAME = USER;
        RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL;
    END;

    -- =========================================================================
    -- ALP_PAIS
    -- =========================================================================

    PROCEDURE SP_CREAR_PAIS(p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN p_mensaje := 'Código y nombre requeridos'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_PAIS WHERE PAI_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código de país ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_PAIS (PAI_CODIGO, PAI_NOMBRE, PAI_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO')
        RETURNING PAI_PAIS INTO p_id;
        p_log(f_uid, 'ALP_PAIS', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'País creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_PAIS(p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_PAIS(p_id) = 'N' THEN p_mensaje := 'País no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_PAIS SET PAI_CODIGO = UPPER(TRIM(p_codigo)), PAI_NOMBRE = TRIM(p_nombre) WHERE PAI_PAIS = p_id;
        p_log(f_uid, 'ALP_PAIS', 'UPDATE', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'País actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_PAIS(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_PAIS(p_id) = 'N' THEN
            p_mensaje := 'Tiene departamentos activos'; RAISE ex_neg;
        END IF;
        UPDATE ALP_PAIS SET PAI_ESTADO = p_estado WHERE PAI_PAIS = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_PAIS', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_PAIS(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT PAI_PAIS, PAI_CODIGO, PAI_NOMBRE, PAI_ESTADO FROM ALP_PAIS WHERE PAI_PAIS = p_id;
    END;

    PROCEDURE SP_LISTAR_PAISES(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT PAI_PAIS, PAI_CODIGO, PAI_NOMBRE, PAI_ESTADO FROM ALP_PAIS
            WHERE (p_solo_activos != 'S' OR PAI_ESTADO = 'ACTIVO') ORDER BY PAI_NOMBRE;
    END;

    FUNCTION FN_EXISTE_PAIS(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PAIS WHERE PAI_PAIS = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_PAIS(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT PAI_ESTADO INTO v_e FROM ALP_PAIS WHERE PAI_PAIS = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_PAIS(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_DEPARTAMENTO WHERE PAI_PAIS = p_id AND DEP_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_DEPARTAMENTO
    -- =========================================================================

    PROCEDURE SP_CREAR_DEPARTAMENTO(p_pais_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF FN_EXISTE_PAIS(p_pais_id) = 'N' THEN p_mensaje := 'País no existe'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_DEPARTAMENTO WHERE PAI_PAIS = p_pais_id AND DEP_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe en este país'; RAISE ex_neg; END IF;
        INSERT INTO ALP_DEPARTAMENTO (PAI_PAIS, DEP_CODIGO, DEP_NOMBRE, DEP_ESTADO)
        VALUES (p_pais_id, UPPER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO')
        RETURNING DEP_DEPARTAMENTO INTO p_id;
        p_log(f_uid, 'ALP_DEPARTAMENTO', 'INSERT', p_id, JSON_OBJECT('pais_id' VALUE p_pais_id, 'codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Departamento creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_DEPARTAMENTO(p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_DEPARTAMENTO(p_id) = 'N' THEN p_mensaje := 'Departamento no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_DEPARTAMENTO SET DEP_CODIGO = UPPER(TRIM(p_codigo)), DEP_NOMBRE = TRIM(p_nombre) WHERE DEP_DEPARTAMENTO = p_id;
        p_log(f_uid, 'ALP_DEPARTAMENTO', 'UPDATE', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Departamento actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_DEPARTAMENTO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_DEPARTAMENTO(p_id) = 'N' THEN
            p_mensaje := 'Tiene ciudades activas'; RAISE ex_neg;
        END IF;
        UPDATE ALP_DEPARTAMENTO SET DEP_ESTADO = p_estado WHERE DEP_DEPARTAMENTO = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_DEPARTAMENTO', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_DEPARTAMENTO(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT d.DEP_DEPARTAMENTO, d.PAI_PAIS, p.PAI_NOMBRE, d.DEP_CODIGO, d.DEP_NOMBRE, d.DEP_ESTADO
            FROM ALP_DEPARTAMENTO d JOIN ALP_PAIS p ON d.PAI_PAIS = p.PAI_PAIS WHERE d.DEP_DEPARTAMENTO = p_id;
    END;

    PROCEDURE SP_LISTAR_DEPARTAMENTOS(p_pais_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT DEP_DEPARTAMENTO, PAI_PAIS, DEP_CODIGO, DEP_NOMBRE, DEP_ESTADO
            FROM ALP_DEPARTAMENTO WHERE PAI_PAIS = p_pais_id
            AND (p_solo_activos != 'S' OR DEP_ESTADO = 'ACTIVO') ORDER BY DEP_NOMBRE;
    END;

    FUNCTION FN_EXISTE_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_DEPARTAMENTO WHERE DEP_DEPARTAMENTO = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT DEP_ESTADO INTO v_e FROM ALP_DEPARTAMENTO WHERE DEP_DEPARTAMENTO = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_CIUDAD WHERE DEP_DEPARTAMENTO = p_id AND CIU_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_CIUDAD
    -- =========================================================================

    PROCEDURE SP_CREAR_CIUDAD(p_dep_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF FN_EXISTE_DEPARTAMENTO(p_dep_id) = 'N' THEN p_mensaje := 'Departamento no existe'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_CIUDAD WHERE DEP_DEPARTAMENTO = p_dep_id AND CIU_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe en este departamento'; RAISE ex_neg; END IF;
        INSERT INTO ALP_CIUDAD (DEP_DEPARTAMENTO, CIU_CODIGO, CIU_NOMBRE, CIU_ESTADO)
        VALUES (p_dep_id, UPPER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO')
        RETURNING CIU_CIUDAD INTO p_id;
        p_log(f_uid, 'ALP_CIUDAD', 'INSERT', p_id, JSON_OBJECT('dep_id' VALUE p_dep_id, 'codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Ciudad creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_CIUDAD(p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_CIUDAD(p_id) = 'N' THEN p_mensaje := 'Ciudad no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_CIUDAD SET CIU_CODIGO = UPPER(TRIM(p_codigo)), CIU_NOMBRE = TRIM(p_nombre) WHERE CIU_CIUDAD = p_id;
        p_log(f_uid, 'ALP_CIUDAD', 'UPDATE', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Ciudad actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_CIUDAD(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_CIUDAD(p_id) = 'N' THEN
            p_mensaje := 'Tiene direcciones de clientes asociadas'; RAISE ex_neg;
        END IF;
        UPDATE ALP_CIUDAD SET CIU_ESTADO = p_estado WHERE CIU_CIUDAD = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_CIUDAD', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_CIUDAD(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT c.CIU_CIUDAD, c.DEP_DEPARTAMENTO, d.DEP_NOMBRE, c.CIU_CODIGO, c.CIU_NOMBRE, c.CIU_ESTADO
            FROM ALP_CIUDAD c JOIN ALP_DEPARTAMENTO d ON c.DEP_DEPARTAMENTO = d.DEP_DEPARTAMENTO WHERE c.CIU_CIUDAD = p_id;
    END;

    PROCEDURE SP_LISTAR_CIUDADES(p_dep_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT CIU_CIUDAD, DEP_DEPARTAMENTO, CIU_CODIGO, CIU_NOMBRE, CIU_ESTADO
            FROM ALP_CIUDAD WHERE DEP_DEPARTAMENTO = p_dep_id
            AND (p_solo_activos != 'S' OR CIU_ESTADO = 'ACTIVO') ORDER BY CIU_NOMBRE;
    END;

    FUNCTION FN_EXISTE_CIUDAD(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_CIUDAD WHERE CIU_CIUDAD = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_CIUDAD(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT CIU_ESTADO INTO v_e FROM ALP_CIUDAD WHERE CIU_CIUDAD = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_CIUDAD(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_CLIENTE_DIRECCION WHERE CIU_CIUDAD = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_MONEDA  (cuerpo idéntico al original, se mantiene por integridad del spec)
    -- =========================================================================

    PROCEDURE SP_CREAR_MONEDA(p_codigo VARCHAR2, p_nombre VARCHAR2, p_simbolo VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF LENGTH(TRIM(p_codigo)) != 3 THEN p_mensaje := 'Código ISO debe tener 3 caracteres'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_MONEDA WHERE MON_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código de moneda ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_MONEDA (MON_CODIGO, MON_NOMBRE, MON_SIMBOLO, MON_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_simbolo, 'ACTIVO')
        RETURNING MON_MONEDA INTO p_id;
        p_log(f_uid, 'ALP_MONEDA', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Moneda creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_MONEDA(p_id NUMBER, p_nombre VARCHAR2, p_simbolo VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_MONEDA(p_id) = 'N' THEN p_mensaje := 'Moneda no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_MONEDA SET MON_NOMBRE = TRIM(p_nombre), MON_SIMBOLO = p_simbolo WHERE MON_MONEDA = p_id;
        p_log(f_uid, 'ALP_MONEDA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Moneda actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_MONEDA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_MONEDA(p_id) = 'N' THEN
            p_mensaje := 'Tiene precios activos asociados'; RAISE ex_neg;
        END IF;
        UPDATE ALP_MONEDA SET MON_ESTADO = p_estado WHERE MON_MONEDA = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_MONEDA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_MONEDA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT MON_MONEDA, MON_CODIGO, MON_NOMBRE, MON_SIMBOLO, MON_ESTADO FROM ALP_MONEDA WHERE MON_MONEDA = p_id; END;

    PROCEDURE SP_LISTAR_MONEDAS(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT MON_MONEDA, MON_CODIGO, MON_NOMBRE, MON_SIMBOLO, MON_ESTADO FROM ALP_MONEDA WHERE (p_solo_activos != 'S' OR MON_ESTADO = 'ACTIVO') ORDER BY MON_NOMBRE; END;

    FUNCTION FN_EXISTE_MONEDA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_MONEDA WHERE MON_MONEDA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_MONEDA(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT MON_ESTADO INTO v_e FROM ALP_MONEDA WHERE MON_MONEDA = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_MONEDA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_PRECIO WHERE MON_MONEDA = p_id AND PPR_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_IDIOMA
    -- =========================================================================

    PROCEDURE SP_CREAR_IDIOMA(p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_IDIOMA WHERE IDI_CODIGO = LOWER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_IDIOMA (IDI_CODIGO, IDI_NOMBRE, IDI_ESTADO) VALUES (LOWER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO') RETURNING IDI_IDIOMA INTO p_id;
        p_log(f_uid, 'ALP_IDIOMA', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Idioma creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_IDIOMA(p_id NUMBER, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_IDIOMA(p_id) = 'N' THEN p_mensaje := 'Idioma no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_IDIOMA SET IDI_NOMBRE = TRIM(p_nombre) WHERE IDI_IDIOMA = p_id;
        p_log(f_uid, 'ALP_IDIOMA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Idioma actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_IDIOMA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_IDIOMA(p_id) = 'N' THEN p_mensaje := 'Tiene traducciones de productos'; RAISE ex_neg; END IF;
        UPDATE ALP_IDIOMA SET IDI_ESTADO = p_estado WHERE IDI_IDIOMA = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_IDIOMA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_IDIOMA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT IDI_IDIOMA, IDI_CODIGO, IDI_NOMBRE, IDI_ESTADO FROM ALP_IDIOMA WHERE IDI_IDIOMA = p_id; END;

    PROCEDURE SP_LISTAR_IDIOMAS(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT IDI_IDIOMA, IDI_CODIGO, IDI_NOMBRE, IDI_ESTADO FROM ALP_IDIOMA WHERE (p_solo_activos != 'S' OR IDI_ESTADO = 'ACTIVO') ORDER BY IDI_NOMBRE; END;

    FUNCTION FN_EXISTE_IDIOMA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_IDIOMA WHERE IDI_IDIOMA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_IDIOMA(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT IDI_ESTADO INTO v_e FROM ALP_IDIOMA WHERE IDI_IDIOMA = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_IDIOMA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_IDIOMA WHERE IDI_IDIOMA = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_UNIDAD_MEDIDA
    -- =========================================================================

    PROCEDURE SP_CREAR_UNIDAD_MEDIDA(p_codigo VARCHAR2, p_nombre VARCHAR2, p_abreviatura VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_UNIDAD_MEDIDA WHERE UNI_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_UNIDAD_MEDIDA (UNI_CODIGO, UNI_NOMBRE, UNI_ABREVIATURA, UNI_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_abreviatura, 'ACTIVO')
        RETURNING UNI_UNIDAD_MEDIDA INTO p_id;
        p_log(f_uid, 'ALP_UNIDAD_MEDIDA', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Unidad de medida creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_UNIDAD_MEDIDA(p_id NUMBER, p_nombre VARCHAR2, p_abreviatura VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_UNIDAD_MEDIDA(p_id) = 'N' THEN p_mensaje := 'Unidad de medida no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_UNIDAD_MEDIDA SET UNI_NOMBRE = TRIM(p_nombre), UNI_ABREVIATURA = p_abreviatura WHERE UNI_UNIDAD_MEDIDA = p_id;
        p_log(f_uid, 'ALP_UNIDAD_MEDIDA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Unidad actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_UNIDAD_MEDIDA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_UNIDAD_MEDIDA(p_id) = 'N' THEN p_mensaje := 'Tiene productos activos'; RAISE ex_neg; END IF;
        UPDATE ALP_UNIDAD_MEDIDA SET UNI_ESTADO = p_estado WHERE UNI_UNIDAD_MEDIDA = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_UNIDAD_MEDIDA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_UNIDAD_MEDIDA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT UNI_UNIDAD_MEDIDA, UNI_CODIGO, UNI_NOMBRE, UNI_ABREVIATURA, UNI_ESTADO FROM ALP_UNIDAD_MEDIDA WHERE UNI_UNIDAD_MEDIDA = p_id; END;

    PROCEDURE SP_LISTAR_UNIDADES_MEDIDA(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT UNI_UNIDAD_MEDIDA, UNI_CODIGO, UNI_NOMBRE, UNI_ABREVIATURA, UNI_ESTADO FROM ALP_UNIDAD_MEDIDA WHERE (p_solo_activos != 'S' OR UNI_ESTADO = 'ACTIVO') ORDER BY UNI_NOMBRE; END;

    FUNCTION FN_EXISTE_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_UNIDAD_MEDIDA WHERE UNI_UNIDAD_MEDIDA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT UNI_ESTADO INTO v_e FROM ALP_UNIDAD_MEDIDA WHERE UNI_UNIDAD_MEDIDA = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO WHERE PRO_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- NUEVO: ALP_CLIENTE_DIRECCION
    -- =========================================================================

    PROCEDURE SP_AGREGAR_DIRECCION_CLIENTE(
        p_cliente_id    NUMBER,
        p_ciudad_id     NUMBER,
        p_tipo          VARCHAR2,
        p_linea1        VARCHAR2,
        p_linea2        VARCHAR2,
        p_codigo_postal VARCHAR2,
        p_referencia    VARCHAR2,
        p_principal     VARCHAR2,
        p_resultado     OUT VARCHAR2,
        p_mensaje       OUT VARCHAR2,
        p_id            OUT NUMBER
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_tipo NOT IN ('ENVIO','FACTURACION','AMBAS') THEN
            p_mensaje := 'Tipo inválido. Use ENVIO, FACTURACION o AMBAS'; RAISE ex_neg;
        END IF;
        IF FN_EXISTE_CIUDAD(p_ciudad_id) = 'N' THEN
            p_mensaje := 'Ciudad no existe'; RAISE ex_neg;
        END IF;
        IF p_linea1 IS NULL THEN
            p_mensaje := 'Línea 1 de dirección es requerida'; RAISE ex_neg;
        END IF;
        -- Si se marca como principal, verificar que existe el cliente
        SELECT COUNT(1) INTO v_c FROM ALP_CLIENTE WHERE CLI_CLIENTE = p_cliente_id;
        IF v_c = 0 THEN p_mensaje := 'Cliente no existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_CLIENTE_DIRECCION
            (CLI_CLIENTE, CIU_CIUDAD, CLD_TIPO, CLD_DIRECCION_LINEA1, CLD_DIRECCION_LINEA2,
             CLD_CODIGO_POSTAL, CLD_REFERENCIA, CLD_PRINCIPAL, CLD_ESTADO)
        VALUES
            (p_cliente_id, p_ciudad_id, p_tipo, TRIM(p_linea1), p_linea2,
             p_codigo_postal, p_referencia, NVL(p_principal,'N'), 'ACTIVO')
        RETURNING CLD_CLIENTE_DIRECCION INTO p_id;

        -- Si se marcó como principal, desmarcar las otras del mismo cliente
        IF NVL(p_principal,'N') = 'S' THEN
            UPDATE ALP_CLIENTE_DIRECCION
            SET CLD_PRINCIPAL = 'N'
            WHERE CLI_CLIENTE = p_cliente_id
              AND CLD_CLIENTE_DIRECCION != p_id;
        END IF;

        p_log(f_uid, 'ALP_CLIENTE_DIRECCION', 'INSERT', p_id,
              JSON_OBJECT('cliente_id' VALUE p_cliente_id, 'ciudad_id' VALUE p_ciudad_id,
                          'tipo' VALUE p_tipo, 'principal' VALUE NVL(p_principal,'N')));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Dirección agregada';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_ACTUALIZAR_DIRECCION_CLIENTE(
        p_id            NUMBER,
        p_ciudad_id     NUMBER,
        p_tipo          VARCHAR2,
        p_linea1        VARCHAR2,
        p_linea2        VARCHAR2,
        p_codigo_postal VARCHAR2,
        p_referencia    VARCHAR2,
        p_resultado     OUT VARCHAR2,
        p_mensaje       OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_DIRECCION_CLIENTE(p_id) = 'N' THEN
            p_mensaje := 'Dirección no existe'; RAISE ex_neg;
        END IF;
        IF p_tipo NOT IN ('ENVIO','FACTURACION','AMBAS') THEN
            p_mensaje := 'Tipo inválido'; RAISE ex_neg;
        END IF;
        IF FN_EXISTE_CIUDAD(p_ciudad_id) = 'N' THEN
            p_mensaje := 'Ciudad no existe'; RAISE ex_neg;
        END IF;

        UPDATE ALP_CLIENTE_DIRECCION
        SET CIU_CIUDAD           = p_ciudad_id,
            CLD_TIPO             = p_tipo,
            CLD_DIRECCION_LINEA1 = TRIM(p_linea1),
            CLD_DIRECCION_LINEA2 = p_linea2,
            CLD_CODIGO_POSTAL    = p_codigo_postal,
            CLD_REFERENCIA       = p_referencia
        WHERE CLD_CLIENTE_DIRECCION = p_id;

        p_log(f_uid, 'ALP_CLIENTE_DIRECCION', 'UPDATE', p_id,
              JSON_OBJECT('ciudad_id' VALUE p_ciudad_id, 'tipo' VALUE p_tipo));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Dirección actualizada';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_INACTIVAR_DIRECCION_CLIENTE(
        p_id         NUMBER,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        ex_neg     EXCEPTION;
        v_principal VARCHAR2(1);
    BEGIN
        IF FN_PUEDE_INACTIVAR_DIRECCION(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene envíos u órdenes asociadas'; RAISE ex_neg;
        END IF;

        SELECT CLD_PRINCIPAL INTO v_principal
        FROM ALP_CLIENTE_DIRECCION
        WHERE CLD_CLIENTE_DIRECCION = p_id
        FOR UPDATE;

        IF v_principal = 'S' THEN
            p_mensaje := 'No se puede inactivar la dirección predeterminada'; RAISE ex_neg;
        END IF;

        UPDATE ALP_CLIENTE_DIRECCION
        SET CLD_ESTADO = 'INACTIVO'
        WHERE CLD_CLIENTE_DIRECCION = p_id;

        p_log(p_usuario_id, 'ALP_CLIENTE_DIRECCION', 'UPDATE', p_id,
              JSON_OBJECT('estado' VALUE 'INACTIVO'));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Dirección inactivada';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN NO_DATA_FOUND THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := 'Dirección no encontrada';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_MARCAR_DIRECCION_PREDETERMINADA(
        p_id         NUMBER,
        p_cliente_id NUMBER,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        -- Verificar que la dirección pertenece al cliente y está activa
        SELECT COUNT(1) INTO v_c
        FROM ALP_CLIENTE_DIRECCION
        WHERE CLD_CLIENTE_DIRECCION = p_id
          AND CLI_CLIENTE = p_cliente_id
          AND CLD_ESTADO = 'ACTIVO';

        IF v_c = 0 THEN
            p_mensaje := 'Dirección no encontrada, no pertenece al cliente o está inactiva';
            RAISE ex_neg;
        END IF;

        -- Desmarcar todas las del cliente
        UPDATE ALP_CLIENTE_DIRECCION
        SET CLD_PRINCIPAL = 'N'
        WHERE CLI_CLIENTE = p_cliente_id;

        -- Marcar la elegida
        UPDATE ALP_CLIENTE_DIRECCION
        SET CLD_PRINCIPAL = 'S'
        WHERE CLD_CLIENTE_DIRECCION = p_id;

        p_log(p_usuario_id, 'ALP_CLIENTE_DIRECCION', 'UPDATE', p_id,
              JSON_OBJECT('accion' VALUE 'MARCAR_PREDETERMINADA', 'cliente_id' VALUE p_cliente_id));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Dirección marcada como predeterminada';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_OBTENER_DIRECCION_CLIENTE(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT d.CLD_CLIENTE_DIRECCION, d.CLI_CLIENTE,
                   d.CIU_CIUDAD, ci.CIU_NOMBRE,
                   dep.DEP_NOMBRE, pai.PAI_NOMBRE,
                   d.CLD_TIPO, d.CLD_DIRECCION_LINEA1, d.CLD_DIRECCION_LINEA2,
                   d.CLD_CODIGO_POSTAL, d.CLD_REFERENCIA,
                   d.CLD_PRINCIPAL, d.CLD_ESTADO
            FROM   ALP_CLIENTE_DIRECCION d
            JOIN   ALP_CIUDAD ci        ON d.CIU_CIUDAD = ci.CIU_CIUDAD
            JOIN   ALP_DEPARTAMENTO dep ON ci.DEP_DEPARTAMENTO = dep.DEP_DEPARTAMENTO
            JOIN   ALP_PAIS pai         ON dep.PAI_PAIS = pai.PAI_PAIS
            WHERE  d.CLD_CLIENTE_DIRECCION = p_id;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_LISTAR_DIRECCIONES_CLIENTE(
        p_cliente_id  NUMBER,
        p_solo_activos VARCHAR2 DEFAULT 'S',
        p_cursor      OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT d.CLD_CLIENTE_DIRECCION,
                   d.CIU_CIUDAD, ci.CIU_NOMBRE,
                   dep.DEP_NOMBRE, pai.PAI_NOMBRE,
                   d.CLD_TIPO, d.CLD_DIRECCION_LINEA1,
                   d.CLD_CODIGO_POSTAL, d.CLD_PRINCIPAL, d.CLD_ESTADO
            FROM   ALP_CLIENTE_DIRECCION d
            JOIN   ALP_CIUDAD ci        ON d.CIU_CIUDAD = ci.CIU_CIUDAD
            JOIN   ALP_DEPARTAMENTO dep ON ci.DEP_DEPARTAMENTO = dep.DEP_DEPARTAMENTO
            JOIN   ALP_PAIS pai         ON dep.PAI_PAIS = pai.PAI_PAIS
            WHERE  d.CLI_CLIENTE = p_cliente_id
              AND  (p_solo_activos != 'S' OR d.CLD_ESTADO = 'ACTIVO')
            ORDER  BY d.CLD_PRINCIPAL DESC, d.CLD_CLIENTE_DIRECCION;
    END;

    -- -------------------------------------------------------------------------

    FUNCTION FN_EXISTE_DIRECCION_CLIENTE(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_CLIENTE_DIRECCION WHERE CLD_CLIENTE_DIRECCION = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_CLIENTE_TIENE_DIRECCION_PRINCIPAL(p_cliente_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_CLIENTE_DIRECCION
            WHERE CLI_CLIENTE = p_cliente_id AND CLD_PRINCIPAL = 'S' AND CLD_ESTADO = 'ACTIVO';
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_PUEDE_INACTIVAR_DIRECCION(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN
        -- No se puede inactivar si tiene envíos o es dirección de una orden
        SELECT COUNT(1) INTO v_c
        FROM DUAL
        WHERE EXISTS (SELECT 1 FROM ALP_ENVIO WHERE CLD_CLIENTE_DIRECCION = p_id AND ROWNUM = 1)
           OR EXISTS (SELECT 1 FROM ALP_ORDEN_VENTA WHERE CLD_CLIENTE_DIRECCION = p_id AND ROWNUM = 1);
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

END PKG_UBICACIONES;
/
-- ============================================================================
-- PKG_TIPOS
-- Módulo: Catálogos de Tipos relacionados a módulos trabajados
-- Tablas: ALP_TIPO_DOCUMENTO, ALP_TIPO_CLIENTE
-- Nota: ALP_TIPO_COMPROBANTE ya está cubierto en PKG_CATALOGOS_GENERALES.
--       ALP_TIPO_ENVIO no existe en el DDL actual; se omite.
-- Orden de ejecución: 3° (sin dependencias de paquetes nuevos)
-- ============================================================================

CREATE OR REPLACE PACKAGE PKG_TIPOS AS

    -- -------------------------------------------------------------------------
    -- ALP_TIPO_DOCUMENTO
    -- -------------------------------------------------------------------------
    PROCEDURE SP_CREAR_TIPO_DOCUMENTO         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_TIPO_DOCUMENTO    (p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_TIPO_DOCUMENTO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_TIPO_DOCUMENTO       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_TIPOS_DOCUMENTO       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_TIPO_DOCUMENTO        (p_id NUMBER)       RETURN VARCHAR2;
    FUNCTION  FN_EXISTE_CODIGO_TIPO_DOCUMENTO (p_codigo VARCHAR2) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_TIPO_DOCUMENTO        (p_id NUMBER)       RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_TIPO_DOCUMENTO(p_id NUMBER)      RETURN VARCHAR2;

    -- -------------------------------------------------------------------------
    -- ALP_TIPO_CLIENTE
    -- -------------------------------------------------------------------------
    PROCEDURE SP_CREAR_TIPO_CLIENTE         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_TIPO_CLIENTE    (p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_TIPO_CLIENTE(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_TIPO_CLIENTE       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_TIPOS_CLIENTE       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_TIPO_CLIENTE        (p_id NUMBER)       RETURN VARCHAR2;
    FUNCTION  FN_EXISTE_CODIGO_TIPO_CLIENTE (p_codigo VARCHAR2) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_TIPO_CLIENTE        (p_id NUMBER)       RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_TIPO_CLIENTE(p_id NUMBER)      RETURN VARCHAR2;

END PKG_TIPOS;
/

CREATE OR REPLACE PACKAGE BODY PKG_TIPOS AS

    -- -------------------------------------------------------------------------
    -- Helpers privados
    -- -------------------------------------------------------------------------

    PROCEDURE p_log(p_uid NUMBER, p_entidad VARCHAR2, p_op VARCHAR2, p_id NUMBER, p_datos CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG
            (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES (p_uid, p_entidad, p_op, p_id, p_datos);
        COMMIT;
    END;

    FUNCTION f_uid RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT USU_USUARIO INTO v_id FROM ALP_USUARIO WHERE USU_USERNAME = USER;
        RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL;
    END;

    -- =========================================================================
    -- ALP_TIPO_DOCUMENTO
    -- =========================================================================

    PROCEDURE SP_CREAR_TIPO_DOCUMENTO(
        p_codigo      VARCHAR2,
        p_nombre      VARCHAR2,
        p_descripcion VARCHAR2,
        p_resultado   OUT VARCHAR2,
        p_mensaje     OUT VARCHAR2,
        p_id          OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        IF FN_EXISTE_CODIGO_TIPO_DOCUMENTO(p_codigo) = 'S' THEN
            p_mensaje := 'Código ya existe: ' || p_codigo; RAISE ex_neg;
        END IF;

        INSERT INTO ALP_TIPO_DOCUMENTO (TDO_CODIGO, TDO_NOMBRE, TDO_DESCRIPCION, TDO_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING TDO_TIPO_DOCUMENTO INTO p_id;

        p_log(f_uid, 'ALP_TIPO_DOCUMENTO', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Tipo de documento creado';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN DUP_VAL_ON_INDEX THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := 'Código duplicado'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_ACTUALIZAR_TIPO_DOCUMENTO(
        p_id          NUMBER,
        p_nombre      VARCHAR2,
        p_descripcion VARCHAR2,
        p_resultado   OUT VARCHAR2,
        p_mensaje     OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_TIPO_DOCUMENTO(p_id) = 'N' THEN
            p_mensaje := 'Tipo de documento no existe'; RAISE ex_neg;
        END IF;
        IF p_nombre IS NULL THEN
            p_mensaje := 'Nombre es requerido'; RAISE ex_neg;
        END IF;

        UPDATE ALP_TIPO_DOCUMENTO
        SET TDO_NOMBRE      = TRIM(p_nombre),
            TDO_DESCRIPCION = p_descripcion
        WHERE TDO_TIPO_DOCUMENTO = p_id;

        p_log(f_uid, 'ALP_TIPO_DOCUMENTO', 'UPDATE', p_id,
              JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Tipo de documento actualizado';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_CAMBIAR_ESTADO_TIPO_DOCUMENTO(
        p_id         NUMBER,
        p_estado     VARCHAR2,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN
            p_mensaje := 'Estado inválido. Use ACTIVO o INACTIVO'; RAISE ex_neg;
        END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_TIPO_DOCUMENTO(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene clientes o empleados registrados con este tipo';
            RAISE ex_neg;
        END IF;

        UPDATE ALP_TIPO_DOCUMENTO
        SET TDO_ESTADO = p_estado
        WHERE TDO_TIPO_DOCUMENTO = p_id;

        IF SQL%ROWCOUNT = 0 THEN
            p_mensaje := 'Registro no encontrado'; RAISE ex_neg;
        END IF;

        p_log(p_usuario_id, 'ALP_TIPO_DOCUMENTO', 'UPDATE', p_id,
              JSON_OBJECT('estado' VALUE p_estado));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_OBTENER_TIPO_DOCUMENTO(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT TDO_TIPO_DOCUMENTO, TDO_CODIGO, TDO_NOMBRE, TDO_DESCRIPCION, TDO_ESTADO
            FROM ALP_TIPO_DOCUMENTO
            WHERE TDO_TIPO_DOCUMENTO = p_id;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_LISTAR_TIPOS_DOCUMENTO(
        p_solo_activos VARCHAR2 DEFAULT 'S',
        p_cursor       OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT TDO_TIPO_DOCUMENTO, TDO_CODIGO, TDO_NOMBRE, TDO_DESCRIPCION, TDO_ESTADO
            FROM ALP_TIPO_DOCUMENTO
            WHERE (p_solo_activos != 'S' OR TDO_ESTADO = 'ACTIVO')
            ORDER BY TDO_NOMBRE;
    END;

    -- -------------------------------------------------------------------------

    FUNCTION FN_EXISTE_TIPO_DOCUMENTO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_TIPO_DOCUMENTO WHERE TDO_TIPO_DOCUMENTO = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_EXISTE_CODIGO_TIPO_DOCUMENTO(p_codigo VARCHAR2) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_TIPO_DOCUMENTO WHERE TDO_CODIGO = UPPER(TRIM(p_codigo));
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_ACTIVO_TIPO_DOCUMENTO(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT TDO_ESTADO INTO v_e FROM ALP_TIPO_DOCUMENTO WHERE TDO_TIPO_DOCUMENTO = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_TIPO_DOCUMENTO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        -- Verifica clientes Y empleados que usan este tipo de documento
        SELECT COUNT(1) INTO v_c
        FROM DUAL
        WHERE EXISTS (SELECT 1 FROM ALP_CLIENTE  WHERE TDO_TIPO_DOCUMENTO = p_id AND ROWNUM = 1)
           OR EXISTS (SELECT 1 FROM ALP_EMPLEADO WHERE TDO_TIPO_DOCUMENTO = p_id AND ROWNUM = 1);
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_TIPO_CLIENTE
    -- =========================================================================

    PROCEDURE SP_CREAR_TIPO_CLIENTE(
        p_codigo      VARCHAR2,
        p_nombre      VARCHAR2,
        p_descripcion VARCHAR2,
        p_resultado   OUT VARCHAR2,
        p_mensaje     OUT VARCHAR2,
        p_id          OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        IF FN_EXISTE_CODIGO_TIPO_CLIENTE(p_codigo) = 'S' THEN
            p_mensaje := 'Código ya existe: ' || p_codigo; RAISE ex_neg;
        END IF;

        INSERT INTO ALP_TIPO_CLIENTE (TCL_CODIGO, TCL_NOMBRE, TCL_DESCRIPCION, TCL_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING TCL_TIPO_CLIENTE INTO p_id;

        p_log(f_uid, 'ALP_TIPO_CLIENTE', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Tipo de cliente creado';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN DUP_VAL_ON_INDEX THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := 'Código duplicado'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_ACTUALIZAR_TIPO_CLIENTE(
        p_id          NUMBER,
        p_nombre      VARCHAR2,
        p_descripcion VARCHAR2,
        p_resultado   OUT VARCHAR2,
        p_mensaje     OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_TIPO_CLIENTE(p_id) = 'N' THEN
            p_mensaje := 'Tipo de cliente no existe'; RAISE ex_neg;
        END IF;
        IF p_nombre IS NULL THEN
            p_mensaje := 'Nombre es requerido'; RAISE ex_neg;
        END IF;

        UPDATE ALP_TIPO_CLIENTE
        SET TCL_NOMBRE      = TRIM(p_nombre),
            TCL_DESCRIPCION = p_descripcion
        WHERE TCL_TIPO_CLIENTE = p_id;

        p_log(f_uid, 'ALP_TIPO_CLIENTE', 'UPDATE', p_id,
              JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Tipo de cliente actualizado';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_CAMBIAR_ESTADO_TIPO_CLIENTE(
        p_id         NUMBER,
        p_estado     VARCHAR2,
        p_usuario_id NUMBER,
        p_resultado  OUT VARCHAR2,
        p_mensaje    OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN
            p_mensaje := 'Estado inválido. Use ACTIVO o INACTIVO'; RAISE ex_neg;
        END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_TIPO_CLIENTE(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene clientes registrados con este tipo';
            RAISE ex_neg;
        END IF;

        UPDATE ALP_TIPO_CLIENTE
        SET TCL_ESTADO = p_estado
        WHERE TCL_TIPO_CLIENTE = p_id;

        IF SQL%ROWCOUNT = 0 THEN
            p_mensaje := 'Registro no encontrado'; RAISE ex_neg;
        END IF;

        p_log(p_usuario_id, 'ALP_TIPO_CLIENTE', 'UPDATE', p_id,
              JSON_OBJECT('estado' VALUE p_estado));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';

    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_OBTENER_TIPO_CLIENTE(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT TCL_TIPO_CLIENTE, TCL_CODIGO, TCL_NOMBRE, TCL_DESCRIPCION, TCL_ESTADO
            FROM ALP_TIPO_CLIENTE
            WHERE TCL_TIPO_CLIENTE = p_id;
    END;

    -- -------------------------------------------------------------------------

    PROCEDURE SP_LISTAR_TIPOS_CLIENTE(
        p_solo_activos VARCHAR2 DEFAULT 'S',
        p_cursor       OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT TCL_TIPO_CLIENTE, TCL_CODIGO, TCL_NOMBRE, TCL_DESCRIPCION, TCL_ESTADO
            FROM ALP_TIPO_CLIENTE
            WHERE (p_solo_activos != 'S' OR TCL_ESTADO = 'ACTIVO')
            ORDER BY TCL_NOMBRE;
    END;

    -- -------------------------------------------------------------------------

    FUNCTION FN_EXISTE_TIPO_CLIENTE(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_TIPO_CLIENTE WHERE TCL_TIPO_CLIENTE = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_EXISTE_CODIGO_TIPO_CLIENTE(p_codigo VARCHAR2) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_TIPO_CLIENTE WHERE TCL_CODIGO = UPPER(TRIM(p_codigo));
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_ACTIVO_TIPO_CLIENTE(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT TCL_ESTADO INTO v_e FROM ALP_TIPO_CLIENTE WHERE TCL_TIPO_CLIENTE = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_TIPO_CLIENTE(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c
        FROM ALP_CLIENTE
        WHERE TCL_TIPO_CLIENTE = p_id
          AND CLI_ESTADO = 'ACTIVO'
          AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

END PKG_TIPOS;
/
