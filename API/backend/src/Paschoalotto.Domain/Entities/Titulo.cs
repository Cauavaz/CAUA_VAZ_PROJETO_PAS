namespace Paschoalotto.Domain.Entities;

public class Titulo
{
    private readonly List<Parcela> _parcelas = new();

    public Guid Id { get; set; }
    public string NumeroTitulo { get; set; } = string.Empty;
    public string NomeDevedor { get; set; } = string.Empty;
    public string CpfDevedor { get; set; } = string.Empty;
    public decimal PercentualJuros { get; set; }
    public decimal PercentualMulta { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public IReadOnlyCollection<Parcela> Parcelas => _parcelas;

    public void AdicionarParcela(Parcela parcela)
    {
        parcela.TituloId = Id;
        parcela.Titulo = this;
        _parcelas.Add(parcela);
    }

    public decimal CalcularValorOriginal() => _parcelas.Sum(p => p.Valor);

    public decimal CalcularMulta()
    {
        var valorOriginal = CalcularValorOriginal();
        return Math.Round(valorOriginal * (PercentualMulta / 100m), 2, MidpointRounding.AwayFromZero);
    }

    public decimal CalcularJurosTotal(DateOnly dataReferencia)
    {
        return _parcelas.Sum(p => p.CalcularJuros(PercentualJuros, dataReferencia));
    }

    public int CalcularDiasAtraso(DateOnly dataReferencia)
    {
        if (_parcelas.Count == 0)
        {
            return 0;
        }

        var parcelaMaisAntiga = _parcelas.OrderBy(p => p.DataVencimento).First();
        return parcelaMaisAntiga.CalcularDiasAtraso(dataReferencia);
    }

    public decimal CalcularValorAtualizado(DateOnly dataReferencia)
    {
        return CalcularValorOriginal() + CalcularMulta() + CalcularJurosTotal(dataReferencia);
    }
}
