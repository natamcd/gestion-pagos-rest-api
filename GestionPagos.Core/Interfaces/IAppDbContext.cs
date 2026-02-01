using GestionPagos.Core.Entities;

namespace GestionPagos.Core.Interfaces;

public interface IAppDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
