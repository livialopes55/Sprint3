using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using MottuApi.Models;
using MottuApi.Utils;

namespace MottuApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/motos")]
    public class MotosController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MotosController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = _db.Motos.AsNoTracking().OrderBy(m => m.Id);
            var paged = query.ToPagedResult(pageNumber, pageSize);
            var links = new List<Link>
            {
                new("self", Url.Action(nameof(GetAll), values: new { pageNumber, pageSize, version = HttpContext.GetRequestedApiVersion()?.ToString()} ) ?? "", "GET"),
                new("create", Url.Action(nameof(Create), values: new { version = HttpContext.GetRequestedApiVersion()?.ToString() }) ?? "", "POST")
            };
            return Ok(new { data = paged.Items, total = paged.TotalCount, pageNumber, pageSize, links });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _db.Motos.FindAsync(id);
            if (entity is null) return NotFound();
            var links = new List<Link>
            {
                new("self", Url.Action(nameof(GetById), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString() }) ?? "", "GET"),
                new("update", Url.Action(nameof(Update), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString()}) ?? "", "PUT"),
                new("delete", Url.Action(nameof(Delete), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString()}) ?? "", "DELETE")
            };
            return Ok(entity.WithLinks(links));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Moto input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            _db.Motos.Add(input);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = input.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, input);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Moto input)
        {
            var entity = await _db.Motos.FindAsync(id);
            if (entity is null) return NotFound();
            entity.Placa = input.Placa;
            entity.Modelo = input.Modelo;
            entity.Ano = input.Ano;
            entity.Status = input.Status;
            entity.PatioId = input.PatioId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Motos.FindAsync(id);
            if (entity is null) return NotFound();
            _db.Motos.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}