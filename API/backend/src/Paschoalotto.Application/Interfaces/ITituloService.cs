using Paschoalotto.Application.DTOs.Titulos;

namespace Paschoalotto.Application.Interfaces;

public interface ITituloService
{
    Task<TituloListagemResponse> CriarAsync(CriarTituloRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TituloListagemResponse>> ListarAsync(CancellationToken cancellationToken = default);
}
