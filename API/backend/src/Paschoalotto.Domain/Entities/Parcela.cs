namespace Paschoalotto.Domain.Entities;

public class Parcela
{
    public Guid Id { get; set; }
    public Guid TituloId { get; set; }
    public int Numero { get; set; }
    public DateOnly DataVencimento { get; set; }
    public decimal Valor { get; set; }

    public Titulo? Titulo { get; set; }

    public int CalcularDiasAtraso(DateOnly dataReferencia)
    {
        var diferenca = dataReferencia.DayNumber - DataVencimento.DayNumber;
        return diferenca > 0 ? diferenca : 0;
    }

    public decimal CalcularJuros(decimal percentualJurosMensal, DateOnly dataReferencia)
    {
        var diasAtraso = CalcularDiasAtraso(dataReferencia);
        if (diasAtraso == 0)
        {
            return 0m;
        }

        var jurosDiario = percentualJurosMensal / 100m / 30m;
        var juros = jurosDiario * diasAtraso * Valor;
        return Math.Round(juros, 2, MidpointRounding.AwayFromZero);
    }
}
