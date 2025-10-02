using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using MottuApi.Models;
using MottuApi.Utils;

namespace MottuApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/motos")]
public class MotosController : ControllerBase
{
    private readonly AppDbContext _db;
    public MotosController(AppDbContext db) => _db = db;

    // GET /api/v1/motos?pageNumber=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Moto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var query = _db.Motos
            .AsNoTracking()
            .OrderBy(m => m.Id);

        var total = await query.CountAsync();
        var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var links = BuildLinks(pageNumber, pageSize, total);

        var result = new PagedResult<Moto>
        {
            Data = data,
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Links = links
        };

        Response.Headers["X-Pagination"] =
            System.Text.Json.JsonSerializer.Serialize(new { total, pageNumber, pageSize });

        return Ok(result);
    }

    private IEnumerable<Link> BuildLinks(int pageNumber, int pageSize, int total)
    {
        var list = new List<Link>
        {
            new("self",   Url.Action(nameof(GetAll), values: new { version = "1", pageNumber, pageSize })!, "GET"),
            new("create", Url.Action(nameof(Create),  values: new { version = "1" })!, "POST")
        };

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        if (pageNumber > 1)
            list.Add(new("prev", Url.Action(nameof(GetAll), values: new { version = "1", pageNumber = pageNumber - 1, pageSize })!, "GET"));
        if (pageNumber < totalPages)
            list.Add(new("next", Url.Action(nameof(GetAll), values: new { version = "1", pageNumber = pageNumber + 1, pageSize })!, "GET"));

        return list;
    }

    // GET /api/v1/motos/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Moto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var moto = await _db.Motos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return moto is null ? NotFound() : Ok(moto);
    }

    // POST /api/v1/motos
    [HttpPost]
    [ProducesResponseType(typeof(Moto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Moto input)
    {
        // valida FK
        var patioExists = await _db.Patios.AnyAsync(p => p.Id == input.PatioId);
        if (!patioExists) return BadRequest(new { message = "PatioId inválido." });

        _db.Motos.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = input.Id, version = "1" }, input);
    }

    // PUT /api/v1/motos/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] Moto input)
    {
        var entity = await _db.Motos.FindAsync(id);
        if (entity is null) return NotFound();

        if (!await _db.Patios.AnyAsync(p => p.Id == input.PatioId))
            return BadRequest(new { message = "PatioId inválido." });

        entity.Placa = input.Placa;
        entity.Modelo = input.Modelo;
        entity.Ano = input.Ano;
        entity.Status = input.Status;
        entity.PatioId = input.PatioId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/v1/motos/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Motos.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Motos.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}