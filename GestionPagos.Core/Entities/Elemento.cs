namespace GestionPagos.Core.Entities;

public class Elemento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
