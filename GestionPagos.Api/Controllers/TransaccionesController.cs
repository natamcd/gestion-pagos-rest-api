using GestionPagos.Core.Entities;
using GestionPagos.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionPagos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransaccionesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/transacciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransacciones()
    {
        return await _context.Transacciones
            .Include(t => t.Usuario)
            .Include(t => t.Elemento)
            .Include(t => t.Naturaleza)
            .ToListAsync();
    }

    // GET: api/transacciones/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Transaccion>> GetTransaccion(int id)
    {
        var transaccion = await _context.Transacciones
            .Include(t => t.Usuario)
            .Include(t => t.Elemento)
            .Include(t => t.Naturaleza)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaccion == null)
        {
            return NotFound();
        }

        return transaccion;
    }

    // GET: api/transacciones/usuario/5
    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransaccionesByUsuario(int usuarioId)
    {
        return await _context.Transacciones
            .Include(t => t.Usuario)
            .Include(t => t.Elemento)
            .Include(t => t.Naturaleza)
            .Where(t => t.UsuarioId == usuarioId)
            .ToListAsync();
    }

    // POST: api/transacciones
    [HttpPost]
    public async Task<ActionResult<Transaccion>> PostTransaccion(Transaccion transaccion)
    {
        _context.Transacciones.Add(transaccion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTransaccion), new { id = transaccion.Id }, transaccion);
    }

    // PUT: api/transacciones/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTransaccion(int id, Transaccion transaccion)
    {
        if (id != transaccion.Id)
        {
            return BadRequest();
        }

        _context.Entry(transaccion).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TransaccionExists(id))
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

    // DELETE: api/transacciones/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaccion(int id)
    {
        var transaccion = await _context.Transacciones.FindAsync(id);
        if (transaccion == null)
        {
            return NotFound();
        }

        _context.Transacciones.Remove(transaccion);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TransaccionExists(int id)
    {
        return _context.Transacciones.Any(e => e.Id == id);
    }
}
