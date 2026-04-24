-- ============================================================================
-- PROYECTO: MUEBLES LOS ALPES - BLOQUE 5
-- MOTOR: Oracle Database 21c
-- ============================================================================

-- ============================================================================
-- BLOQUE 5: PRODUCTOS Y CATÁLOGO
-- ============================================================================
-- Descripción: Gestión completa del catálogo de productos
-- Incluye: Productos, Variantes, Atributos, Precios, Imágenes, Reseñas
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Tabla: ALP_TIPO_MUEBLE
-- Descripción: Clasificación general de muebles
-- Ejemplos: 'SALA', 'COMEDOR', 'DORMITORIO', 'OFICINA', 'EXTERIOR'
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_TIPO_MUEBLE (
    TMU_TIPO_MUEBLE          NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    TMU_NOMBRE               VARCHAR2(100) NOT NULL UNIQUE,
    TMU_DESCRIPCION          VARCHAR2(500),
    
    CONSTRAINT CHK_TMU_NOMBRE CHECK (LENGTH(TMU_NOMBRE) >= 3)
);

COMMENT ON TABLE ALP_TIPO_MUEBLE IS 'Clasificación general de tipos de muebles';

-- ----------------------------------------------------------------------------
-- Tabla: ALP_CATEGORIA
-- Descripción: Categorías de productos (taxonomía)
-- Ejemplos: 'Sofás', 'Mesas', 'Sillas', 'Armarios'
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_CATEGORIA (
    CAT_CATEGORIA            NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    CAT_CODIGO               VARCHAR2(50) NOT NULL UNIQUE,
    CAT_NOMBRE               VARCHAR2(150) NOT NULL,
    CAT_DESCRIPCION          VARCHAR2(500),
    
    CONSTRAINT CHK_CAT_CODIGO CHECK (LENGTH(CAT_CODIGO) >= 3)
);

COMMENT ON TABLE ALP_CATEGORIA IS 'Categorías de productos (jerarquía de clasificación)';

-- ----------------------------------------------------------------------------
-- Tabla: ALP_COLECCION
-- Descripción: Colecciones o líneas de productos
-- Ejemplos: 'Vintage 2026', 'Moderna Escandinava', 'Rústica Premium'
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_COLECCION (
    COL_COLECCION            NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    COL_CODIGO               VARCHAR2(50) NOT NULL UNIQUE,
    COL_NOMBRE               VARCHAR2(150) NOT NULL,
    COL_DESCRIPCION          VARCHAR2(1000),
    COL_FECHA_INICIO         DATE,
    COL_FECHA_FIN            DATE,
    COL_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT CHK_COL_ESTADO CHECK (COL_ESTADO IN ('ACTIVO', 'INACTIVO', 'DESCONTINUADO')),
    CONSTRAINT CHK_COL_FECHAS CHECK (COL_FECHA_FIN IS NULL OR COL_FECHA_FIN >= COL_FECHA_INICIO)
);

COMMENT ON TABLE ALP_COLECCION IS 'Colecciones o líneas de productos (temporales o permanentes)';

CREATE INDEX IDX_COL_ESTADO ON ALP_COLECCION(COL_ESTADO);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_ATRIBUTO
-- Descripción: Atributos configurables de productos
-- Ejemplos: 'Color', 'Tamaño', 'Material', 'Acabado'
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_ATRIBUTO (
    ATR_ATRIBUTO             NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ATR_CODIGO               VARCHAR2(50) NOT NULL UNIQUE,
    ATR_NOMBRE               VARCHAR2(100) NOT NULL,
    ATR_TIPO                 VARCHAR2(20) NOT NULL,
    ATR_DESCRIPCION          VARCHAR2(500),
    ATR_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT CHK_ATR_TIPO CHECK (ATR_TIPO IN ('COLOR', 'TAMANO', 'MATERIAL', 'ACABADO', 'ESTILO', 'OTRO')),
    CONSTRAINT CHK_ATR_ESTADO CHECK (ATR_ESTADO IN ('ACTIVO', 'INACTIVO'))
);

COMMENT ON TABLE ALP_ATRIBUTO IS 'Atributos configurables para variantes de productos';
COMMENT ON COLUMN ALP_ATRIBUTO.ATR_TIPO IS 'Tipo de atributo para agrupación en UI';

CREATE INDEX IDX_ATR_TIPO ON ALP_ATRIBUTO(ATR_TIPO);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_ATRIBUTO_VALOR
-- Descripción: Valores posibles de cada atributo
-- Ejemplos: Para atributo 'Color': 'Rojo', 'Azul', 'Negro'
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_ATRIBUTO_VALOR (
    ATV_ATRIBUTO_VALOR       NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    ATR_ATRIBUTO             NUMBER NOT NULL,
    ATV_VALOR                VARCHAR2(100) NOT NULL,
    ATV_CODIGO_HEX           VARCHAR2(7),
    ATV_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT FK_ATV_ATRIBUTO FOREIGN KEY (ATR_ATRIBUTO) 
        REFERENCES ALP_ATRIBUTO(ATR_ATRIBUTO),
    CONSTRAINT UNQ_ATV_ATRIBUTO_VALOR UNIQUE (ATR_ATRIBUTO, ATV_VALOR),
    CONSTRAINT CHK_ATV_ESTADO CHECK (ATV_ESTADO IN ('ACTIVO', 'INACTIVO'))
);

COMMENT ON TABLE ALP_ATRIBUTO_VALOR IS 'Valores específicos de cada atributo';
COMMENT ON COLUMN ALP_ATRIBUTO_VALOR.ATV_CODIGO_HEX IS 'Código hexadecimal para colores (#FF0000)';

CREATE INDEX IDX_ATV_ATRIBUTO ON ALP_ATRIBUTO_VALOR(ATR_ATRIBUTO);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_MATERIAL
-- Descripción: Materiales utilizados en productos
-- Ejemplos: 'Madera de Roble', 'Tela Lino', 'Metal Cromado'
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_MATERIAL (
    MAT_MATERIAL             NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    MAT_CODIGO               VARCHAR2(50) NOT NULL UNIQUE,
    MAT_NOMBRE               VARCHAR2(150) NOT NULL,
    MAT_DESCRIPCION          VARCHAR2(500),
    MAT_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT CHK_MAT_ESTADO CHECK (MAT_ESTADO IN ('ACTIVO', 'INACTIVO', 'DESCONTINUADO'))
);

COMMENT ON TABLE ALP_MATERIAL IS 'Catálogo de materiales de fabricación';

-- ----------------------------------------------------------------------------
-- Tabla: ALP_COLOR
-- Descripción: Catálogo de colores disponibles
-- Uso: Colores estándar de la marca, paleta de diseño
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_COLOR (
    COL_COLOR                NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    COL_CODIGO               VARCHAR2(50) NOT NULL UNIQUE, 
    COL_NOMBRE               VARCHAR2(100) NOT NULL,
    COL_CODIGO_HEX           VARCHAR2(7) NOT NULL,
    COL_DESCRIPCION          VARCHAR2(500),
    COL_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    -- Nombres únicos: agregamos _COLOR_ para diferenciarlos
    CONSTRAINT CHK_COLOR_ESTADO CHECK (COL_ESTADO IN ('ACTIVO', 'INACTIVO')),
    CONSTRAINT CHK_COLOR_HEX CHECK (COL_CODIGO_HEX LIKE '#%')
);

COMMENT ON TABLE ALP_COLOR IS 'Paleta de colores estándar de la marca';
COMMENT ON COLUMN ALP_COLOR.COL_CODIGO_HEX IS 'Color en formato hexadecimal (#RRGGBB)';

-- Nota: Recuerda que COL_CODIGO ya tiene índice por ser UNIQUE, 
-- no necesitas el CREATE INDEX manual aquí.

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO
-- Descripción: Productos maestros (SKU base)
-- CRÍTICO: Tabla principal del catálogo de productos
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO (
    PRO_PRODUCTO             NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    TMU_TIPO_MUEBLE          NUMBER,
    PRO_SKU                  VARCHAR2(100) NOT NULL UNIQUE,
    PRO_NOMBRE               VARCHAR2(255) NOT NULL,
    PRO_DESCRIPCION_CORTA    VARCHAR2(500),
    PRO_DESCRIPCION_LARGA    CLOB,
    PRO_PESO                 NUMBER(10,2),
    PRO_ES_CONFIGURABLE      VARCHAR2(1) DEFAULT 'N' NOT NULL,
    PRO_DESTACADO            VARCHAR2(1) DEFAULT 'N' NOT NULL,
    PRO_PUBLICADO            VARCHAR2(1) DEFAULT 'N' NOT NULL,
    PRO_ESTADO               VARCHAR2(20) DEFAULT 'BORRADOR' NOT NULL,
    PRO_FECHA_CREACION       TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT FK_PRO_TIPO_MUEBLE FOREIGN KEY (TMU_TIPO_MUEBLE) 
        REFERENCES ALP_TIPO_MUEBLE(TMU_TIPO_MUEBLE),
    CONSTRAINT CHK_PRO_CONFIGURABLE CHECK (PRO_ES_CONFIGURABLE IN ('S', 'N')),
    CONSTRAINT CHK_PRO_DESTACADO CHECK (PRO_DESTACADO IN ('S', 'N')),
    CONSTRAINT CHK_PRO_PUBLICADO CHECK (PRO_PUBLICADO IN ('S', 'N')),
    CONSTRAINT CHK_PRO_ESTADO CHECK (PRO_ESTADO IN ('BORRADOR', 'ACTIVO', 'INACTIVO', 'DESCONTINUADO')),
    CONSTRAINT CHK_PRO_PESO CHECK (PRO_PESO IS NULL OR PRO_PESO > 0)
);

COMMENT ON TABLE ALP_PRODUCTO IS 'Catálogo maestro de productos (SKU base)';
COMMENT ON COLUMN ALP_PRODUCTO.PRO_SKU IS 'SKU único del producto (Stock Keeping Unit)';
COMMENT ON COLUMN ALP_PRODUCTO.PRO_ES_CONFIGURABLE IS 'S = tiene variantes configurables';
COMMENT ON COLUMN ALP_PRODUCTO.PRO_DESTACADO IS 'S = producto destacado en homepage';

CREATE INDEX IDX_PRO_TIPO_MUEBLE ON ALP_PRODUCTO(TMU_TIPO_MUEBLE);
CREATE INDEX IDX_PRO_ESTADO ON ALP_PRODUCTO(PRO_ESTADO);
CREATE INDEX IDX_PRO_DESTACADO ON ALP_PRODUCTO(PRO_DESTACADO);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_CATEGORIA
-- Descripción: Relación muchos a muchos entre productos y categorías
-- Un producto puede pertenecer a múltiples categorías
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_CATEGORIA (
    PCT_PRODUCTO_CATEGORIA   NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    CAT_CATEGORIA            NUMBER NOT NULL,
    PCT_PRINCIPAL            VARCHAR2(1) DEFAULT 'N' NOT NULL,
    
    CONSTRAINT FK_PCT_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_PCT_CATEGORIA FOREIGN KEY (CAT_CATEGORIA) 
        REFERENCES ALP_CATEGORIA(CAT_CATEGORIA),
    CONSTRAINT UNQ_PCT_PRODUCTO_CATEGORIA UNIQUE (PRO_PRODUCTO, CAT_CATEGORIA),
    CONSTRAINT CHK_PCT_PRINCIPAL CHECK (PCT_PRINCIPAL IN ('S', 'N'))
);

COMMENT ON TABLE ALP_PRODUCTO_CATEGORIA IS 'Clasificación de productos en categorías';

CREATE INDEX IDX_PCT_PRODUCTO ON ALP_PRODUCTO_CATEGORIA(PRO_PRODUCTO);
CREATE INDEX IDX_PCT_CATEGORIA ON ALP_PRODUCTO_CATEGORIA(CAT_CATEGORIA);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_COLECCION
-- Descripción: Relación entre productos y colecciones
-- Un producto puede pertenecer a múltiples colecciones
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_COLECCION (
    PCL_PRODUCTO_COLECCION   NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    COL_COLECCION            NUMBER NOT NULL,
    PCL_ORDEN                NUMBER,
    
    CONSTRAINT FK_PCL_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_PCL_COLECCION FOREIGN KEY (COL_COLECCION) 
        REFERENCES ALP_COLECCION(COL_COLECCION),
    CONSTRAINT UNQ_PCL_PRODUCTO_COLECCION UNIQUE (PRO_PRODUCTO, COL_COLECCION)
);

COMMENT ON TABLE ALP_PRODUCTO_COLECCION IS 'Asignación de productos a colecciones';
COMMENT ON COLUMN ALP_PRODUCTO_COLECCION.PCL_ORDEN IS 'Orden de visualización en la colección';

CREATE INDEX IDX_PCL_PRODUCTO ON ALP_PRODUCTO_COLECCION(PRO_PRODUCTO);
CREATE INDEX IDX_PCL_COLECCION ON ALP_PRODUCTO_COLECCION(COL_COLECCION);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_MATERIAL
-- Descripción: Materiales utilizados en cada producto
-- Un producto puede estar fabricado con múltiples materiales
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_MATERIAL (
    PMA_PRODUCTO_MATERIAL    NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    MAT_MATERIAL             NUMBER NOT NULL,
    PMA_PORCENTAJE           NUMBER(5,2),
    PMA_PRINCIPAL            VARCHAR2(1) DEFAULT 'N' NOT NULL,
    
    CONSTRAINT FK_PMA_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_PMA_MATERIAL FOREIGN KEY (MAT_MATERIAL) 
        REFERENCES ALP_MATERIAL(MAT_MATERIAL),
    CONSTRAINT UNQ_PMA_PRODUCTO_MATERIAL UNIQUE (PRO_PRODUCTO, MAT_MATERIAL),
    CONSTRAINT CHK_PMA_PRINCIPAL CHECK (PMA_PRINCIPAL IN ('S', 'N')),
    CONSTRAINT CHK_PMA_PORCENTAJE CHECK (PMA_PORCENTAJE IS NULL OR (PMA_PORCENTAJE > 0 AND PMA_PORCENTAJE <= 100))
);

COMMENT ON TABLE ALP_PRODUCTO_MATERIAL IS 'Composición de materiales por producto';

CREATE INDEX IDX_PMA_PRODUCTO ON ALP_PRODUCTO_MATERIAL(PRO_PRODUCTO);
CREATE INDEX IDX_PMA_MATERIAL ON ALP_PRODUCTO_MATERIAL(MAT_MATERIAL);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_COLOR
-- Descripción: Colores disponibles para cada producto
-- Un producto puede estar disponible en múltiples colores
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_COLOR (
    PCO_PRODUCTO_COLOR       NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    COL_COLOR                NUMBER NOT NULL,
    PCO_IMAGEN_URL           VARCHAR2(500),
    PCO_ORDEN                NUMBER,
    PCO_DISPONIBLE           VARCHAR2(1) DEFAULT 'S' NOT NULL,
    PCO_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT FK_PCO_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_PCO_COLOR FOREIGN KEY (COL_COLOR) 
        REFERENCES ALP_COLOR(COL_COLOR),
    CONSTRAINT UNQ_PCO_PRODUCTO_COLOR UNIQUE (PRO_PRODUCTO, COL_COLOR),
    CONSTRAINT CHK_PCO_DISPONIBLE CHECK (PCO_DISPONIBLE IN ('S', 'N')),
    CONSTRAINT CHK_PCO_ESTADO CHECK (PCO_ESTADO IN ('ACTIVO', 'INACTIVO'))
);

COMMENT ON TABLE ALP_PRODUCTO_COLOR IS 'Variantes de color por producto';

CREATE INDEX IDX_PCO_PRODUCTO ON ALP_PRODUCTO_COLOR(PRO_PRODUCTO);
CREATE INDEX IDX_PCO_COLOR ON ALP_PRODUCTO_COLOR(COL_COLOR);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_DIMENSION
-- Descripción: Dimensiones físicas de productos
-- Uso: Cálculo de envíos, especificaciones técnicas
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_DIMENSION (
    PDI_PRODUCTO_DIMENSION   NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    UNI_UNIDAD_MEDIDA        NUMBER NOT NULL,
    PDI_LARGO                NUMBER(10,2),
    PDI_ANCHO                NUMBER(10,2),
    PDI_ALTO                 NUMBER(10,2),
    PDI_DIAMETRO             NUMBER(10,2),
    
    CONSTRAINT FK_PDI_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_PDI_UNIDAD FOREIGN KEY (UNI_UNIDAD_MEDIDA) 
        REFERENCES ALP_UNIDAD_MEDIDA(UNI_UNIDAD_MEDIDA),
    CONSTRAINT UNQ_PDI_PRODUCTO UNIQUE (PRO_PRODUCTO),
    CONSTRAINT CHK_PDI_MEDIDAS CHECK (
        (PDI_LARGO IS NOT NULL OR PDI_ANCHO IS NOT NULL OR PDI_ALTO IS NOT NULL OR PDI_DIAMETRO IS NOT NULL)
    )
);

COMMENT ON TABLE ALP_PRODUCTO_DIMENSION IS 'Dimensiones físicas de productos';


-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_VARIANTE
-- Descripción: Variantes de productos configurables
-- Ejemplo: Sofá modelo X en color Azul, tamaño L, acabado Mate
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_VARIANTE (
    PVA_PRODUCTO_VARIANTE    NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    PVA_SKU                  VARCHAR2(100) NOT NULL UNIQUE,
    PVA_NOMBRE               VARCHAR2(255),
    PVA_CODIGO_BARRAS        VARCHAR2(50),
    PVA_IMAGEN_URL           VARCHAR2(500),
    PVA_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT FK_PVA_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT CHK_PVA_ESTADO CHECK (PVA_ESTADO IN ('ACTIVO', 'INACTIVO', 'AGOTADO'))
);

COMMENT ON TABLE ALP_PRODUCTO_VARIANTE IS 'Variantes configurables de productos (SKU específicos)';
COMMENT ON COLUMN ALP_PRODUCTO_VARIANTE.PVA_SKU IS 'SKU único de la variante (ej: SOFA-X-AZUL-L)';

CREATE INDEX IDX_PVA_PRODUCTO ON ALP_PRODUCTO_VARIANTE(PRO_PRODUCTO);
CREATE INDEX IDX_PVA_BARRAS ON ALP_PRODUCTO_VARIANTE(PVA_CODIGO_BARRAS);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_VARIANTE_ATRIBUTO_VALOR
-- Descripción: Valores de atributos de cada variante
-- Ejemplo: Variante 123 tiene Color=Azul, Tamaño=L
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_VARIANTE_ATRIBUTO_VALOR (
    VAV_VARIANTE_ATRIBUTO_VALOR NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PVA_PRODUCTO_VARIANTE    NUMBER NOT NULL,
    ATV_ATRIBUTO_VALOR       NUMBER NOT NULL,
    
    CONSTRAINT FK_VAV_VARIANTE FOREIGN KEY (PVA_PRODUCTO_VARIANTE) 
        REFERENCES ALP_PRODUCTO_VARIANTE(PVA_PRODUCTO_VARIANTE),
    CONSTRAINT FK_VAV_ATRIBUTO_VALOR FOREIGN KEY (ATV_ATRIBUTO_VALOR) 
        REFERENCES ALP_ATRIBUTO_VALOR(ATV_ATRIBUTO_VALOR),
    CONSTRAINT UNQ_VAV_VARIANTE_ATRIBUTO UNIQUE (PVA_PRODUCTO_VARIANTE, ATV_ATRIBUTO_VALOR)
);

COMMENT ON TABLE ALP_VARIANTE_ATRIBUTO_VALOR IS 'Configuración de atributos por variante';

CREATE INDEX IDX_VAV_VARIANTE ON ALP_VARIANTE_ATRIBUTO_VALOR(PVA_PRODUCTO_VARIANTE);
CREATE INDEX IDX_VAV_ATRIBUTO_VALOR ON ALP_VARIANTE_ATRIBUTO_VALOR(ATV_ATRIBUTO_VALOR);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_IMAGEN
-- Descripción: Galería de imágenes de productos
-- Uso: Imágenes principales, secundarias, zoom, 360°
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_IMAGEN (
    PIM_PRODUCTO_IMAGEN      NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    PIM_URL                  VARCHAR2(500) NOT NULL,
    PIM_TIPO                 VARCHAR2(20) DEFAULT 'PRINCIPAL' NOT NULL,
    PIM_ORDEN                NUMBER DEFAULT 1 NOT NULL,
    PIM_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT FK_PIM_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT CHK_PIM_TIPO CHECK (PIM_TIPO IN ('PRINCIPAL', 'SECUNDARIA', 'DETALLE', '360', 'VIDEO')),
    CONSTRAINT CHK_PIM_ESTADO CHECK (PIM_ESTADO IN ('ACTIVO', 'INACTIVO')),
    CONSTRAINT CHK_PIM_ORDEN CHECK (PIM_ORDEN > 0)
);

COMMENT ON TABLE ALP_PRODUCTO_IMAGEN IS 'Galería multimedia de productos';

CREATE INDEX IDX_PIM_PRODUCTO ON ALP_PRODUCTO_IMAGEN(PRO_PRODUCTO);
CREATE INDEX IDX_PIM_TIPO ON ALP_PRODUCTO_IMAGEN(PIM_TIPO);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_IDIOMA
-- Descripción: Traducciones de productos para multi-idioma
-- Uso: Nombres y descripciones en diferentes idiomas
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_IDIOMA (
    PID_PRODUCTO_IDIOMA      NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    IDI_IDIOMA               NUMBER NOT NULL,
    PID_NOMBRE               VARCHAR2(255) NOT NULL,
    PID_DESCRIPCION_CORTA    VARCHAR2(500),
    PID_DESCRIPCION_LARGA    CLOB,
    
    CONSTRAINT FK_PID_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_PID_IDIOMA FOREIGN KEY (IDI_IDIOMA) 
        REFERENCES ALP_IDIOMA(IDI_IDIOMA),
    CONSTRAINT UNQ_PID_PRODUCTO_IDIOMA UNIQUE (PRO_PRODUCTO, IDI_IDIOMA)
);

COMMENT ON TABLE ALP_PRODUCTO_IDIOMA IS 'Traducciones de productos (i18n)';

CREATE INDEX IDX_PID_PRODUCTO ON ALP_PRODUCTO_IDIOMA(PRO_PRODUCTO);
CREATE INDEX IDX_PID_IDIOMA ON ALP_PRODUCTO_IDIOMA(IDI_IDIOMA);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_PRECIO
-- Descripción: Precios actuales de productos
-- Soporte multi-moneda y tipos de precio (regular, mayorista)
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_PRECIO (
    PPR_PRODUCTO_PRECIO      NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER,
    PVA_PRODUCTO_VARIANTE    NUMBER,
    MON_MONEDA               NUMBER NOT NULL,
    PPR_TIPO                 VARCHAR2(20) DEFAULT 'REGULAR' NOT NULL,
    PPR_PRECIO               NUMBER(12,2) NOT NULL,
    PPR_PRECIO_OFERTA        NUMBER(12,2),
    PPR_FECHA_INICIO         DATE DEFAULT SYSDATE NOT NULL,
    PPR_FECHA_FIN            DATE,
    PPR_ESTADO               VARCHAR2(20) DEFAULT 'ACTIVO' NOT NULL,
    
    CONSTRAINT FK_PPR_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_PPR_VARIANTE FOREIGN KEY (PVA_PRODUCTO_VARIANTE) 
        REFERENCES ALP_PRODUCTO_VARIANTE(PVA_PRODUCTO_VARIANTE),
    CONSTRAINT FK_PPR_MONEDA FOREIGN KEY (MON_MONEDA) 
        REFERENCES ALP_MONEDA(MON_MONEDA),
    CONSTRAINT CHK_PPR_TIPO CHECK (PPR_TIPO IN ('REGULAR', 'MAYORISTA', 'CORPORATIVO', 'VIP')),
    CONSTRAINT CHK_PPR_ESTADO CHECK (PPR_ESTADO IN ('ACTIVO', 'INACTIVO', 'VENCIDO')),
    CONSTRAINT CHK_PPR_PRECIO CHECK (PPR_PRECIO > 0),
    CONSTRAINT CHK_PPR_OFERTA CHECK (PPR_PRECIO_OFERTA IS NULL OR (PPR_PRECIO_OFERTA > 0 AND PPR_PRECIO_OFERTA < PPR_PRECIO)),
    CONSTRAINT CHK_PPR_FECHAS CHECK (PPR_FECHA_FIN IS NULL OR PPR_FECHA_FIN >= PPR_FECHA_INICIO),
    CONSTRAINT CHK_PPR_PRODUCTO_O_VARIANTE CHECK (
        (PRO_PRODUCTO IS NOT NULL AND PVA_PRODUCTO_VARIANTE IS NULL) OR
        (PRO_PRODUCTO IS NULL AND PVA_PRODUCTO_VARIANTE IS NOT NULL)
    )
);

COMMENT ON TABLE ALP_PRODUCTO_PRECIO IS 'Precios vigentes de productos y variantes';
COMMENT ON COLUMN ALP_PRODUCTO_PRECIO.PPR_PRECIO_OFERTA IS 'Precio especial/promocional (menor al regular)';

CREATE INDEX IDX_PPR_PRODUCTO ON ALP_PRODUCTO_PRECIO(PRO_PRODUCTO);
CREATE INDEX IDX_PPR_VARIANTE ON ALP_PRODUCTO_PRECIO(PVA_PRODUCTO_VARIANTE);
CREATE INDEX IDX_PPR_MONEDA ON ALP_PRODUCTO_PRECIO(MON_MONEDA);
CREATE INDEX IDX_PPR_ESTADO ON ALP_PRODUCTO_PRECIO(PPR_ESTADO);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_PRODUCTO_PRECIO_HIST
-- Descripción: Historial de cambios de precios
-- Uso: Auditoría, análisis de tendencias, estrategia de pricing
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_PRODUCTO_PRECIO_HIST (
    PPH_PRODUCTO_PRECIO_HIST NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PPR_PRODUCTO_PRECIO      NUMBER NOT NULL,
    PPH_PRECIO_ANTERIOR      NUMBER(12,2),
    PPH_PRECIO_NUEVO         NUMBER(12,2) NOT NULL,
    USU_USUARIO              NUMBER,
    PPH_MOTIVO               VARCHAR2(500),
    PPH_FECHA_CAMBIO         TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    
    CONSTRAINT FK_PPH_PRECIO FOREIGN KEY (PPR_PRODUCTO_PRECIO) 
        REFERENCES ALP_PRODUCTO_PRECIO(PPR_PRODUCTO_PRECIO),
    CONSTRAINT FK_PPH_USUARIO FOREIGN KEY (USU_USUARIO) 
        REFERENCES ALP_USUARIO(USU_USUARIO)
);

COMMENT ON TABLE ALP_PRODUCTO_PRECIO_HIST IS 'Historial de cambios de precios (auditoría)';

CREATE INDEX IDX_PPH_PRECIO ON ALP_PRODUCTO_PRECIO_HIST(PPR_PRODUCTO_PRECIO);
CREATE INDEX IDX_PPH_FECHA ON ALP_PRODUCTO_PRECIO_HIST(PPH_FECHA_CAMBIO);

-- ----------------------------------------------------------------------------
-- Tabla: ALP_RESENA_PRODUCTO
-- Descripción: Reseñas y calificaciones de clientes
-- Uso: Social proof, SEO, mejora de productos
-- ----------------------------------------------------------------------------
CREATE TABLE ALP_RESENA_PRODUCTO (
    RPR_RESENA_PRODUCTO      NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    PRO_PRODUCTO             NUMBER NOT NULL,
    CLI_CLIENTE              NUMBER NOT NULL,
    RPR_CALIFICACION         NUMBER(2,1) NOT NULL,
    RPR_TITULO               VARCHAR2(255),
    RPR_COMENTARIO           CLOB,
    RPR_VERIFICADA           VARCHAR2(1) DEFAULT 'N' NOT NULL,
    RPR_APROBADA             VARCHAR2(1) DEFAULT 'N' NOT NULL,
    RPR_DESTACADA            VARCHAR2(1) DEFAULT 'N' NOT NULL,
    RPR_FECHA_RESENA         TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    RPR_ESTADO               VARCHAR2(20) DEFAULT 'PENDIENTE' NOT NULL,
    
    CONSTRAINT FK_RPR_PRODUCTO FOREIGN KEY (PRO_PRODUCTO) 
        REFERENCES ALP_PRODUCTO(PRO_PRODUCTO),
    CONSTRAINT FK_RPR_CLIENTE FOREIGN KEY (CLI_CLIENTE) 
        REFERENCES ALP_CLIENTE(CLI_CLIENTE),
    CONSTRAINT CHK_RPR_CALIFICACION CHECK (RPR_CALIFICACION >= 1 AND RPR_CALIFICACION <= 5),
    CONSTRAINT CHK_RPR_VERIFICADA CHECK (RPR_VERIFICADA IN ('S', 'N')),
    CONSTRAINT CHK_RPR_APROBADA CHECK (RPR_APROBADA IN ('S', 'N')),
    CONSTRAINT CHK_RPR_DESTACADA CHECK (RPR_DESTACADA IN ('S', 'N')),
    CONSTRAINT CHK_RPR_ESTADO CHECK (RPR_ESTADO IN ('PENDIENTE', 'APROBADA', 'RECHAZADA', 'REPORTADA'))
);

COMMENT ON TABLE ALP_RESENA_PRODUCTO IS 'Reseñas y calificaciones de productos por clientes';
COMMENT ON COLUMN ALP_RESENA_PRODUCTO.RPR_VERIFICADA IS 'S = cliente compró el producto';
COMMENT ON COLUMN ALP_RESENA_PRODUCTO.RPR_CALIFICACION IS 'Calificación de 1.0 a 5.0 estrellas';

CREATE INDEX IDX_RPR_PRODUCTO ON ALP_RESENA_PRODUCTO(PRO_PRODUCTO);
CREATE INDEX IDX_RPR_CLIENTE ON ALP_RESENA_PRODUCTO(CLI_CLIENTE);
CREATE INDEX IDX_RPR_CALIFICACION ON ALP_RESENA_PRODUCTO(RPR_CALIFICACION);
CREATE INDEX IDX_RPR_ESTADO ON ALP_RESENA_PRODUCTO(RPR_ESTADO);
CREATE INDEX IDX_RPR_FECHA ON ALP_RESENA_PRODUCTO(RPR_FECHA_RESENA);

-- ============================================================================
-- FIN BLOQUE 5 - PRODUCTOS Y CATÁLOGO
-- ============================================================================

SELECT 'BLOQUE 5 COMPLETADO - Productos y Catálogo' AS ESTADO FROM DUAL;
