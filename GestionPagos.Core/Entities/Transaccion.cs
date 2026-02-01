namespace GestionPagos.Core.Entities;

public class Transaccion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int ElementoId { get; set; }
    public Elemento Elemento { get; set; } = null!;
    public int NaturalezaId { get; set; }
    public Naturaleza Naturaleza { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
