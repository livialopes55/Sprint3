using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using MottuApi.Models;
using MottuApi.Utils;

namespace MottuApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/filiais")]
public class FiliaisController : ControllerBase
{
    private readonly AppDbContext _db;
    public FiliaisController(AppDbContext db) => _db = db;

    // GET /api/v1/filiais?pageNumber=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Filial>), 200)]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var query = _db.Filiais.AsNoTracking().OrderBy(f => f.Id);

        var total = await query.CountAsync();
        var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var links = new List<Link>
        {
            new("self",   Url.Action(nameof(Get),    values: new { version="1", pageNumber, pageSize })!, "GET"),
            new("create", Url.Action(nameof(Create), values: new { version="1" })!,                       "POST")
        };

        var result = new PagedResult<Filial>
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

    // GET /api/v1/filiais/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Filial), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var filial = await _db.Filiais.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (filial is null) return NotFound();

        return Ok(filial);
    }

    // POST /api/v1/filiais
    [HttpPost]
    [ProducesResponseType(typeof(Filial), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Filial model)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        _db.Filiais.Add(model);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id, version = "1" }, model);
    }

    // PUT /api/v1/filiais/{id}
    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] Filial input)
    {
        var entity = await _db.Filiais.FirstOrDefaultAsync(f => f.Id == id);
        if (entity is null) return NotFound();

        entity.Nome = input.Nome;
        entity.Endereco = input.Endereco;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/v1/filiais/{id}
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Filiais.FirstOrDefaultAsync(f => f.Id == id);
        if (entity is null) return NotFound();

        _db.Filiais.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
