-- ============================================================================
-- PKG_CATALOGOS_GENERALES
-- Módulo 3: Catálogos Comerciales
-- Tablas: ALP_FORMA_PAGO, ALP_ESTADO_ORDEN, ALP_ESTADO_PAGO,
--         ALP_TIPO_COMPROBANTE, ALP_CANAL_VENTA, ALP_CATEGORIA_TIPO_DEVOLUCION
-- ============================================================================

CREATE OR REPLACE PACKAGE PKG_CATALOGOS_GENERALES AS

    -- ALP_FORMA_PAGO
    PROCEDURE SP_CREAR_FORMA_PAGO         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_FORMA_PAGO    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_FORMA_PAGO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_FORMA_PAGO       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_FORMAS_PAGO       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_FORMA_PAGO        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_EXISTE_CODIGO_FORMA_PAGO (p_codigo VARCHAR2) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_FORMA_PAGO        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_FORMA_PAGO(p_id NUMBER)    RETURN VARCHAR2;

    -- ALP_ESTADO_ORDEN
    PROCEDURE SP_CREAR_ESTADO_ORDEN         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_ESTADO_ORDEN_CAT(p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_ESTADO_ORDEN(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_ESTADO_ORDEN_CAT   (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_ESTADOS_ORDEN       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_ESTADO_ORDEN        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_EXISTE_CODIGO_ESTADO_ORDEN (p_codigo VARCHAR2) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_ESTADO_ORDEN        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_ESTADO_ORDEN(p_id NUMBER)    RETURN VARCHAR2;

    -- ALP_ESTADO_PAGO
    PROCEDURE SP_CREAR_ESTADO_PAGO         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_ESTADO_PAGO    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_ESTADO_PAGO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_ESTADO_PAGO       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_ESTADOS_PAGO       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_ESTADO_PAGO        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_ESTADO_PAGO        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_ESTADO_PAGO(p_id NUMBER)    RETURN VARCHAR2;

    -- ALP_TIPO_COMPROBANTE
    PROCEDURE SP_CREAR_TIPO_COMPROBANTE         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_TIPO_COMPROBANTE    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_TIPO_COMPROBANTE(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_TIPO_COMPROBANTE       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_TIPOS_COMPROBANTE       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_TIPO_COMPROBANTE        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_TIPO_COMPROBANTE        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_TIPO_COMPROBANTE(p_id NUMBER)    RETURN VARCHAR2;

    -- ALP_CANAL_VENTA
    PROCEDURE SP_CREAR_CANAL_VENTA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_CANAL_VENTA    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_CANAL_VENTA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_CANALES_VENTA      (p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_CANAL_VENTA        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_EXISTE_CODIGO_CANAL_VENTA (p_codigo VARCHAR2) RETURN VARCHAR2;

    -- ALP_CATEGORIA_TIPO_DEVOLUCION
    PROCEDURE SP_CREAR_CAT_TIPO_DEV         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_CAT_TIPO_DEV    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_CAT_TIPO_DEV(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_CAT_TIPO_DEV       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_CAT_TIPO_DEV        (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_CAT_TIPO_DEV        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_CAT_TIPO_DEV        (p_id NUMBER)     RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_CAT_TIPO_DEV(p_id NUMBER)   RETURN VARCHAR2;

    -- Utilidad genérica de catálogos
    FUNCTION  FN_OBTENER_ID_POR_CODIGO      (p_tabla VARCHAR2, p_col_pk VARCHAR2, p_col_codigo VARCHAR2, p_codigo VARCHAR2) RETURN NUMBER;

END PKG_CATALOGOS_GENERALES;
/

CREATE OR REPLACE PACKAGE BODY PKG_CATALOGOS_GENERALES AS

    -- =========================================================================
    -- HELPERS PRIVADOS
    -- =========================================================================

    PROCEDURE p_log(p_usuario_id NUMBER, p_entidad VARCHAR2, p_operacion VARCHAR2, p_id NUMBER, p_datos CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES (p_usuario_id, p_entidad, p_operacion, p_id, p_datos);
        COMMIT;
    END;

    FUNCTION f_usuario_sesion RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT USU_USUARIO INTO v_id FROM ALP_USUARIO WHERE USU_USERNAME = USER;
        RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL;
    END;

    -- =========================================================================
    -- ALP_FORMA_PAGO
    -- =========================================================================

    PROCEDURE SP_CREAR_FORMA_PAGO(
        p_codigo      VARCHAR2, p_nombre      VARCHAR2, p_descripcion VARCHAR2,
        p_resultado   OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        IF FN_EXISTE_CODIGO_FORMA_PAGO(p_codigo) = 'S' THEN
            p_mensaje := 'Código ya existe: ' || p_codigo; RAISE ex_neg;
        END IF;

        INSERT INTO ALP_FORMA_PAGO (FPA_CODIGO, FPA_NOMBRE, FPA_DESCRIPCION, FPA_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING FPA_FORMA_PAGO INTO p_id;

        p_log(f_usuario_sesion, 'ALP_FORMA_PAGO', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Forma de pago creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN DUP_VAL_ON_INDEX THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := 'Código duplicado'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_FORMA_PAGO(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_FORMA_PAGO(p_id) = 'N' THEN
            p_mensaje := 'Forma de pago no existe'; RAISE ex_neg;
        END IF;
        UPDATE ALP_FORMA_PAGO
        SET FPA_CODIGO = UPPER(TRIM(p_codigo)), FPA_NOMBRE = TRIM(p_nombre), FPA_DESCRIPCION = p_descripcion
        WHERE FPA_FORMA_PAGO = p_id;

        p_log(f_usuario_sesion, 'ALP_FORMA_PAGO', 'UPDATE', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Forma de pago actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_FORMA_PAGO(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN
            p_mensaje := 'Estado inválido. Use ACTIVO o INACTIVO'; RAISE ex_neg;
        END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_FORMA_PAGO(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene pagos asociados'; RAISE ex_neg;
        END IF;
        UPDATE ALP_FORMA_PAGO SET FPA_ESTADO = p_estado WHERE FPA_FORMA_PAGO = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;

        p_log(p_usuario_id, 'ALP_FORMA_PAGO', 'UPDATE', p_id,
              JSON_OBJECT('estado' VALUE p_estado));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_FORMA_PAGO(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT FPA_FORMA_PAGO, FPA_CODIGO, FPA_NOMBRE, FPA_DESCRIPCION, FPA_ESTADO
            FROM ALP_FORMA_PAGO WHERE FPA_FORMA_PAGO = p_id;
    END;

    PROCEDURE SP_LISTAR_FORMAS_PAGO(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT FPA_FORMA_PAGO, FPA_CODIGO, FPA_NOMBRE, FPA_DESCRIPCION, FPA_ESTADO
            FROM ALP_FORMA_PAGO
            WHERE (p_solo_activos != 'S' OR FPA_ESTADO = 'ACTIVO')
            ORDER BY FPA_NOMBRE;
    END;

    FUNCTION FN_EXISTE_FORMA_PAGO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_FORMA_PAGO WHERE FPA_FORMA_PAGO = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_EXISTE_CODIGO_FORMA_PAGO(p_codigo VARCHAR2) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_FORMA_PAGO WHERE FPA_CODIGO = UPPER(TRIM(p_codigo));
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_ACTIVO_FORMA_PAGO(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT FPA_ESTADO INTO v_e FROM ALP_FORMA_PAGO WHERE FPA_FORMA_PAGO = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_FORMA_PAGO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c
        FROM ALP_PAGO
        WHERE FPA_FORMA_PAGO = p_id
          AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_ESTADO_ORDEN
    -- =========================================================================

    PROCEDURE SP_CREAR_ESTADO_ORDEN(
        p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        IF FN_EXISTE_CODIGO_ESTADO_ORDEN(p_codigo) = 'S' THEN
            p_mensaje := 'Código ya existe: ' || p_codigo; RAISE ex_neg;
        END IF;
        INSERT INTO ALP_ESTADO_ORDEN (ESO_CODIGO, ESO_NOMBRE, ESO_DESCRIPCION, ESO_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING ESO_ESTADO_ORDEN INTO p_id;

        p_log(f_usuario_sesion, 'ALP_ESTADO_ORDEN', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado de orden creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_ESTADO_ORDEN_CAT(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_ESTADO_ORDEN(p_id) = 'N' THEN
            p_mensaje := 'Estado de orden no existe'; RAISE ex_neg;
        END IF;
        UPDATE ALP_ESTADO_ORDEN
        SET ESO_CODIGO = UPPER(TRIM(p_codigo)), ESO_NOMBRE = TRIM(p_nombre), ESO_DESCRIPCION = p_descripcion
        WHERE ESO_ESTADO_ORDEN = p_id;

        p_log(f_usuario_sesion, 'ALP_ESTADO_ORDEN', 'UPDATE', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado de orden actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_ESTADO_ORDEN(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN
            p_mensaje := 'Estado inválido'; RAISE ex_neg;
        END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_ESTADO_ORDEN(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene órdenes asociadas'; RAISE ex_neg;
        END IF;
        UPDATE ALP_ESTADO_ORDEN SET ESO_ESTADO = p_estado WHERE ESO_ESTADO_ORDEN = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_ESTADO_ORDEN', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_ESTADO_ORDEN_CAT(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT ESO_ESTADO_ORDEN, ESO_CODIGO, ESO_NOMBRE, ESO_DESCRIPCION, ESO_ESTADO
            FROM ALP_ESTADO_ORDEN WHERE ESO_ESTADO_ORDEN = p_id;
    END;

    PROCEDURE SP_LISTAR_ESTADOS_ORDEN(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT ESO_ESTADO_ORDEN, ESO_CODIGO, ESO_NOMBRE, ESO_DESCRIPCION, ESO_ESTADO
            FROM ALP_ESTADO_ORDEN
            WHERE (p_solo_activos != 'S' OR ESO_ESTADO = 'ACTIVO')
            ORDER BY ESO_NOMBRE;
    END;

    FUNCTION FN_EXISTE_ESTADO_ORDEN(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_ESTADO_ORDEN WHERE ESO_ESTADO_ORDEN = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_EXISTE_CODIGO_ESTADO_ORDEN(p_codigo VARCHAR2) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_ESTADO_ORDEN WHERE ESO_CODIGO = UPPER(TRIM(p_codigo));
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_ACTIVO_ESTADO_ORDEN(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT ESO_ESTADO INTO v_e FROM ALP_ESTADO_ORDEN WHERE ESO_ESTADO_ORDEN = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_ESTADO_ORDEN(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_ORDEN_VENTA WHERE ESO_ESTADO_ORDEN = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_ESTADO_PAGO
    -- =========================================================================

    PROCEDURE SP_CREAR_ESTADO_PAGO(
        p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
        v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_ESTADO_PAGO WHERE ESP_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_ESTADO_PAGO (ESP_CODIGO, ESP_NOMBRE, ESP_DESCRIPCION, ESP_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING ESP_ESTADO_PAGO INTO p_id;

        p_log(f_usuario_sesion, 'ALP_ESTADO_PAGO', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado de pago creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_ESTADO_PAGO(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_ESTADO_PAGO(p_id) = 'N' THEN
            p_mensaje := 'Estado de pago no existe'; RAISE ex_neg;
        END IF;
        UPDATE ALP_ESTADO_PAGO
        SET ESP_CODIGO = UPPER(TRIM(p_codigo)), ESP_NOMBRE = TRIM(p_nombre), ESP_DESCRIPCION = p_descripcion
        WHERE ESP_ESTADO_PAGO = p_id;
        p_log(f_usuario_sesion, 'ALP_ESTADO_PAGO', 'UPDATE', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado de pago actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_ESTADO_PAGO(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN
            p_mensaje := 'Estado inválido'; RAISE ex_neg;
        END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_ESTADO_PAGO(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene pagos asociados'; RAISE ex_neg;
        END IF;
        UPDATE ALP_ESTADO_PAGO SET ESP_ESTADO = p_estado WHERE ESP_ESTADO_PAGO = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_ESTADO_PAGO', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_ESTADO_PAGO(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT ESP_ESTADO_PAGO, ESP_CODIGO, ESP_NOMBRE, ESP_DESCRIPCION, ESP_ESTADO
            FROM ALP_ESTADO_PAGO WHERE ESP_ESTADO_PAGO = p_id;
    END;

    PROCEDURE SP_LISTAR_ESTADOS_PAGO(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT ESP_ESTADO_PAGO, ESP_CODIGO, ESP_NOMBRE, ESP_DESCRIPCION, ESP_ESTADO
            FROM ALP_ESTADO_PAGO
            WHERE (p_solo_activos != 'S' OR ESP_ESTADO = 'ACTIVO')
            ORDER BY ESP_NOMBRE;
    END;

    FUNCTION FN_EXISTE_ESTADO_PAGO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_ESTADO_PAGO WHERE ESP_ESTADO_PAGO = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_ACTIVO_ESTADO_PAGO(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT ESP_ESTADO INTO v_e FROM ALP_ESTADO_PAGO WHERE ESP_ESTADO_PAGO = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_ESTADO_PAGO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_PAGO WHERE ESP_ESTADO_PAGO = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_TIPO_COMPROBANTE
    -- =========================================================================

    PROCEDURE SP_CREAR_TIPO_COMPROBANTE(
        p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
        v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_TIPO_COMPROBANTE WHERE TCO_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_TIPO_COMPROBANTE (TCO_CODIGO, TCO_NOMBRE, TCO_DESCRIPCION, TCO_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING TCO_TIPO_COMPROBANTE INTO p_id;

        p_log(f_usuario_sesion, 'ALP_TIPO_COMPROBANTE', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Tipo de comprobante creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_TIPO_COMPROBANTE(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_TIPO_COMPROBANTE(p_id) = 'N' THEN
            p_mensaje := 'Tipo de comprobante no existe'; RAISE ex_neg;
        END IF;
        UPDATE ALP_TIPO_COMPROBANTE
        SET TCO_CODIGO = UPPER(TRIM(p_codigo)), TCO_NOMBRE = TRIM(p_nombre), TCO_DESCRIPCION = p_descripcion
        WHERE TCO_TIPO_COMPROBANTE = p_id;
        p_log(f_usuario_sesion, 'ALP_TIPO_COMPROBANTE', 'UPDATE', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Tipo de comprobante actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_TIPO_COMPROBANTE(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN
            p_mensaje := 'Estado inválido'; RAISE ex_neg;
        END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_TIPO_COMPROBANTE(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene comprobantes emitidos'; RAISE ex_neg;
        END IF;
        UPDATE ALP_TIPO_COMPROBANTE SET TCO_ESTADO = p_estado WHERE TCO_TIPO_COMPROBANTE = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_TIPO_COMPROBANTE', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_TIPO_COMPROBANTE(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT TCO_TIPO_COMPROBANTE, TCO_CODIGO, TCO_NOMBRE, TCO_DESCRIPCION, TCO_ESTADO
            FROM ALP_TIPO_COMPROBANTE WHERE TCO_TIPO_COMPROBANTE = p_id;
    END;

    PROCEDURE SP_LISTAR_TIPOS_COMPROBANTE(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT TCO_TIPO_COMPROBANTE, TCO_CODIGO, TCO_NOMBRE, TCO_DESCRIPCION, TCO_ESTADO
            FROM ALP_TIPO_COMPROBANTE
            WHERE (p_solo_activos != 'S' OR TCO_ESTADO = 'ACTIVO')
            ORDER BY TCO_NOMBRE;
    END;

    FUNCTION FN_EXISTE_TIPO_COMPROBANTE(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_TIPO_COMPROBANTE WHERE TCO_TIPO_COMPROBANTE = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_ACTIVO_TIPO_COMPROBANTE(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT TCO_ESTADO INTO v_e FROM ALP_TIPO_COMPROBANTE WHERE TCO_TIPO_COMPROBANTE = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_TIPO_COMPROBANTE(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_COMPROBANTE WHERE TCO_TIPO_COMPROBANTE = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_CANAL_VENTA
    -- =========================================================================

    PROCEDURE SP_CREAR_CANAL_VENTA(
        p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        IF FN_EXISTE_CODIGO_CANAL_VENTA(p_codigo) = 'S' THEN
            p_mensaje := 'Código ya existe'; RAISE ex_neg;
        END IF;
        INSERT INTO ALP_CANAL_VENTA (CVE_CODIGO, CVE_NOMBRE, CVE_DESCRIPCION)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion)
        RETURNING CVE_CANAL_VENTA INTO p_id;
        p_log(f_usuario_sesion, 'ALP_CANAL_VENTA', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Canal de venta creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_CANAL_VENTA(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_CANAL_VENTA(p_id) = 'N' THEN
            p_mensaje := 'Canal de venta no existe'; RAISE ex_neg;
        END IF;
        UPDATE ALP_CANAL_VENTA
        SET CVE_CODIGO = UPPER(TRIM(p_codigo)), CVE_NOMBRE = TRIM(p_nombre), CVE_DESCRIPCION = p_descripcion
        WHERE CVE_CANAL_VENTA = p_id;
        p_log(f_usuario_sesion, 'ALP_CANAL_VENTA', 'UPDATE', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Canal de venta actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_CANAL_VENTA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT CVE_CANAL_VENTA, CVE_CODIGO, CVE_NOMBRE, CVE_DESCRIPCION
            FROM ALP_CANAL_VENTA WHERE CVE_CANAL_VENTA = p_id;
    END;

    PROCEDURE SP_LISTAR_CANALES_VENTA(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT CVE_CANAL_VENTA, CVE_CODIGO, CVE_NOMBRE, CVE_DESCRIPCION
            FROM ALP_CANAL_VENTA ORDER BY CVE_NOMBRE;
    END;

    FUNCTION FN_EXISTE_CANAL_VENTA(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_CANAL_VENTA WHERE CVE_CANAL_VENTA = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_EXISTE_CODIGO_CANAL_VENTA(p_codigo VARCHAR2) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_CANAL_VENTA WHERE CVE_CODIGO = UPPER(TRIM(p_codigo));
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_CATEGORIA_TIPO_DEVOLUCION
    -- =========================================================================

    PROCEDURE SP_CREAR_CAT_TIPO_DEV(
        p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION;
        v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN
            p_mensaje := 'Código y nombre son requeridos'; RAISE ex_neg;
        END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_CATEGORIA_TIPO_DEVOLUCION WHERE CTD_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_CATEGORIA_TIPO_DEVOLUCION (CTD_CODIGO, CTD_NOMBRE, CTD_DESCRIPCION, CTD_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING CTD_CATEGORIA_TIPO_DEV INTO p_id;
        p_log(f_usuario_sesion, 'ALP_CATEGORIA_TIPO_DEVOLUCION', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Categoría de devolución creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_CAT_TIPO_DEV(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_CAT_TIPO_DEV(p_id) = 'N' THEN
            p_mensaje := 'Categoría de devolución no existe'; RAISE ex_neg;
        END IF;
        UPDATE ALP_CATEGORIA_TIPO_DEVOLUCION
        SET CTD_CODIGO = UPPER(TRIM(p_codigo)), CTD_NOMBRE = TRIM(p_nombre), CTD_DESCRIPCION = p_descripcion
        WHERE CTD_CATEGORIA_TIPO_DEV = p_id;
        p_log(f_usuario_sesion, 'ALP_CATEGORIA_TIPO_DEVOLUCION', 'UPDATE', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Categoría de devolución actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_CAT_TIPO_DEV(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN
            p_mensaje := 'Estado inválido'; RAISE ex_neg;
        END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_CAT_TIPO_DEV(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene devoluciones asociadas'; RAISE ex_neg;
        END IF;
        UPDATE ALP_CATEGORIA_TIPO_DEVOLUCION SET CTD_ESTADO = p_estado WHERE CTD_CATEGORIA_TIPO_DEV = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_CATEGORIA_TIPO_DEVOLUCION', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT;
        p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_CAT_TIPO_DEV(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT CTD_CATEGORIA_TIPO_DEV, CTD_CODIGO, CTD_NOMBRE, CTD_DESCRIPCION, CTD_ESTADO
            FROM ALP_CATEGORIA_TIPO_DEVOLUCION WHERE CTD_CATEGORIA_TIPO_DEV = p_id;
    END;

    PROCEDURE SP_LISTAR_CAT_TIPO_DEV(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT CTD_CATEGORIA_TIPO_DEV, CTD_CODIGO, CTD_NOMBRE, CTD_DESCRIPCION, CTD_ESTADO
            FROM ALP_CATEGORIA_TIPO_DEVOLUCION
            WHERE (p_solo_activos != 'S' OR CTD_ESTADO = 'ACTIVO')
            ORDER BY CTD_NOMBRE;
    END;

    FUNCTION FN_EXISTE_CAT_TIPO_DEV(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_CATEGORIA_TIPO_DEVOLUCION WHERE CTD_CATEGORIA_TIPO_DEV = p_id;
        RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END;
    END;

    FUNCTION FN_ACTIVO_CAT_TIPO_DEV(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT CTD_ESTADO INTO v_e FROM ALP_CATEGORIA_TIPO_DEVOLUCION WHERE CTD_CATEGORIA_TIPO_DEV = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_CAT_TIPO_DEV(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_DEVOLUCION WHERE CTD_CATEGORIA_TIPO_DEV = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- UTILIDAD GENÉRICA
    -- =========================================================================

    FUNCTION FN_OBTENER_ID_POR_CODIGO(
        p_tabla VARCHAR2, p_col_pk VARCHAR2, p_col_codigo VARCHAR2, p_codigo VARCHAR2
    ) RETURN NUMBER IS
        v_id   NUMBER;
        v_sql  VARCHAR2(500);
    BEGIN
        v_sql := 'SELECT ' || p_col_pk || ' FROM ' || p_tabla ||
                 ' WHERE ' || p_col_codigo || ' = :1 FETCH FIRST 1 ROWS ONLY';
        EXECUTE IMMEDIATE v_sql INTO v_id USING UPPER(TRIM(p_codigo));
        RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL;
    END;

END PKG_CATALOGOS_GENERALES;
/
-- ============================================================================
-- PKG_UBICACIONES
-- Módulo 3: Catálogos de Ubicación y Medida
-- Tablas: ALP_PAIS, ALP_DEPARTAMENTO, ALP_CIUDAD, ALP_MONEDA,
--         ALP_IDIOMA, ALP_UNIDAD_MEDIDA
-- ============================================================================

CREATE OR REPLACE PACKAGE PKG_UBICACIONES AS

    -- ALP_PAIS
    PROCEDURE SP_CREAR_PAIS         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_PAIS    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_PAIS(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_PAIS       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_PAISES      (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_PAIS        (p_id NUMBER)       RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_PAIS        (p_id NUMBER)       RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_PAIS(p_id NUMBER)      RETURN VARCHAR2;

    -- ALP_DEPARTAMENTO
    PROCEDURE SP_CREAR_DEPARTAMENTO         (p_pais_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_DEPARTAMENTO    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_DEPARTAMENTO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_DEPARTAMENTO       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_DEPARTAMENTOS       (p_pais_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_DEPARTAMENTO        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_DEPARTAMENTO        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2;

    -- ALP_CIUDAD
    PROCEDURE SP_CREAR_CIUDAD         (p_dep_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_CIUDAD    (p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_CIUDAD(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_CIUDAD       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_CIUDADES      (p_dep_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_CIUDAD        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_CIUDAD        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_CIUDAD(p_id NUMBER) RETURN VARCHAR2;

    -- ALP_MONEDA
    PROCEDURE SP_CREAR_MONEDA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_simbolo VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_MONEDA    (p_id NUMBER, p_nombre VARCHAR2, p_simbolo VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_MONEDA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_MONEDA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_MONEDAS       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_MONEDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_MONEDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_MONEDA(p_id NUMBER) RETURN VARCHAR2;

    -- ALP_IDIOMA
    PROCEDURE SP_CREAR_IDIOMA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_IDIOMA    (p_id NUMBER, p_nombre VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_IDIOMA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_IDIOMA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_IDIOMAS       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_IDIOMA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_IDIOMA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_IDIOMA(p_id NUMBER) RETURN VARCHAR2;

    -- ALP_UNIDAD_MEDIDA
    PROCEDURE SP_CREAR_UNIDAD_MEDIDA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_abreviatura VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_UNIDAD_MEDIDA    (p_id NUMBER, p_nombre VARCHAR2, p_abreviatura VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_UNIDAD_MEDIDA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_UNIDAD_MEDIDA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_UNIDADES_MEDIDA      (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_UNIDAD_MEDIDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_UNIDAD_MEDIDA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2;

END PKG_UBICACIONES;
/

CREATE OR REPLACE PACKAGE BODY PKG_UBICACIONES AS

    PROCEDURE p_log(p_usuario_id NUMBER, p_entidad VARCHAR2, p_operacion VARCHAR2, p_id NUMBER, p_datos CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES (p_usuario_id, p_entidad, p_operacion, p_id, p_datos);
        COMMIT;
    END;

    FUNCTION f_usuario_sesion RETURN NUMBER IS
        v_id NUMBER;
    BEGIN
        SELECT USU_USUARIO INTO v_id FROM ALP_USUARIO WHERE USU_USERNAME = USER;
        RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL;
    END;

    -- =========================================================================
    -- ALP_PAIS
    -- =========================================================================

    PROCEDURE SP_CREAR_PAIS(
        p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN p_mensaje := 'Código y nombre requeridos'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_PAIS WHERE PAI_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código de país ya existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_PAIS (PAI_CODIGO, PAI_NOMBRE, PAI_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO')
        RETURNING PAI_PAIS INTO p_id;
        p_log(f_usuario_sesion, 'ALP_PAIS', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'País creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_PAIS(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_PAIS(p_id) = 'N' THEN p_mensaje := 'País no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_PAIS SET PAI_CODIGO = UPPER(TRIM(p_codigo)), PAI_NOMBRE = TRIM(p_nombre) WHERE PAI_PAIS = p_id;
        p_log(f_usuario_sesion, 'ALP_PAIS', 'UPDATE', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'País actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_PAIS(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_PAIS(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene departamentos activos'; RAISE ex_neg;
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
        OPEN p_cursor FOR
            SELECT PAI_PAIS, PAI_CODIGO, PAI_NOMBRE, PAI_ESTADO FROM ALP_PAIS WHERE PAI_PAIS = p_id;
    END;

    PROCEDURE SP_LISTAR_PAISES(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT PAI_PAIS, PAI_CODIGO, PAI_NOMBRE, PAI_ESTADO FROM ALP_PAIS
            WHERE (p_solo_activos != 'S' OR PAI_ESTADO = 'ACTIVO') ORDER BY PAI_NOMBRE;
    END;

    FUNCTION FN_EXISTE_PAIS(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PAIS WHERE PAI_PAIS = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_PAIS(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT PAI_ESTADO INTO v_e FROM ALP_PAIS WHERE PAI_PAIS = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_PAIS(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_DEPARTAMENTO WHERE PAI_PAIS = p_id AND DEP_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_DEPARTAMENTO
    -- =========================================================================

    PROCEDURE SP_CREAR_DEPARTAMENTO(
        p_pais_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN p_mensaje := 'Código y nombre requeridos'; RAISE ex_neg; END IF;
        IF FN_EXISTE_PAIS(p_pais_id) = 'N' THEN p_mensaje := 'País no existe'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_DEPARTAMENTO WHERE PAI_PAIS = p_pais_id AND DEP_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe en este país'; RAISE ex_neg; END IF;

        INSERT INTO ALP_DEPARTAMENTO (PAI_PAIS, DEP_CODIGO, DEP_NOMBRE, DEP_ESTADO)
        VALUES (p_pais_id, UPPER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO')
        RETURNING DEP_DEPARTAMENTO INTO p_id;
        p_log(f_usuario_sesion, 'ALP_DEPARTAMENTO', 'INSERT', p_id,
              JSON_OBJECT('pais_id' VALUE p_pais_id, 'codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Departamento creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_DEPARTAMENTO(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_DEPARTAMENTO(p_id) = 'N' THEN p_mensaje := 'Departamento no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_DEPARTAMENTO SET DEP_CODIGO = UPPER(TRIM(p_codigo)), DEP_NOMBRE = TRIM(p_nombre) WHERE DEP_DEPARTAMENTO = p_id;
        p_log(f_usuario_sesion, 'ALP_DEPARTAMENTO', 'UPDATE', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Departamento actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_DEPARTAMENTO(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_DEPARTAMENTO(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene ciudades activas'; RAISE ex_neg;
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
        OPEN p_cursor FOR
            SELECT d.DEP_DEPARTAMENTO, d.PAI_PAIS, p.PAI_NOMBRE, d.DEP_CODIGO, d.DEP_NOMBRE, d.DEP_ESTADO
            FROM ALP_DEPARTAMENTO d JOIN ALP_PAIS p ON d.PAI_PAIS = p.PAI_PAIS
            WHERE d.DEP_DEPARTAMENTO = p_id;
    END;

    PROCEDURE SP_LISTAR_DEPARTAMENTOS(p_pais_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT DEP_DEPARTAMENTO, PAI_PAIS, DEP_CODIGO, DEP_NOMBRE, DEP_ESTADO
            FROM ALP_DEPARTAMENTO
            WHERE PAI_PAIS = p_pais_id AND (p_solo_activos != 'S' OR DEP_ESTADO = 'ACTIVO')
            ORDER BY DEP_NOMBRE;
    END;

    FUNCTION FN_EXISTE_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_DEPARTAMENTO WHERE DEP_DEPARTAMENTO = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT DEP_ESTADO INTO v_e FROM ALP_DEPARTAMENTO WHERE DEP_DEPARTAMENTO = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_DEPARTAMENTO(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_CIUDAD WHERE DEP_DEPARTAMENTO = p_id AND CIU_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_CIUDAD
    -- =========================================================================

    PROCEDURE SP_CREAR_CIUDAD(
        p_dep_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF FN_EXISTE_DEPARTAMENTO(p_dep_id) = 'N' THEN p_mensaje := 'Departamento no existe'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_CIUDAD WHERE DEP_DEPARTAMENTO = p_dep_id AND CIU_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe en este departamento'; RAISE ex_neg; END IF;

        INSERT INTO ALP_CIUDAD (DEP_DEPARTAMENTO, CIU_CODIGO, CIU_NOMBRE, CIU_ESTADO)
        VALUES (p_dep_id, UPPER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO')
        RETURNING CIU_CIUDAD INTO p_id;
        p_log(f_usuario_sesion, 'ALP_CIUDAD', 'INSERT', p_id,
              JSON_OBJECT('dep_id' VALUE p_dep_id, 'codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Ciudad creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_CIUDAD(
        p_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_CIUDAD(p_id) = 'N' THEN p_mensaje := 'Ciudad no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_CIUDAD SET CIU_CODIGO = UPPER(TRIM(p_codigo)), CIU_NOMBRE = TRIM(p_nombre) WHERE CIU_CIUDAD = p_id;
        p_log(f_usuario_sesion, 'ALP_CIUDAD', 'UPDATE', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Ciudad actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_CIUDAD(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_CIUDAD(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene clientes o órdenes asociadas'; RAISE ex_neg;
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
        OPEN p_cursor FOR
            SELECT c.CIU_CIUDAD, c.DEP_DEPARTAMENTO, d.DEP_NOMBRE, c.CIU_CODIGO, c.CIU_NOMBRE, c.CIU_ESTADO
            FROM ALP_CIUDAD c JOIN ALP_DEPARTAMENTO d ON c.DEP_DEPARTAMENTO = d.DEP_DEPARTAMENTO
            WHERE c.CIU_CIUDAD = p_id;
    END;

    PROCEDURE SP_LISTAR_CIUDADES(p_dep_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT CIU_CIUDAD, DEP_DEPARTAMENTO, CIU_CODIGO, CIU_NOMBRE, CIU_ESTADO
            FROM ALP_CIUDAD
            WHERE DEP_DEPARTAMENTO = p_dep_id AND (p_solo_activos != 'S' OR CIU_ESTADO = 'ACTIVO')
            ORDER BY CIU_NOMBRE;
    END;

    FUNCTION FN_EXISTE_CIUDAD(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_CIUDAD WHERE CIU_CIUDAD = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_CIUDAD(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT CIU_ESTADO INTO v_e FROM ALP_CIUDAD WHERE CIU_CIUDAD = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_CIUDAD(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_CLIENTE_DIRECCION WHERE CIU_CIUDAD = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_MONEDA
    -- =========================================================================

    PROCEDURE SP_CREAR_MONEDA(
        p_codigo VARCHAR2, p_nombre VARCHAR2, p_simbolo VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF LENGTH(TRIM(p_codigo)) != 3 THEN p_mensaje := 'Código ISO debe tener 3 caracteres'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_MONEDA WHERE MON_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código de moneda ya existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_MONEDA (MON_CODIGO, MON_NOMBRE, MON_SIMBOLO, MON_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_simbolo, 'ACTIVO')
        RETURNING MON_MONEDA INTO p_id;
        p_log(f_usuario_sesion, 'ALP_MONEDA', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Moneda creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_MONEDA(
        p_id NUMBER, p_nombre VARCHAR2, p_simbolo VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_MONEDA(p_id) = 'N' THEN p_mensaje := 'Moneda no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_MONEDA SET MON_NOMBRE = TRIM(p_nombre), MON_SIMBOLO = p_simbolo WHERE MON_MONEDA = p_id;
        p_log(f_usuario_sesion, 'ALP_MONEDA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Moneda actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_MONEDA(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_MONEDA(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene precios asociados'; RAISE ex_neg;
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
    BEGIN
        OPEN p_cursor FOR SELECT MON_MONEDA, MON_CODIGO, MON_NOMBRE, MON_SIMBOLO, MON_ESTADO FROM ALP_MONEDA WHERE MON_MONEDA = p_id;
    END;

    PROCEDURE SP_LISTAR_MONEDAS(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT MON_MONEDA, MON_CODIGO, MON_NOMBRE, MON_SIMBOLO, MON_ESTADO FROM ALP_MONEDA
            WHERE (p_solo_activos != 'S' OR MON_ESTADO = 'ACTIVO') ORDER BY MON_NOMBRE;
    END;

    FUNCTION FN_EXISTE_MONEDA(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_MONEDA WHERE MON_MONEDA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_MONEDA(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT MON_ESTADO INTO v_e FROM ALP_MONEDA WHERE MON_MONEDA = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_MONEDA(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_PRECIO WHERE MON_MONEDA = p_id AND PPR_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_IDIOMA
    -- =========================================================================

    PROCEDURE SP_CREAR_IDIOMA(
        p_codigo VARCHAR2, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN p_mensaje := 'Código y nombre requeridos'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_IDIOMA WHERE IDI_CODIGO = LOWER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código de idioma ya existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_IDIOMA (IDI_CODIGO, IDI_NOMBRE, IDI_ESTADO)
        VALUES (LOWER(TRIM(p_codigo)), TRIM(p_nombre), 'ACTIVO')
        RETURNING IDI_IDIOMA INTO p_id;
        p_log(f_usuario_sesion, 'ALP_IDIOMA', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Idioma creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_IDIOMA(
        p_id NUMBER, p_nombre VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_IDIOMA(p_id) = 'N' THEN p_mensaje := 'Idioma no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_IDIOMA SET IDI_NOMBRE = TRIM(p_nombre) WHERE IDI_IDIOMA = p_id;
        p_log(f_usuario_sesion, 'ALP_IDIOMA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Idioma actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_IDIOMA(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_IDIOMA(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene traducciones de productos'; RAISE ex_neg;
        END IF;
        UPDATE ALP_IDIOMA SET IDI_ESTADO = p_estado WHERE IDI_IDIOMA = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_IDIOMA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_IDIOMA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR SELECT IDI_IDIOMA, IDI_CODIGO, IDI_NOMBRE, IDI_ESTADO FROM ALP_IDIOMA WHERE IDI_IDIOMA = p_id;
    END;

    PROCEDURE SP_LISTAR_IDIOMAS(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT IDI_IDIOMA, IDI_CODIGO, IDI_NOMBRE, IDI_ESTADO FROM ALP_IDIOMA
            WHERE (p_solo_activos != 'S' OR IDI_ESTADO = 'ACTIVO') ORDER BY IDI_NOMBRE;
    END;

    FUNCTION FN_EXISTE_IDIOMA(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_IDIOMA WHERE IDI_IDIOMA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_IDIOMA(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT IDI_ESTADO INTO v_e FROM ALP_IDIOMA WHERE IDI_IDIOMA = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_IDIOMA(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_IDIOMA WHERE IDI_IDIOMA = p_id AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_UNIDAD_MEDIDA
    -- =========================================================================

    PROCEDURE SP_CREAR_UNIDAD_MEDIDA(
        p_codigo VARCHAR2, p_nombre VARCHAR2, p_abreviatura VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER
    ) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_codigo IS NULL OR p_nombre IS NULL THEN p_mensaje := 'Código y nombre requeridos'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_UNIDAD_MEDIDA WHERE UNI_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código de unidad ya existe'; RAISE ex_neg; END IF;

        INSERT INTO ALP_UNIDAD_MEDIDA (UNI_CODIGO, UNI_NOMBRE, UNI_ABREVIATURA, UNI_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_abreviatura, 'ACTIVO')
        RETURNING UNI_UNIDAD_MEDIDA INTO p_id;
        p_log(f_usuario_sesion, 'ALP_UNIDAD_MEDIDA', 'INSERT', p_id,
              JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Unidad de medida creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_UNIDAD_MEDIDA(
        p_id NUMBER, p_nombre VARCHAR2, p_abreviatura VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_UNIDAD_MEDIDA(p_id) = 'N' THEN p_mensaje := 'Unidad de medida no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_UNIDAD_MEDIDA SET UNI_NOMBRE = TRIM(p_nombre), UNI_ABREVIATURA = p_abreviatura WHERE UNI_UNIDAD_MEDIDA = p_id;
        p_log(f_usuario_sesion, 'ALP_UNIDAD_MEDIDA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Unidad de medida actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_UNIDAD_MEDIDA(
        p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2
    ) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_UNIDAD_MEDIDA(p_id) = 'N' THEN
            p_mensaje := 'No se puede inactivar: tiene productos asociados'; RAISE ex_neg;
        END IF;
        UPDATE ALP_UNIDAD_MEDIDA SET UNI_ESTADO = p_estado WHERE UNI_UNIDAD_MEDIDA = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_UNIDAD_MEDIDA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_UNIDAD_MEDIDA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT UNI_UNIDAD_MEDIDA, UNI_CODIGO, UNI_NOMBRE, UNI_ABREVIATURA, UNI_ESTADO
            FROM ALP_UNIDAD_MEDIDA WHERE UNI_UNIDAD_MEDIDA = p_id;
    END;

    PROCEDURE SP_LISTAR_UNIDADES_MEDIDA(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN
        OPEN p_cursor FOR
            SELECT UNI_UNIDAD_MEDIDA, UNI_CODIGO, UNI_NOMBRE, UNI_ABREVIATURA, UNI_ESTADO
            FROM ALP_UNIDAD_MEDIDA
            WHERE (p_solo_activos != 'S' OR UNI_ESTADO = 'ACTIVO') ORDER BY UNI_NOMBRE;
    END;

    FUNCTION FN_EXISTE_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_UNIDAD_MEDIDA WHERE UNI_UNIDAD_MEDIDA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2 IS
        v_e VARCHAR2(20);
    BEGIN
        SELECT UNI_ESTADO INTO v_e FROM ALP_UNIDAD_MEDIDA WHERE UNI_UNIDAD_MEDIDA = p_id;
        RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N';
    END;

    FUNCTION FN_PUEDE_INACTIVAR_UNIDAD_MEDIDA(p_id NUMBER) RETURN VARCHAR2 IS
        v_c NUMBER;
    BEGIN
        -- ALP_PRODUCTO referencia UNI_UNIDAD_MEDIDA implícitamente en dimensiones
        SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO WHERE PRO_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        -- Conservador: si existen productos activos no inactivar
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

END PKG_UBICACIONES;
/
-- ============================================================================
-- PKG_CATALOGOS_PRODUCTOS
-- Módulo 3/4: Catálogos de Productos e Inventario
-- Tablas: ALP_MATERIAL, ALP_COLOR, ALP_CATEGORIA, ALP_COLECCION,
--         ALP_ATRIBUTO, ALP_ATRIBUTO_VALOR, ALP_TIPO_MUEBLE,
--         ALP_BODEGA, ALP_ZONA_BODEGA
-- ============================================================================

CREATE OR REPLACE PACKAGE PKG_CATALOGOS_PRODUCTOS AS

    PROCEDURE SP_CREAR_MATERIAL         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_MATERIAL    (p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_MATERIAL(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_MATERIAL       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_MATERIALES      (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_MATERIAL        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_MATERIAL        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_MATERIAL(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_COLOR         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_hex VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_COLOR    (p_id NUMBER, p_nombre VARCHAR2, p_hex VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_COLOR(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_COLOR       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_COLORES      (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_COLOR        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_COLOR        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_COLOR(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_CATEGORIA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_CATEGORIA    (p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_CATEGORIA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_CATEGORIA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_CATEGORIAS       (p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_CATEGORIA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_CATEGORIA(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_COLECCION         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_fecha_inicio DATE, p_fecha_fin DATE, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_COLECCION    (p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_fecha_inicio DATE, p_fecha_fin DATE, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_COLECCION(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_COLECCION       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_COLECCIONES      (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_COLECCION        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_COLECCION        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_COLECCION(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_ATRIBUTO         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_tipo VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_ATRIBUTO    (p_id NUMBER, p_nombre VARCHAR2, p_tipo VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_ATRIBUTO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_ATRIBUTO       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_ATRIBUTOS       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_ATRIBUTO        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_ATRIBUTO        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_ATRIBUTO(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_ATRIBUTO_VALOR         (p_atributo_id NUMBER, p_valor VARCHAR2, p_hex VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_ATRIBUTO_VALOR    (p_id NUMBER, p_valor VARCHAR2, p_hex VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_ATRIBUTO_VALOR(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_ATRIBUTO_VALOR       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_ATRIBUTO_VALORES      (p_atributo_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_ATRIBUTO_VALOR        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_ATRIBUTO_VALOR        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_ATRIBUTO_VALOR(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_TIPO_MUEBLE         (p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_TIPO_MUEBLE    (p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_TIPO_MUEBLE       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_TIPOS_MUEBLE       (p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_TIPO_MUEBLE        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_TIPO_MUEBLE(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_BODEGA         (p_codigo VARCHAR2, p_nombre VARCHAR2, p_direccion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_BODEGA    (p_id NUMBER, p_nombre VARCHAR2, p_direccion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_BODEGA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_BODEGA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_BODEGAS       (p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_BODEGA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_BODEGA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_BODEGA(p_id NUMBER) RETURN VARCHAR2;

    PROCEDURE SP_CREAR_ZONA_BODEGA         (p_bodega_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER);
    PROCEDURE SP_ACTUALIZAR_ZONA_BODEGA    (p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_CAMBIAR_ESTADO_ZONA_BODEGA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2);
    PROCEDURE SP_OBTENER_ZONA_BODEGA       (p_id NUMBER, p_cursor OUT SYS_REFCURSOR);
    PROCEDURE SP_LISTAR_ZONAS_BODEGA       (p_bodega_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR);
    FUNCTION  FN_EXISTE_ZONA_BODEGA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_ACTIVO_ZONA_BODEGA        (p_id NUMBER) RETURN VARCHAR2;
    FUNCTION  FN_PUEDE_INACTIVAR_ZONA_BODEGA(p_id NUMBER) RETURN VARCHAR2;

END PKG_CATALOGOS_PRODUCTOS;
/

CREATE OR REPLACE PACKAGE BODY PKG_CATALOGOS_PRODUCTOS AS

    PROCEDURE p_log(p_uid NUMBER, p_entidad VARCHAR2, p_op VARCHAR2, p_id NUMBER, p_datos CLOB) IS
        PRAGMA AUTONOMOUS_TRANSACTION;
    BEGIN
        INSERT INTO ALP_TRANSACCION_LOG (USU_USUARIO, TRL_ENTIDAD, TRL_OPERACION, TRL_REGISTRO_ID, TRL_DATOS_NUEVOS)
        VALUES (p_uid, p_entidad, p_op, p_id, p_datos);
        COMMIT;
    END;

    FUNCTION f_uid RETURN NUMBER IS v_id NUMBER;
    BEGIN SELECT USU_USUARIO INTO v_id FROM ALP_USUARIO WHERE USU_USERNAME = USER;
          RETURN v_id;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN NULL; END;

    -- =========================================================================
    -- ALP_MATERIAL
    -- =========================================================================

    PROCEDURE SP_CREAR_MATERIAL(p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_MATERIAL WHERE MAT_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_MATERIAL (MAT_CODIGO, MAT_NOMBRE, MAT_DESCRIPCION, MAT_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING MAT_MATERIAL INTO p_id;
        p_log(f_uid, 'ALP_MATERIAL', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Material creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_MATERIAL(p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_MATERIAL(p_id) = 'N' THEN p_mensaje := 'Material no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_MATERIAL SET MAT_NOMBRE = TRIM(p_nombre), MAT_DESCRIPCION = p_descripcion WHERE MAT_MATERIAL = p_id;
        p_log(f_uid, 'ALP_MATERIAL', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Material actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_MATERIAL(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO','DESCONTINUADO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado != 'ACTIVO' AND FN_PUEDE_INACTIVAR_MATERIAL(p_id) = 'N' THEN
            p_mensaje := 'Tiene productos activos asociados'; RAISE ex_neg;
        END IF;
        UPDATE ALP_MATERIAL SET MAT_ESTADO = p_estado WHERE MAT_MATERIAL = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_MATERIAL', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_MATERIAL(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT MAT_MATERIAL, MAT_CODIGO, MAT_NOMBRE, MAT_DESCRIPCION, MAT_ESTADO FROM ALP_MATERIAL WHERE MAT_MATERIAL = p_id; END;

    PROCEDURE SP_LISTAR_MATERIALES(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT MAT_MATERIAL, MAT_CODIGO, MAT_NOMBRE, MAT_ESTADO FROM ALP_MATERIAL WHERE (p_solo_activos != 'S' OR MAT_ESTADO = 'ACTIVO') ORDER BY MAT_NOMBRE; END;

    FUNCTION FN_EXISTE_MATERIAL(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_MATERIAL WHERE MAT_MATERIAL = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_MATERIAL(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT MAT_ESTADO INTO v_e FROM ALP_MATERIAL WHERE MAT_MATERIAL = p_id; RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_MATERIAL(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_MATERIAL WHERE MAT_MATERIAL = p_id AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_COLOR
    -- =========================================================================

    PROCEDURE SP_CREAR_COLOR(p_codigo VARCHAR2, p_nombre VARCHAR2, p_hex VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_hex NOT LIKE '#%' THEN p_mensaje := 'Hex debe iniciar con #'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_COLOR WHERE COL_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_COLOR (COL_CODIGO, COL_NOMBRE, COL_CODIGO_HEX, COL_DESCRIPCION, COL_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_hex, p_descripcion, 'ACTIVO')
        RETURNING COL_COLOR INTO p_id;
        p_log(f_uid, 'ALP_COLOR', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo, 'hex' VALUE p_hex));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Color creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_COLOR(p_id NUMBER, p_nombre VARCHAR2, p_hex VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_COLOR(p_id) = 'N' THEN p_mensaje := 'Color no existe'; RAISE ex_neg; END IF;
        IF p_hex IS NOT NULL AND p_hex NOT LIKE '#%' THEN p_mensaje := 'Hex debe iniciar con #'; RAISE ex_neg; END IF;
        UPDATE ALP_COLOR SET COL_NOMBRE = TRIM(p_nombre), COL_CODIGO_HEX = NVL(p_hex, COL_CODIGO_HEX), COL_DESCRIPCION = p_descripcion WHERE COL_COLOR = p_id;
        p_log(f_uid, 'ALP_COLOR', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Color actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_COLOR(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_COLOR(p_id) = 'N' THEN
            p_mensaje := 'Tiene productos activos asociados'; RAISE ex_neg;
        END IF;
        UPDATE ALP_COLOR SET COL_ESTADO = p_estado WHERE COL_COLOR = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_COLOR', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_COLOR(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT COL_COLOR, COL_CODIGO, COL_NOMBRE, COL_CODIGO_HEX, COL_DESCRIPCION, COL_ESTADO FROM ALP_COLOR WHERE COL_COLOR = p_id; END;

    PROCEDURE SP_LISTAR_COLORES(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT COL_COLOR, COL_CODIGO, COL_NOMBRE, COL_CODIGO_HEX, COL_ESTADO FROM ALP_COLOR WHERE (p_solo_activos != 'S' OR COL_ESTADO = 'ACTIVO') ORDER BY COL_NOMBRE; END;

    FUNCTION FN_EXISTE_COLOR(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_COLOR WHERE COL_COLOR = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_COLOR(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT COL_ESTADO INTO v_e FROM ALP_COLOR WHERE COL_COLOR = p_id; RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_COLOR(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_COLOR WHERE COL_COLOR = p_id AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_CATEGORIA
    -- =========================================================================

    PROCEDURE SP_CREAR_CATEGORIA(p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_CATEGORIA WHERE CAT_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_CATEGORIA (CAT_CODIGO, CAT_NOMBRE, CAT_DESCRIPCION)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion)
        RETURNING CAT_CATEGORIA INTO p_id;
        p_log(f_uid, 'ALP_CATEGORIA', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Categoría creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_CATEGORIA(p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_CATEGORIA(p_id) = 'N' THEN p_mensaje := 'Categoría no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_CATEGORIA SET CAT_NOMBRE = TRIM(p_nombre), CAT_DESCRIPCION = p_descripcion WHERE CAT_CATEGORIA = p_id;
        p_log(f_uid, 'ALP_CATEGORIA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Categoría actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_CATEGORIA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        -- ALP_CATEGORIA no tiene columna de estado en DDL; validación de negocio vía productos
        IF FN_PUEDE_INACTIVAR_CATEGORIA(p_id) = 'N' THEN
            p_mensaje := 'Tiene productos activos en esta categoría'; RAISE ex_neg;
        END IF;
        -- Soft-delete lógico usando eliminación de relaciones si se requiere
        p_log(p_usuario_id, 'ALP_CATEGORIA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Operación registrada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_CATEGORIA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT CAT_CATEGORIA, CAT_CODIGO, CAT_NOMBRE, CAT_DESCRIPCION FROM ALP_CATEGORIA WHERE CAT_CATEGORIA = p_id; END;

    PROCEDURE SP_LISTAR_CATEGORIAS(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT CAT_CATEGORIA, CAT_CODIGO, CAT_NOMBRE, CAT_DESCRIPCION FROM ALP_CATEGORIA ORDER BY CAT_NOMBRE; END;

    FUNCTION FN_EXISTE_CATEGORIA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_CATEGORIA WHERE CAT_CATEGORIA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_PUEDE_INACTIVAR_CATEGORIA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_CATEGORIA WHERE CAT_CATEGORIA = p_id AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_COLECCION
    -- =========================================================================

    PROCEDURE SP_CREAR_COLECCION(p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_fecha_inicio DATE, p_fecha_fin DATE,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_fecha_fin IS NOT NULL AND p_fecha_fin < p_fecha_inicio THEN
            p_mensaje := 'Fecha fin no puede ser menor a fecha inicio'; RAISE ex_neg;
        END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_COLECCION WHERE COL_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_COLECCION (COL_CODIGO, COL_NOMBRE, COL_DESCRIPCION, COL_FECHA_INICIO, COL_FECHA_FIN, COL_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, p_fecha_inicio, p_fecha_fin, 'ACTIVO')
        RETURNING COL_COLECCION INTO p_id;
        p_log(f_uid, 'ALP_COLECCION', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Colección creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_COLECCION(p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_fecha_inicio DATE, p_fecha_fin DATE, p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_COLECCION(p_id) = 'N' THEN p_mensaje := 'Colección no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_COLECCION SET COL_NOMBRE = TRIM(p_nombre), COL_DESCRIPCION = p_descripcion,
            COL_FECHA_INICIO = p_fecha_inicio, COL_FECHA_FIN = p_fecha_fin WHERE COL_COLECCION = p_id;
        p_log(f_uid, 'ALP_COLECCION', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Colección actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_COLECCION(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO','DESCONTINUADO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado != 'ACTIVO' AND FN_PUEDE_INACTIVAR_COLECCION(p_id) = 'N' THEN
            p_mensaje := 'Tiene productos activos en esta colección'; RAISE ex_neg;
        END IF;
        UPDATE ALP_COLECCION SET COL_ESTADO = p_estado WHERE COL_COLECCION = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_COLECCION', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_COLECCION(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT COL_COLECCION, COL_CODIGO, COL_NOMBRE, COL_DESCRIPCION, COL_FECHA_INICIO, COL_FECHA_FIN, COL_ESTADO FROM ALP_COLECCION WHERE COL_COLECCION = p_id; END;

    PROCEDURE SP_LISTAR_COLECCIONES(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT COL_COLECCION, COL_CODIGO, COL_NOMBRE, COL_ESTADO, COL_FECHA_FIN FROM ALP_COLECCION WHERE (p_solo_activos != 'S' OR COL_ESTADO = 'ACTIVO') ORDER BY COL_NOMBRE; END;

    FUNCTION FN_EXISTE_COLECCION(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_COLECCION WHERE COL_COLECCION = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_COLECCION(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT COL_ESTADO INTO v_e FROM ALP_COLECCION WHERE COL_COLECCION = p_id; RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_COLECCION(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO_COLECCION WHERE COL_COLECCION = p_id AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_ATRIBUTO
    -- =========================================================================

    PROCEDURE SP_CREAR_ATRIBUTO(p_codigo VARCHAR2, p_nombre VARCHAR2, p_tipo VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF p_tipo NOT IN ('COLOR','TAMANO','MATERIAL','ACABADO','ESTILO','OTRO') THEN
            p_mensaje := 'Tipo inválido'; RAISE ex_neg;
        END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_ATRIBUTO WHERE ATR_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_ATRIBUTO (ATR_CODIGO, ATR_NOMBRE, ATR_TIPO, ATR_DESCRIPCION, ATR_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_tipo, p_descripcion, 'ACTIVO')
        RETURNING ATR_ATRIBUTO INTO p_id;
        p_log(f_uid, 'ALP_ATRIBUTO', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo, 'tipo' VALUE p_tipo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Atributo creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_ATRIBUTO(p_id NUMBER, p_nombre VARCHAR2, p_tipo VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_ATRIBUTO(p_id) = 'N' THEN p_mensaje := 'Atributo no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_ATRIBUTO SET ATR_NOMBRE = TRIM(p_nombre), ATR_TIPO = p_tipo, ATR_DESCRIPCION = p_descripcion WHERE ATR_ATRIBUTO = p_id;
        p_log(f_uid, 'ALP_ATRIBUTO', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Atributo actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_ATRIBUTO(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_ATRIBUTO(p_id) = 'N' THEN
            p_mensaje := 'Tiene variantes activas con este atributo'; RAISE ex_neg;
        END IF;
        UPDATE ALP_ATRIBUTO SET ATR_ESTADO = p_estado WHERE ATR_ATRIBUTO = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_ATRIBUTO', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_ATRIBUTO(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT ATR_ATRIBUTO, ATR_CODIGO, ATR_NOMBRE, ATR_TIPO, ATR_DESCRIPCION, ATR_ESTADO FROM ALP_ATRIBUTO WHERE ATR_ATRIBUTO = p_id; END;

    PROCEDURE SP_LISTAR_ATRIBUTOS(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT ATR_ATRIBUTO, ATR_CODIGO, ATR_NOMBRE, ATR_TIPO, ATR_ESTADO FROM ALP_ATRIBUTO WHERE (p_solo_activos != 'S' OR ATR_ESTADO = 'ACTIVO') ORDER BY ATR_NOMBRE; END;

    FUNCTION FN_EXISTE_ATRIBUTO(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_ATRIBUTO WHERE ATR_ATRIBUTO = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_ATRIBUTO(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT ATR_ESTADO INTO v_e FROM ALP_ATRIBUTO WHERE ATR_ATRIBUTO = p_id; RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_ATRIBUTO(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_ATRIBUTO_VALOR WHERE ATR_ATRIBUTO = p_id AND ATV_ESTADO = 'ACTIVO' AND ROWNUM = 1;
        RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END;
    END;

    -- =========================================================================
    -- ALP_ATRIBUTO_VALOR
    -- =========================================================================

    PROCEDURE SP_CREAR_ATRIBUTO_VALOR(p_atributo_id NUMBER, p_valor VARCHAR2, p_hex VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF FN_EXISTE_ATRIBUTO(p_atributo_id) = 'N' THEN p_mensaje := 'Atributo no existe'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_ATRIBUTO_VALOR WHERE ATR_ATRIBUTO = p_atributo_id AND ATV_VALOR = TRIM(p_valor);
        IF v_c > 0 THEN p_mensaje := 'Valor ya existe para este atributo'; RAISE ex_neg; END IF;
        INSERT INTO ALP_ATRIBUTO_VALOR (ATR_ATRIBUTO, ATV_VALOR, ATV_CODIGO_HEX, ATV_ESTADO)
        VALUES (p_atributo_id, TRIM(p_valor), p_hex, 'ACTIVO')
        RETURNING ATV_ATRIBUTO_VALOR INTO p_id;
        p_log(f_uid, 'ALP_ATRIBUTO_VALOR', 'INSERT', p_id, JSON_OBJECT('atributo_id' VALUE p_atributo_id, 'valor' VALUE p_valor));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Valor de atributo creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_ATRIBUTO_VALOR(p_id NUMBER, p_valor VARCHAR2, p_hex VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_ATRIBUTO_VALOR(p_id) = 'N' THEN p_mensaje := 'Valor de atributo no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_ATRIBUTO_VALOR SET ATV_VALOR = TRIM(p_valor), ATV_CODIGO_HEX = p_hex WHERE ATV_ATRIBUTO_VALOR = p_id;
        p_log(f_uid, 'ALP_ATRIBUTO_VALOR', 'UPDATE', p_id, JSON_OBJECT('valor' VALUE p_valor));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Valor actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_ATRIBUTO_VALOR(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_ATRIBUTO_VALOR(p_id) = 'N' THEN
            p_mensaje := 'Tiene variantes que usan este valor'; RAISE ex_neg;
        END IF;
        UPDATE ALP_ATRIBUTO_VALOR SET ATV_ESTADO = p_estado WHERE ATV_ATRIBUTO_VALOR = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_ATRIBUTO_VALOR', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_ATRIBUTO_VALOR(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT ATV_ATRIBUTO_VALOR, ATR_ATRIBUTO, ATV_VALOR, ATV_CODIGO_HEX, ATV_ESTADO FROM ALP_ATRIBUTO_VALOR WHERE ATV_ATRIBUTO_VALOR = p_id; END;

    PROCEDURE SP_LISTAR_ATRIBUTO_VALORES(p_atributo_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT ATV_ATRIBUTO_VALOR, ATR_ATRIBUTO, ATV_VALOR, ATV_CODIGO_HEX, ATV_ESTADO FROM ALP_ATRIBUTO_VALOR WHERE ATR_ATRIBUTO = p_atributo_id AND (p_solo_activos != 'S' OR ATV_ESTADO = 'ACTIVO') ORDER BY ATV_VALOR; END;

    FUNCTION FN_EXISTE_ATRIBUTO_VALOR(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_ATRIBUTO_VALOR WHERE ATV_ATRIBUTO_VALOR = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_ATRIBUTO_VALOR(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT ATV_ESTADO INTO v_e FROM ALP_ATRIBUTO_VALOR WHERE ATV_ATRIBUTO_VALOR = p_id; RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_ATRIBUTO_VALOR(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_VARIANTE_ATRIBUTO_VALOR WHERE ATV_ATRIBUTO_VALOR = p_id AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_TIPO_MUEBLE
    -- =========================================================================

    PROCEDURE SP_CREAR_TIPO_MUEBLE(p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_TIPO_MUEBLE WHERE TMU_NOMBRE = TRIM(p_nombre);
        IF v_c > 0 THEN p_mensaje := 'Nombre ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_TIPO_MUEBLE (TMU_NOMBRE, TMU_DESCRIPCION)
        VALUES (TRIM(p_nombre), p_descripcion)
        RETURNING TMU_TIPO_MUEBLE INTO p_id;
        p_log(f_uid, 'ALP_TIPO_MUEBLE', 'INSERT', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Tipo de mueble creado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_TIPO_MUEBLE(p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_TIPO_MUEBLE(p_id) = 'N' THEN p_mensaje := 'Tipo de mueble no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_TIPO_MUEBLE SET TMU_NOMBRE = TRIM(p_nombre), TMU_DESCRIPCION = p_descripcion WHERE TMU_TIPO_MUEBLE = p_id;
        p_log(f_uid, 'ALP_TIPO_MUEBLE', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Tipo de mueble actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_TIPO_MUEBLE(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT TMU_TIPO_MUEBLE, TMU_NOMBRE, TMU_DESCRIPCION FROM ALP_TIPO_MUEBLE WHERE TMU_TIPO_MUEBLE = p_id; END;

    PROCEDURE SP_LISTAR_TIPOS_MUEBLE(p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT TMU_TIPO_MUEBLE, TMU_NOMBRE, TMU_DESCRIPCION FROM ALP_TIPO_MUEBLE ORDER BY TMU_NOMBRE; END;

    FUNCTION FN_EXISTE_TIPO_MUEBLE(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_TIPO_MUEBLE WHERE TMU_TIPO_MUEBLE = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_PUEDE_INACTIVAR_TIPO_MUEBLE(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_PRODUCTO WHERE TMU_TIPO_MUEBLE = p_id AND PRO_ESTADO = 'ACTIVO' AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_BODEGA
    -- =========================================================================

    PROCEDURE SP_CREAR_BODEGA(p_codigo VARCHAR2, p_nombre VARCHAR2, p_direccion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        SELECT COUNT(1) INTO v_c FROM ALP_BODEGA WHERE BOD_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe'; RAISE ex_neg; END IF;
        INSERT INTO ALP_BODEGA (BOD_CODIGO, BOD_NOMBRE, BOD_DIRECCION, BOD_ESTADO)
        VALUES (UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_direccion, 'ACTIVO')
        RETURNING BOD_BODEGA INTO p_id;
        p_log(f_uid, 'ALP_BODEGA', 'INSERT', p_id, JSON_OBJECT('codigo' VALUE p_codigo, 'nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Bodega creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_BODEGA(p_id NUMBER, p_nombre VARCHAR2, p_direccion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_BODEGA(p_id) = 'N' THEN p_mensaje := 'Bodega no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_BODEGA SET BOD_NOMBRE = TRIM(p_nombre), BOD_DIRECCION = p_direccion WHERE BOD_BODEGA = p_id;
        p_log(f_uid, 'ALP_BODEGA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Bodega actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_BODEGA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO','MANTENIMIENTO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado = 'INACTIVO' AND FN_PUEDE_INACTIVAR_BODEGA(p_id) = 'N' THEN
            p_mensaje := 'Tiene stock existente en esta bodega'; RAISE ex_neg;
        END IF;
        UPDATE ALP_BODEGA SET BOD_ESTADO = p_estado WHERE BOD_BODEGA = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_BODEGA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_BODEGA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT BOD_BODEGA, BOD_CODIGO, BOD_NOMBRE, BOD_DIRECCION, BOD_ESTADO FROM ALP_BODEGA WHERE BOD_BODEGA = p_id; END;

    PROCEDURE SP_LISTAR_BODEGAS(p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT BOD_BODEGA, BOD_CODIGO, BOD_NOMBRE, BOD_ESTADO FROM ALP_BODEGA WHERE (p_solo_activos != 'S' OR BOD_ESTADO = 'ACTIVO') ORDER BY BOD_NOMBRE; END;

    FUNCTION FN_EXISTE_BODEGA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_BODEGA WHERE BOD_BODEGA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_BODEGA(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT BOD_ESTADO INTO v_e FROM ALP_BODEGA WHERE BOD_BODEGA = p_id; RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_BODEGA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_EXISTENCIA WHERE BOD_BODEGA = p_id AND EXI_CANTIDAD_DISPONIBLE > 0 AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

    -- =========================================================================
    -- ALP_ZONA_BODEGA
    -- =========================================================================

    PROCEDURE SP_CREAR_ZONA_BODEGA(p_bodega_id NUMBER, p_codigo VARCHAR2, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2, p_id OUT NUMBER) IS
        ex_neg EXCEPTION; v_c NUMBER;
    BEGIN
        IF FN_EXISTE_BODEGA(p_bodega_id) = 'N' THEN p_mensaje := 'Bodega no existe'; RAISE ex_neg; END IF;
        SELECT COUNT(1) INTO v_c FROM ALP_ZONA_BODEGA WHERE BOD_BODEGA = p_bodega_id AND ZBO_CODIGO = UPPER(TRIM(p_codigo));
        IF v_c > 0 THEN p_mensaje := 'Código ya existe en esta bodega'; RAISE ex_neg; END IF;
        INSERT INTO ALP_ZONA_BODEGA (BOD_BODEGA, ZBO_CODIGO, ZBO_NOMBRE, ZBO_DESCRIPCION, ZBO_ESTADO)
        VALUES (p_bodega_id, UPPER(TRIM(p_codigo)), TRIM(p_nombre), p_descripcion, 'ACTIVO')
        RETURNING ZBO_ZONA_BODEGA INTO p_id;
        p_log(f_uid, 'ALP_ZONA_BODEGA', 'INSERT', p_id, JSON_OBJECT('bodega_id' VALUE p_bodega_id, 'codigo' VALUE p_codigo));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Zona de bodega creada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR'; p_id := NULL;
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM; p_id := NULL;
    END;

    PROCEDURE SP_ACTUALIZAR_ZONA_BODEGA(p_id NUMBER, p_nombre VARCHAR2, p_descripcion VARCHAR2,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF FN_EXISTE_ZONA_BODEGA(p_id) = 'N' THEN p_mensaje := 'Zona de bodega no existe'; RAISE ex_neg; END IF;
        UPDATE ALP_ZONA_BODEGA SET ZBO_NOMBRE = TRIM(p_nombre), ZBO_DESCRIPCION = p_descripcion WHERE ZBO_ZONA_BODEGA = p_id;
        p_log(f_uid, 'ALP_ZONA_BODEGA', 'UPDATE', p_id, JSON_OBJECT('nombre' VALUE p_nombre));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Zona actualizada';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_CAMBIAR_ESTADO_ZONA_BODEGA(p_id NUMBER, p_estado VARCHAR2, p_usuario_id NUMBER,
        p_resultado OUT VARCHAR2, p_mensaje OUT VARCHAR2) IS
        ex_neg EXCEPTION;
    BEGIN
        IF p_estado NOT IN ('ACTIVO','INACTIVO','BLOQUEADO') THEN p_mensaje := 'Estado inválido'; RAISE ex_neg; END IF;
        IF p_estado != 'ACTIVO' AND FN_PUEDE_INACTIVAR_ZONA_BODEGA(p_id) = 'N' THEN
            p_mensaje := 'Tiene stock activo en esta zona'; RAISE ex_neg;
        END IF;
        UPDATE ALP_ZONA_BODEGA SET ZBO_ESTADO = p_estado WHERE ZBO_ZONA_BODEGA = p_id;
        IF SQL%ROWCOUNT = 0 THEN p_mensaje := 'Registro no encontrado'; RAISE ex_neg; END IF;
        p_log(p_usuario_id, 'ALP_ZONA_BODEGA', 'UPDATE', p_id, JSON_OBJECT('estado' VALUE p_estado));
        COMMIT; p_resultado := 'EXITO'; p_mensaje := 'Estado actualizado';
    EXCEPTION
        WHEN ex_neg THEN ROLLBACK; p_resultado := 'ERROR';
        WHEN OTHERS THEN ROLLBACK; p_resultado := 'ERROR'; p_mensaje := SQLERRM;
    END;

    PROCEDURE SP_OBTENER_ZONA_BODEGA(p_id NUMBER, p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT z.ZBO_ZONA_BODEGA, z.BOD_BODEGA, b.BOD_NOMBRE, z.ZBO_CODIGO, z.ZBO_NOMBRE, z.ZBO_DESCRIPCION, z.ZBO_ESTADO FROM ALP_ZONA_BODEGA z JOIN ALP_BODEGA b ON z.BOD_BODEGA = b.BOD_BODEGA WHERE z.ZBO_ZONA_BODEGA = p_id; END;

    PROCEDURE SP_LISTAR_ZONAS_BODEGA(p_bodega_id NUMBER, p_solo_activos VARCHAR2 DEFAULT 'S', p_cursor OUT SYS_REFCURSOR) IS
    BEGIN OPEN p_cursor FOR SELECT ZBO_ZONA_BODEGA, BOD_BODEGA, ZBO_CODIGO, ZBO_NOMBRE, ZBO_ESTADO FROM ALP_ZONA_BODEGA WHERE BOD_BODEGA = p_bodega_id AND (p_solo_activos != 'S' OR ZBO_ESTADO = 'ACTIVO') ORDER BY ZBO_NOMBRE; END;

    FUNCTION FN_EXISTE_ZONA_BODEGA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_ZONA_BODEGA WHERE ZBO_ZONA_BODEGA = p_id; RETURN CASE WHEN v_c > 0 THEN 'S' ELSE 'N' END; END;

    FUNCTION FN_ACTIVO_ZONA_BODEGA(p_id NUMBER) RETURN VARCHAR2 IS v_e VARCHAR2(20);
    BEGIN SELECT ZBO_ESTADO INTO v_e FROM ALP_ZONA_BODEGA WHERE ZBO_ZONA_BODEGA = p_id; RETURN CASE WHEN v_e = 'ACTIVO' THEN 'S' ELSE 'N' END;
    EXCEPTION WHEN NO_DATA_FOUND THEN RETURN 'N'; END;

    FUNCTION FN_PUEDE_INACTIVAR_ZONA_BODEGA(p_id NUMBER) RETURN VARCHAR2 IS v_c NUMBER;
    BEGIN SELECT COUNT(1) INTO v_c FROM ALP_EXISTENCIA WHERE ZBO_ZONA_BODEGA = p_id AND EXI_CANTIDAD_DISPONIBLE > 0 AND ROWNUM = 1; RETURN CASE WHEN v_c = 0 THEN 'S' ELSE 'N' END; END;

END PKG_CATALOGOS_PRODUCTOS;
/
