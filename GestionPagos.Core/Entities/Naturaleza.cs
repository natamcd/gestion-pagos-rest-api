namespace GestionPagos.Core.Entities;

public class Naturaleza
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // "Ingreso", "Egreso"
    public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
