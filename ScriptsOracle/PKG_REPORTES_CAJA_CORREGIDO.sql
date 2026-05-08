-- ==============================================================================
-- PKG_REPORTES_CAJA CORREGIDO
-- Correccion aplicada:
--   DIFERENCIA_CAJA = MONTO_FINAL - MONTO_INICIAL - TOTAL_VENTAS
--   Se conserva el alias DIFERENCIA_CAJA para evitar ORA-50033 en el backend.
-- ==============================================================================

CREATE OR REPLACE PACKAGE PKG_REPORTES_CAJA AS

    PROCEDURE SP_REGISTRAR_EJECUCION_REPORTE(
        p_usu_usuario               IN  ALP_EJECUCION_REPORTE.USU_USUARIO%TYPE DEFAULT NULL,
        p_ejr_nombre_reporte        IN  ALP_EJECUCION_REPORTE.EJR_NOMBRE_REPORTE%TYPE,
        p_ejr_parametros            IN  ALP_EJECUCION_REPORTE.EJR_PARAMETROS%TYPE DEFAULT NULL,
        p_ejr_tiempo_ejecucion_ms   IN  ALP_EJECUCION_REPORTE.EJR_TIEMPO_EJECUCION_MS%TYPE DEFAULT NULL,
        p_ejr_estado                IN  ALP_EJECUCION_REPORTE.EJR_ESTADO%TYPE DEFAULT 'EXITOSO',
        p_ejr_ejecucion_reporte_out OUT ALP_EJECUCION_REPORTE.EJR_EJECUCION_REPORTE%TYPE
    );

    PROCEDURE SP_GENERAR_REPORTE_CORTE_CAJA(
        p_fecha_inicio              IN  DATE,
        p_fecha_fin                 IN  DATE,
        p_estado                    IN  ALP_CORTE_CAJA.RCC_ESTADO%TYPE DEFAULT NULL,
        p_usu_usuario               IN  ALP_EJECUCION_REPORTE.USU_USUARIO%TYPE DEFAULT NULL,
        p_resultado                 OUT SYS_REFCURSOR
    );

    FUNCTION FN_TOTAL_VENTAS_CORTE_CAJA(
        p_rcc_reporte_corte_caja    IN ALP_CORTE_CAJA.RCC_REPORTE_CORTE_CAJA%TYPE
    ) RETURN NUMBER;

    FUNCTION FN_DIFERENCIA_CORTE_CAJA(
        p_rcc_reporte_corte_caja    IN ALP_CORTE_CAJA.RCC_REPORTE_CORTE_CAJA%TYPE
    ) RETURN NUMBER;

    PROCEDURE SP_REGISTRAR_EJECUCION_REPORTE(
        p_usu_usuario IN ALP_EJECUCION_REPORTE.USU_USUARIO%TYPE,
        p_ejr_nombre_reporte IN ALP_EJECUCION_REPORTE.EJR_NOMBRE_REPORTE%TYPE,
        p_ejr_parametros IN ALP_EJECUCION_REPORTE.EJR_PARAMETROS%TYPE,
        p_ejr_tiempo_ejecucion_ms IN ALP_EJECUCION_REPORTE.EJR_TIEMPO_EJECUCION_MS%TYPE,
        p_ejr_estado IN ALP_EJECUCION_REPORTE.EJR_ESTADO%TYPE,
        p_usuario_id IN NUMBER,
        p_resultado OUT VARCHAR2,
        p_mensaje OUT VARCHAR2,
        p_id OUT NUMBER
    );

END PKG_REPORTES_CAJA;
/

CREATE OR REPLACE PACKAGE BODY PKG_REPORTES_CAJA AS

    PROCEDURE p_log(p_uid NUMBER, p_entidad VARCHAR2, p_op VARCHAR2, p_id NUMBER, p_datos CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG
            (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES
            (p_uid, p_entidad, p_op, p_id, p_datos);

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
    END p_log;

    PROCEDURE PR_VALIDAR_RANGO_FECHAS(
        p_fecha_inicio IN DATE,
        p_fecha_fin    IN DATE
    )
    IS
    BEGIN
        IF p_fecha_inicio IS NULL OR p_fecha_fin IS NULL THEN
            RAISE_APPLICATION_ERROR(-20870, 'Las fechas inicio y fin son obligatorias.');
        END IF;

        IF p_fecha_fin < p_fecha_inicio THEN
            RAISE_APPLICATION_ERROR(-20871, 'La fecha fin no puede ser menor que la fecha inicio.');
        END IF;
    END PR_VALIDAR_RANGO_FECHAS;

    FUNCTION FN_EXISTE_CORTE_CAJA_LOCAL(
        p_rcc_reporte_corte_caja IN ALP_CORTE_CAJA.RCC_REPORTE_CORTE_CAJA%TYPE
    ) RETURN NUMBER
    IS
        v_count NUMBER;
    BEGIN
        SELECT COUNT(*)
          INTO v_count
          FROM ALP_CORTE_CAJA
         WHERE RCC_REPORTE_CORTE_CAJA = p_rcc_reporte_corte_caja;

        RETURN CASE WHEN v_count > 0 THEN 1 ELSE 0 END;
    END FN_EXISTE_CORTE_CAJA_LOCAL;

    PROCEDURE PR_REGISTRAR_EJECUCION_INTERNA(
        p_usu_usuario        IN ALP_EJECUCION_REPORTE.USU_USUARIO%TYPE DEFAULT NULL,
        p_ejr_nombre_reporte IN ALP_EJECUCION_REPORTE.EJR_NOMBRE_REPORTE%TYPE,
        p_ejr_parametros     IN ALP_EJECUCION_REPORTE.EJR_PARAMETROS%TYPE DEFAULT NULL,
        p_ejr_tiempo_ms      IN ALP_EJECUCION_REPORTE.EJR_TIEMPO_EJECUCION_MS%TYPE DEFAULT NULL,
        p_ejr_estado         IN ALP_EJECUCION_REPORTE.EJR_ESTADO%TYPE DEFAULT 'EXITOSO'
    )
    IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_EJECUCION_REPORTE (
            USU_USUARIO,
            EJR_NOMBRE_REPORTE,
            EJR_PARAMETROS,
            EJR_TIEMPO_EJECUCION_MS,
            EJR_ESTADO
        )
        VALUES (
            p_usu_usuario,
            p_ejr_nombre_reporte,
            p_ejr_parametros,
            p_ejr_tiempo_ms,
            p_ejr_estado
        );

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
    END PR_REGISTRAR_EJECUCION_INTERNA;

    PROCEDURE SP_REGISTRAR_EJECUCION_REPORTE(
        p_usu_usuario               IN  ALP_EJECUCION_REPORTE.USU_USUARIO%TYPE DEFAULT NULL,
        p_ejr_nombre_reporte        IN  ALP_EJECUCION_REPORTE.EJR_NOMBRE_REPORTE%TYPE,
        p_ejr_parametros            IN  ALP_EJECUCION_REPORTE.EJR_PARAMETROS%TYPE DEFAULT NULL,
        p_ejr_tiempo_ejecucion_ms   IN  ALP_EJECUCION_REPORTE.EJR_TIEMPO_EJECUCION_MS%TYPE DEFAULT NULL,
        p_ejr_estado                IN  ALP_EJECUCION_REPORTE.EJR_ESTADO%TYPE DEFAULT 'EXITOSO',
        p_ejr_ejecucion_reporte_out OUT ALP_EJECUCION_REPORTE.EJR_EJECUCION_REPORTE%TYPE
    )
    IS
    BEGIN
        INSERT INTO ALP_EJECUCION_REPORTE (
            USU_USUARIO,
            EJR_NOMBRE_REPORTE,
            EJR_PARAMETROS,
            EJR_TIEMPO_EJECUCION_MS,
            EJR_ESTADO
        )
        VALUES (
            p_usu_usuario,
            p_ejr_nombre_reporte,
            p_ejr_parametros,
            p_ejr_tiempo_ejecucion_ms,
            p_ejr_estado
        )
        RETURNING EJR_EJECUCION_REPORTE
             INTO p_ejr_ejecucion_reporte_out;

        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE;
    END SP_REGISTRAR_EJECUCION_REPORTE;

    PROCEDURE SP_GENERAR_REPORTE_CORTE_CAJA(
        p_fecha_inicio              IN  DATE,
        p_fecha_fin                 IN  DATE,
        p_estado                    IN  ALP_CORTE_CAJA.RCC_ESTADO%TYPE DEFAULT NULL,
        p_usu_usuario               IN  ALP_EJECUCION_REPORTE.USU_USUARIO%TYPE DEFAULT NULL,
        p_resultado                 OUT SYS_REFCURSOR
    )
    IS
    BEGIN
        PR_VALIDAR_RANGO_FECHAS(p_fecha_inicio, p_fecha_fin);

        OPEN p_resultado FOR
            SELECT c.RCC_REPORTE_CORTE_CAJA,
                   c.RCC_FECHA_CORTE,
                   c.RCC_MONTO_INICIAL,
                   c.RCC_MONTO_FINAL,
                   c.RCC_TOTAL_VENTAS,
                   c.RCC_OBSERVACION,
                   c.RCC_ESTADO,
                   (
                       NVL(c.RCC_MONTO_FINAL, 0)
                       - NVL(c.RCC_MONTO_INICIAL, 0)
                       - NVL(c.RCC_TOTAL_VENTAS, 0)
                   ) AS DIFERENCIA_CAJA
              FROM ALP_CORTE_CAJA c
             WHERE c.RCC_FECHA_CORTE BETWEEN p_fecha_inicio AND p_fecha_fin
               AND (p_estado IS NULL OR c.RCC_ESTADO = p_estado)
             ORDER BY c.RCC_FECHA_CORTE DESC, c.RCC_REPORTE_CORTE_CAJA DESC;

        PR_REGISTRAR_EJECUCION_INTERNA(
            p_usu_usuario        => p_usu_usuario,
            p_ejr_nombre_reporte => 'SP_GENERAR_REPORTE_CORTE_CAJA',
            p_ejr_parametros     => '{"fecha_inicio":"' || TO_CHAR(p_fecha_inicio, 'YYYY-MM-DD') ||
                                    '","fecha_fin":"' || TO_CHAR(p_fecha_fin, 'YYYY-MM-DD') ||
                                    '","estado":"' || NVL(p_estado, 'NULL') || '"}',
            p_ejr_estado         => 'EXITOSO'
        );
    EXCEPTION
        WHEN OTHERS THEN
            PR_REGISTRAR_EJECUCION_INTERNA(
                p_usu_usuario        => p_usu_usuario,
                p_ejr_nombre_reporte => 'SP_GENERAR_REPORTE_CORTE_CAJA',
                p_ejr_parametros     => '{"estado":"error"}',
                p_ejr_estado         => 'ERROR'
            );
            RAISE;
    END SP_GENERAR_REPORTE_CORTE_CAJA;

    FUNCTION FN_TOTAL_VENTAS_CORTE_CAJA(
        p_rcc_reporte_corte_caja    IN ALP_CORTE_CAJA.RCC_REPORTE_CORTE_CAJA%TYPE
    ) RETURN NUMBER
    IS
        v_total NUMBER;
    BEGIN
        IF FN_EXISTE_CORTE_CAJA_LOCAL(p_rcc_reporte_corte_caja) = 0 THEN
            RAISE_APPLICATION_ERROR(-20872, 'El corte de caja no existe.');
        END IF;

        SELECT RCC_TOTAL_VENTAS
          INTO v_total
          FROM ALP_CORTE_CAJA
         WHERE RCC_REPORTE_CORTE_CAJA = p_rcc_reporte_corte_caja;

        RETURN NVL(v_total, 0);
    END FN_TOTAL_VENTAS_CORTE_CAJA;

    FUNCTION FN_DIFERENCIA_CORTE_CAJA(
        p_rcc_reporte_corte_caja    IN ALP_CORTE_CAJA.RCC_REPORTE_CORTE_CAJA%TYPE
    ) RETURN NUMBER
    IS
        v_diferencia NUMBER;
    BEGIN
        IF FN_EXISTE_CORTE_CAJA_LOCAL(p_rcc_reporte_corte_caja) = 0 THEN
            RAISE_APPLICATION_ERROR(-20873, 'El corte de caja no existe.');
        END IF;

        SELECT NVL(RCC_MONTO_FINAL, 0)
               - NVL(RCC_MONTO_INICIAL, 0)
               - NVL(RCC_TOTAL_VENTAS, 0)
          INTO v_diferencia
          FROM ALP_CORTE_CAJA
         WHERE RCC_REPORTE_CORTE_CAJA = p_rcc_reporte_corte_caja;

        RETURN v_diferencia;
    END FN_DIFERENCIA_CORTE_CAJA;

    PROCEDURE SP_REGISTRAR_EJECUCION_REPORTE(
        p_usu_usuario IN ALP_EJECUCION_REPORTE.USU_USUARIO%TYPE,
        p_ejr_nombre_reporte IN ALP_EJECUCION_REPORTE.EJR_NOMBRE_REPORTE%TYPE,
        p_ejr_parametros IN ALP_EJECUCION_REPORTE.EJR_PARAMETROS%TYPE,
        p_ejr_tiempo_ejecucion_ms IN ALP_EJECUCION_REPORTE.EJR_TIEMPO_EJECUCION_MS%TYPE,
        p_ejr_estado IN ALP_EJECUCION_REPORTE.EJR_ESTADO%TYPE,
        p_usuario_id IN NUMBER,
        p_resultado OUT VARCHAR2,
        p_mensaje OUT VARCHAR2,
        p_id OUT NUMBER
    ) IS
        v_ejr_ejecucion_reporte_out ALP_EJECUCION_REPORTE.EJR_EJECUCION_REPORTE%TYPE;
    BEGIN
        SP_REGISTRAR_EJECUCION_REPORTE(
            p_usu_usuario => p_usu_usuario,
            p_ejr_nombre_reporte => p_ejr_nombre_reporte,
            p_ejr_parametros => p_ejr_parametros,
            p_ejr_tiempo_ejecucion_ms => p_ejr_tiempo_ejecucion_ms,
            p_ejr_estado => p_ejr_estado,
            p_ejr_ejecucion_reporte_out => v_ejr_ejecucion_reporte_out
        );

        p_id := v_ejr_ejecucion_reporte_out;

        p_log(
            p_usuario_id,
            'ALP_EJECUCION_REPORTE',
            'INSERT',
            p_id,
            TO_CLOB('{"procedimiento":"SP_REGISTRAR_EJECUCION_REPORTE"}')
        );

        p_resultado := 'EXITO';
        p_mensaje   := 'Registrar ejecucion reporte registrado correctamente';
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            p_resultado := 'ERROR';
            p_mensaje   := SQLERRM;
            p_id        := NULL;
    END SP_REGISTRAR_EJECUCION_REPORTE;

END PKG_REPORTES_CAJA;
/
