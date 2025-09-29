// Program.cs (versão só SQLite)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MottuApi.Data;
using MottuApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Sempre SQLite
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? "Data Source=/home/site/wwwroot/app.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(conn));

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Versionamento
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ReportApiVersions = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mottu API - Sprint 3 (.NET)", Version = "v1" });
});

var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Filiais.Any())
    {
        var filial = new Filial { Nome = "Filial Centro", Endereco = "Av. Central, 1000" };
        var patio = new Patio { Descricao = "Pátio A", Dimensao = "50x30", Filial = filial };
        db.Filiais.Add(filial);
        db.Patios.Add(patio);
        db.Motos.Add(new Moto { Placa = "ABC1D23", Modelo = "CG 160", Ano = 2022, Status = "Em uso", Patio = patio });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();
