using GestionPagos.Core.Entities;
using GestionPagos.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPagos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElementosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ElementosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/elementos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Elemento>>> GetElementos()
    {
        return await _context.Elementos.ToListAsync();
    }

    // GET: api/elementos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Elemento>> GetElemento(int id)
    {
        var elemento = await _context.Elementos.FindAsync(id);

        if (elemento == null)
        {
            return NotFound();
        }

        return elemento;
    }

    // POST: api/elementos
    [HttpPost]
    public async Task<ActionResult<Elemento>> PostElemento(Elemento elemento)
    {
        _context.Elementos.Add(elemento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetElemento), new { id = elemento.Id }, elemento);
    }

    // PUT: api/elementos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutElemento(int id, Elemento elemento)
    {
        if (id != elemento.Id)
        {
            return BadRequest();
        }

        _context.Entry(elemento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ElementoExists(id))
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

    // DELETE: api/elementos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteElemento(int id)
    {
        var elemento = await _context.Elementos.FindAsync(id);
        if (elemento == null)
        {
            return NotFound();
        }

        _context.Elementos.Remove(elemento);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ElementoExists(int id)
    {
        return _context.Elementos.Any(e => e.Id == id);
    }
}
