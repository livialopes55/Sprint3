using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MottuApi.Data;
using MottuApi.Models;
using MottuApi.Utils;

namespace MottuApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/filiais")]
    public class FiliaisController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FiliaisController(AppDbContext db) => _db = db;

        /// <summary>Lista filiais com paginação</summary>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = _db.Filiais.AsNoTracking().OrderBy(f => f.Id);
            var paged = query.ToPagedResult(pageNumber, pageSize);

            var links = new List<Link>
            {
                new("self", Url.Action(nameof(GetAll), values: new { pageNumber, pageSize, version = HttpContext.GetRequestedApiVersion()?.ToString()} ) ?? "", "GET"),
                new("create", Url.Action(nameof(Create), values: new { version = HttpContext.GetRequestedApiVersion()?.ToString() }) ?? "", "POST")
            };

            return Ok(new { data = paged.Items, total = paged.TotalCount, pageNumber, pageSize, links });
        }

        /// <summary>Busca filial por id</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _db.Filiais.FindAsync(id);
            if (entity is null) return NotFound();

            var links = new List<Link>
            {
                new("self", Url.Action(nameof(GetById), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString() }) ?? "", "GET"),
                new("update", Url.Action(nameof(Update), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString()}) ?? "", "PUT"),
                new("delete", Url.Action(nameof(Delete), values: new { id, version = HttpContext.GetRequestedApiVersion()?.ToString()}) ?? "", "DELETE")
            };

            return Ok(entity.WithLinks(links));
        }

        /// <summary>Cria nova filial</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Filial), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Filial input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            _db.Filiais.Add(input);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = input.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() }, input);
        }

        /// <summary>Atualiza filial</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Filial input)
        {
            var entity = await _db.Filiais.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Nome = input.Nome;
            entity.Endereco = input.Endereco;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Exclui filial</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Filiais.FindAsync(id);
            if (entity is null) return NotFound();
            _db.Filiais.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}