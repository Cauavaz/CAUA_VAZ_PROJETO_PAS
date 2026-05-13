namespace Paschoalotto.Application.DTOs.Titulos;

public record TituloListagemResponse(
    Guid Id,
    string NumeroTitulo,
    string NomeDevedor,
    string CpfDevedor,
    int QuantidadeParcelas,
    decimal ValorOriginal,
    int DiasAtraso,
    decimal ValorAtualizado,
    DateOnly DataAtualizacao
);
