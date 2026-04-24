using Microsoft.Extensions.DependencyInjection;
using MuebleriaAlpesWebBackend.Data.Connection;
using MuebleriaAlpesWebBackend.Business.Services;
using MuebleriaAlpesWebBackend.Data.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Repositories;
using MuebleriaAlpesWebBackend.Domain.Interfaces.Services;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OracleConnectionFactory>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IEnviosRepository, EnviosRepository>();
builder.Services.AddScoped<IEnviosService, EnviosService>();
builder.Services.AddScoped<ISeguridadRepository, SeguridadRepository>();
builder.Services.AddScoped<ISeguridadService, SeguridadService>();
builder.Services.AddScoped<IAutenticacionRepository, AutenticacionRepository>();
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
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
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
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
