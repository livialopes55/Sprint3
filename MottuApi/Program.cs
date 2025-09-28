using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MottuApi.Data;
using MottuApi.Models;
using System.Text.Json.Serialization;
using System;

var builder = WebApplication.CreateBuilder(args);


var conn = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrWhiteSpace(conn) &&
    conn.Contains("database.windows.net", StringComparison.OrdinalIgnoreCase))
{
    // Azure SQL
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));
}
else
{
   
    var sqliteConn = string.IsNullOrWhiteSpace(conn)
        ? "Data Source=/home/site/wwwroot/app.db"
        : conn;

    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(sqliteConn));
}

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Versionamento de API
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mottu API - Sprint 3 (.NET)",
        Version = "v1",
        Description = "API RESTful para Filiais, Pátios e Motos com paginação, HATEOAS e boas práticas REST."
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();


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
        db.Motos.Add(new Moto
        {
            Placa = "ABC1D23",
            Modelo = "CG 160",
            Ano = 2022,
            Status = "Em uso",
            Patio = patio
        });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();


app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
