namespace GestionPagos.Core.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
