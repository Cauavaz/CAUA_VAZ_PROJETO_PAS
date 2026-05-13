using Paschoalotto.Domain.Entities;

namespace Paschoalotto.Application.Interfaces;

public interface ITituloRepository
{
    Task<Titulo> AdicionarAsync(Titulo titulo, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Titulo>> ListarAsync(CancellationToken cancellationToken = default);
    Task<bool> ExisteNumeroAsync(string numeroTitulo, CancellationToken cancellationToken = default);
}
