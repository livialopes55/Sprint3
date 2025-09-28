using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MottuApi.Data;
using MottuApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Cria/seed do banco local (SQLite)
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

// Swagger sempre habilitado aqui (ok para dev)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

// Redireciona a raiz para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
