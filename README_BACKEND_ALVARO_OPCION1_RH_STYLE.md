# Backend Alvaro - Opción 1 estilo RH

Backend limpio para los módulos asignados:

- Seguridad
- Autenticación
- Envíos
- Reportes

## Organización estilo RH

La estructura se separó por módulo dentro de cada capa:

```text
MuebleriaAlpesWebBackend.API/Controllers
├── Seguridad
├── Autenticacion
├── Envios
└── Reportes

MuebleriaAlpesWebBackend.Business/Services
├── Seguridad
├── Autenticacion
├── Envios
└── Reportes

MuebleriaAlpesWebBackend.Data/Repositories
├── Seguridad
├── Autenticacion
├── Envios
└── Reportes

MuebleriaAlpesWebBackend.Domain
├── DTOs
└── Interfaces
```

## Packages Oracle consumidos

- `PKG_SEGURIDAD`
- `PKG_AUTENTICACION`
- `PKG_ENVIOS`
- `PKG_REPORTES_CLIENTE`
- `PKG_REPORTES_VENTAS`
- `PKG_REPORTES_CAJA`
- `PKG_REPORTES_MARKETING`

## Qué no incluye

No incluye backend completo de:

- Clientes
- Catálogos generales
- Catálogos comerciales
- Ubicaciones
- Productos
- Carrito
- Ventas/Pagos como módulos CRUD

Esos datos se reciben como IDs o son consultados por los packages de reportes/envíos cuando corresponde.

## Reglas respetadas

- El backend llama packages PL/SQL.
- No genera códigos manuales.
- No manda PK `IDENTITY` en procesos de creación.
- Trabaja con las salidas de los packages: resultado, mensaje, ID/código cuando aplica.
- Seguridad incluye autenticación porque login, logout, sesiones y recuperación de clave dependen de usuarios.

## Prueba local

Abrir la solución en Visual Studio y ejecutar:

```bash
dotnet restore
dotnet build
```

Luego revisar `appsettings.json` y ajustar la cadena `OracleDb` según tu ambiente.
