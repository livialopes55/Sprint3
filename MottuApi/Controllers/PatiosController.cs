using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using MottuApi.Models;
using MottuApi.Utils;

namespace MottuApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/patios")]
public class PatiosController : ControllerBase
{
    private readonly AppDbContext _db;
    public PatiosController(AppDbContext db) => _db = db;

    // GET /api/v1/patios?pageNumber=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Patio>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var query = _db.Patios
            .AsNoTracking()
            .OrderBy(p => p.Id);

        var total = await query.CountAsync();
        var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var links = BuildLinks(pageNumber, pageSize, total);

        var result = new PagedResult<Patio>
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
            new("create", Url.Action(nameof(Create), values: new { version = "1" })!, "POST")
        };

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        if (pageNumber > 1)
            list.Add(new("prev", Url.Action(nameof(GetAll), values: new { version = "1", pageNumber = pageNumber - 1, pageSize })!, "GET"));
        if (pageNumber < totalPages)
            list.Add(new("next", Url.Action(nameof(GetAll), values: new { version = "1", pageNumber = pageNumber + 1, pageSize })!, "GET"));

        return list;
    }

    // GET /api/v1/patios/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Patio), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var patio = await _db.Patios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return patio is null ? NotFound() : Ok(patio);
    }

    // POST /api/v1/patios
    [HttpPost]
    [ProducesResponseType(typeof(Patio), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Patio input)
    {
        // valida FK
        var filialExists = await _db.Filiais.AnyAsync(f => f.Id == input.FilialId);
        if (!filialExists) return BadRequest(new { message = "FilialId inválido." });

        _db.Patios.Add(input);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = input.Id, version = "1" }, input);
    }

    // PUT /api/v1/patios/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] Patio input)
    {
        var entity = await _db.Patios.FindAsync(id);
        if (entity is null) return NotFound();

        if (!await _db.Filiais.AnyAsync(f => f.Id == input.FilialId))
            return BadRequest(new { message = "FilialId inválido." });

        entity.Descricao = input.Descricao;
        entity.Dimensao = input.Dimensao;
        entity.FilialId = input.FilialId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/v1/patios/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Patios.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Patios.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
