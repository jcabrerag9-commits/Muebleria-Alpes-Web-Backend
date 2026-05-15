using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Business.Services.Seguridad;
using MuebleriaAlpesWebBackend.Business.Services.Autenticacion;
using MuebleriaAlpesWebBackend.Business.Services.Envios;
using MuebleriaAlpesWebBackend.Business.Services.Reportes;
using MuebleriaAlpesWebBackend.Data.Repositories.Seguridad;
using MuebleriaAlpesWebBackend.Data.Repositories.Autenticacion;
using MuebleriaAlpesWebBackend.Data.Repositories.Envios;
using MuebleriaAlpesWebBackend.Data.Repositories.Reportes;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Seguridad;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Autenticacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Envios;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.Reportes;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.Seguridad;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.Autenticacion;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.Envios;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.Reportes;
using MuebleriaAlpesWebBackend.Business.Services;
using MuebleriaAlpesWebBackend.Business.Services.RecursosHumanos;
using MuebleriaAlpesWebBackend.Data.Repositories;
using MuebleriaAlpesWebBackend.Data.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories.RecursosHumanos;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services.RecursosHumanos;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OracleConnectionFactory>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IVarianteRepository, VarianteRepository>();
builder.Services.AddScoped<IContenidoRepository, ContenidoRepository>();
builder.Services.AddScoped<IPrecioRepository, PrecioRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IFacturacionRepository, FacturacionRepository>();
builder.Services.AddScoped<IFacturacionService, FacturacionService>();

// Categorías
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// Colores
builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<IColorService, ColorService>();

// Materiales
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMaterialService, MaterialService>();

// Productos
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IProductoImagenRepository, ProductoImagenRepository>();
builder.Services.AddScoped<IProductoImagenService, ProductoImagenService>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IBodegaRepository, BodegaRepository>();
builder.Services.AddScoped<IBodegaService, BodegaService>();
builder.Services.AddScoped<IFinanzasRepository, FinanzasRepository>();
builder.Services.AddScoped<IFinanzasService, FinanzasService>();
builder.Services.AddScoped<IVarianteService, VarianteService>();
builder.Services.AddScoped<IContenidoService, ContenidoService>();
builder.Services.AddScoped<IPrecioService, PrecioService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVentasRepository, VentasRepository>();
builder.Services.AddScoped<IVentasService, VentasService>();
builder.Services.AddScoped<ICarritoRepository, CarritoRepository>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<ILogisticaRepository, LogisticaRepository>();
builder.Services.AddScoped<ILogisticaService, LogisticaService>();
builder.Services.AddScoped<IReportesRepository, ReportesRepository>();
builder.Services.AddScoped<IReportesService, ReportesService>();
builder.Services.AddScoped<ICajaRepository, CajaRepository>();
builder.Services.AddScoped<ICajaService, CajaService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

// ── Seguridad y autenticación ────────────────────────────────────────────────
builder.Services.AddScoped<ISeguridadRepository, SeguridadRepository>();
builder.Services.AddScoped<ISeguridadService, SeguridadService>();
builder.Services.AddScoped<IAutenticacionRepository, AutenticacionRepository>();
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();

// Envíos
builder.Services.AddScoped<IEnviosRepository, EnviosRepository>();
builder.Services.AddScoped<IEnviosService, EnviosService>();

// Reportes
builder.Services.AddScoped<IReportesClienteRepository, ReportesClienteRepository>();
builder.Services.AddScoped<IReportesClienteService, ReportesClienteService>();
builder.Services.AddScoped<IReportesVentasRepository, ReportesVentasRepository>();
builder.Services.AddScoped<IReportesVentasService, ReportesVentasService>();
builder.Services.AddScoped<IReportesCajaRepository, ReportesCajaRepository>();
builder.Services.AddScoped<IReportesCajaService, ReportesCajaService>();
builder.Services.AddScoped<IReportesMarketingRepository, ReportesMarketingRepository>();
builder.Services.AddScoped<IReportesMarketingService, ReportesMarketingService>();

// ── Autenticación ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ── Recursos Humanos ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<IPuestoRepository, PuestoRepository>();
builder.Services.AddScoped<IPuestoService, PuestoService>();
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddScoped<ITipoPagoRepository, TipoPagoRepository>();
builder.Services.AddScoped<ITipoPagoService, TipoPagoService>();
builder.Services.AddScoped<ITipoDeduccionRepository, TipoDeduccionRepository>();
builder.Services.AddScoped<ITipoDeduccionService, TipoDeduccionService>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IAsignacionEmpleadoRepository, AsignacionEmpleadoRepository>();
builder.Services.AddScoped<IAsignacionEmpleadoService, AsignacionEmpleadoService>();
builder.Services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
builder.Services.AddScoped<IAsistenciaService, AsistenciaService>();
builder.Services.AddScoped<IVacacionRepository, VacacionesRepository>();
builder.Services.AddScoped<IVacacionesService, VacacionesService>();
builder.Services.AddScoped<INominaRepository, NominaRepository>();
builder.Services.AddScoped<INominaService, NominaService>();
builder.Services.AddScoped<IEvaluacionRepository, EvaluacionRepository>();
builder.Services.AddScoped<IEvaluacionService, EvaluacionService>();

// ── Promociones ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<IPromocionRepository, PromocionRepository>();
builder.Services.AddScoped<IPromocionService, PromocionService>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IBannerService, BannerService>();

// ── Devoluciones ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IDevolucionRepository, DevolucionRepository>();
builder.Services.AddScoped<IDevolucionService, DevolucionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<MuebleriaAlpesWebBackend.API.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowMVC");
app.UseAuthorization();
app.MapControllers();
app.Run();
