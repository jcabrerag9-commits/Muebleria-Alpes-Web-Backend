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

// Productos (Genéricos)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();

// Productos (Inventario)
builder.Services.AddScoped<IProductoInventarioRepository, ProductoInventarioRepository>();
builder.Services.AddScoped<IProductoInventarioService, ProductoInventarioService>();

builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IProductoImagenRepository, ProductoImagenRepository>();
builder.Services.AddScoped<IProductoImagenService, ProductoImagenService>();
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

builder.Services.AddScoped<OracleConnectionFactory>();

// Seguridad y autenticación
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowMVC");
app.UseAuthorization();
app.MapControllers();
app.Run();
