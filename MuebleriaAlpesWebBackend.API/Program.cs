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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
