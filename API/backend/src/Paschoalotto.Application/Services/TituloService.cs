using Paschoalotto.Application.DTOs.Titulos;
using Paschoalotto.Application.Interfaces;
using Paschoalotto.Application.Validation;
using Paschoalotto.Domain.Entities;

namespace Paschoalotto.Application.Services;

public class TituloService : ITituloService
{
    private readonly ITituloRepository _repository;

    public TituloService(ITituloRepository repository)
    {
        _repository = repository;
    }

    public async Task<TituloListagemResponse> CriarAsync(CriarTituloRequest request, CancellationToken cancellationToken = default)
    {
        Validar(request);

        if (await _repository.ExisteNumeroAsync(request.NumeroTitulo, cancellationToken))
        {
            throw new InvalidOperationException($"Já existe um título com o número '{request.NumeroTitulo}'.");
        }

        var titulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = request.NumeroTitulo.Trim(),
            NomeDevedor = request.NomeDevedor.Trim(),
            CpfDevedor = CpfValidator.Sanitize(request.CpfDevedor),
            PercentualJuros = request.PercentualJuros,
            PercentualMulta = request.PercentualMulta,
            CriadoEm = DateTime.UtcNow
        };

        foreach (var parcelaRequest in request.Parcelas)
        {
            titulo.AdicionarParcela(new Parcela
            {
                Id = Guid.NewGuid(),
                Numero = parcelaRequest.Numero,
                DataVencimento = parcelaRequest.DataVencimento,
                Valor = parcelaRequest.Valor
            });
        }

        await _repository.AdicionarAsync(titulo, cancellationToken);

        return Mapear(titulo, DataDeHoje());
    }

    public async Task<IReadOnlyList<TituloListagemResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var titulos = await _repository.ListarAsync(cancellationToken);
        var hoje = DataDeHoje();
        return titulos.Select(t => Mapear(t, hoje)).ToList();
    }

    private static void Validar(CriarTituloRequest request)
    {
        if (!CpfValidator.IsValid(request.CpfDevedor))
        {
            throw new InvalidOperationException("CPF do devedor é inválido.");
        }

        if (request.PercentualJuros < 0)
        {
            throw new InvalidOperationException("O percentual de juros não pode ser negativo.");
        }

        if (request.PercentualMulta < 0)
        {
            throw new InvalidOperationException("O percentual de multa não pode ser negativo.");
        }

        if (request.Parcelas is null || request.Parcelas.Count == 0)
        {
            throw new InvalidOperationException("Informe ao menos uma parcela.");
        }

        var numeros = new HashSet<int>();
        foreach (var parcela in request.Parcelas)
        {
            if (parcela.Numero <= 0)
            {
                throw new InvalidOperationException("Número da parcela deve ser maior que zero.");
            }

            if (!numeros.Add(parcela.Numero))
            {
                throw new InvalidOperationException($"Existem parcelas duplicadas com o número {parcela.Numero}.");
            }

            if (parcela.Valor <= 0)
            {
                throw new InvalidOperationException("O valor da parcela deve ser maior que zero.");
            }
        }
    }

    private static TituloListagemResponse Mapear(Titulo titulo, DateOnly dataReferencia)
    {
        return new TituloListagemResponse(
            Id: titulo.Id,
            NumeroTitulo: titulo.NumeroTitulo,
            NomeDevedor: titulo.NomeDevedor,
            CpfDevedor: titulo.CpfDevedor,
            QuantidadeParcelas: titulo.Parcelas.Count,
            ValorOriginal: titulo.CalcularValorOriginal(),
            DiasAtraso: titulo.CalcularDiasAtraso(dataReferencia),
            ValorAtualizado: titulo.CalcularValorAtualizado(dataReferencia),
            DataAtualizacao: dataReferencia
        );
    }

    private static DateOnly DataDeHoje() => DateOnly.FromDateTime(DateTime.Today);
}
