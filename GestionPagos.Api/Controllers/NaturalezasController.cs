using GestionPagos.Core.Entities;
using GestionPagos.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPagos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NaturalezasController : ControllerBase
{
    private readonly AppDbContext _context;

    public NaturalezasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/naturalezas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Naturaleza>>> GetNaturalezas()
    {
        return await _context.Naturalezas.ToListAsync();
    }

    // GET: api/naturalezas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Naturaleza>> GetNaturaleza(int id)
    {
        var naturaleza = await _context.Naturalezas.FindAsync(id);

        if (naturaleza == null)
        {
            return NotFound();
        }

        return naturaleza;
    }

    // POST: api/naturalezas
    [HttpPost]
    public async Task<ActionResult<Naturaleza>> PostNaturaleza(Naturaleza naturaleza)
    {
        _context.Naturalezas.Add(naturaleza);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetNaturaleza), new { id = naturaleza.Id }, naturaleza);
    }

    // PUT: api/naturalezas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutNaturaleza(int id, Naturaleza naturaleza)
    {
        if (id != naturaleza.Id)
        {
            return BadRequest();
        }

        _context.Entry(naturaleza).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!NaturalezaExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/naturalezas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNaturaleza(int id)
    {
        var naturaleza = await _context.Naturalezas.FindAsync(id);
        if (naturaleza == null)
        {
            return NotFound();
        }

        _context.Naturalezas.Remove(naturaleza);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool NaturalezaExists(int id)
    {
        return _context.Naturalezas.Any(e => e.Id == id);
    }
}
