using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionPagos.Infrastructure.Data;
using GestionPagos.Core.Entities;
using GestionPagos.Api.Models.Auth;
using GestionPagos.Api.Services;
using BCrypt.Net;

namespace GestionPagos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterDto dto)
    {
        // Validar si el email ya existe
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest(new { message = "El email ya está registrado" });
        }

        // Validar contraseña (mínimo 6 caracteres)
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
        {
            return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres" });
        }

        // Crear usuario con contraseña hasheada (cost factor 11)
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, 11),
            FechaCreacion = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Generar token
        var token = _jwtService.GenerateToken(usuario);

        return Ok(new AuthResponse
        {
            Token = token,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                FechaCreacion = usuario.FechaCreacion
            }
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto dto)
    {
        // Buscar usuario por email
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario == null)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        // Verificar contraseña
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        // Generar token
        var token = _jwtService.GenerateToken(usuario);

        return Ok(new AuthResponse
        {
            Token = token,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                FechaCreacion = usuario.FechaCreacion
            }
        });
    }

    [HttpGet("validate")]
    public IActionResult ValidateToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(new { message = "Token no proporcionado" });
        }

        var principal = _jwtService.ValidateToken(token);
        
        if (principal == null)
        {
            return Unauthorized(new { message = "Token inválido o expirado" });
        }

        return Ok(new { message = "Token válido", claims = principal.Claims.Select(c => new { c.Type, c.Value }) });
    }
}
