using FluentAssertions;
using Paschoalotto.Domain.Entities;
using Xunit;

namespace Paschoalotto.Tests.Domain.Tests.Entities;

public class ParcelaTests
{
    [Fact]
    public void CalcularDiasAtraso_ParcelaVencida_DeveRetornarDiasPositivos()
    {
        // Arrange
        var parcela = new Parcela 
        { 
            DataVencimento = new DateOnly(2026, 4, 12) 
        };
        var dataHoje = new DateOnly(2026, 5, 12); // 30 dias depois

        // Act
        var diasAtraso = parcela.CalcularDiasAtraso(dataHoje);

        // Assert
        diasAtraso.Should().Be(30);
    }

    [Fact]
    public void CalcularDiasAtraso_ParcelaNaoVencida_DeveRetornarZero()
    {
        // Arrange
        var parcela = new Parcela 
        { 
            DataVencimento = new DateOnly(2026, 6, 12) 
        };
        var dataHoje = new DateOnly(2026, 5, 12); // antes do vencimento

        // Act
        var diasAtraso = parcela.CalcularDiasAtraso(dataHoje);

        // Assert
        diasAtraso.Should().Be(0);
    }

    [Fact]
    public void CalcularDiasAtraso_ParcelaVenceHoje_DeveRetornarZero()
    {
        // Arrange
        var parcela = new Parcela 
        { 
            DataVencimento = new DateOnly(2026, 5, 12) 
        };
        var dataHoje = new DateOnly(2026, 5, 12); // mesmo dia

        // Act
        var diasAtraso = parcela.CalcularDiasAtraso(dataHoje);

        // Assert
        diasAtraso.Should().Be(0);
    }

    [Theory]
    [InlineData(100, 1, 30, 1.00)] // 100 * 1% / 30 * 30 dias = 1.00
    [InlineData(1000, 2, 15, 10.00)] // 1000 * 2% / 30 * 15 dias = 10.00
    [InlineData(500, 0.5, 60, 5.00)] // 500 * 0.5% / 30 * 60 dias = 5.00
    public void CalcularJuros_ParcelaVencida_DeveCalcularCorretamente(
        decimal valor, decimal percentualMensal, int diasAtraso, decimal esperado)
    {
        // Arrange
        var parcela = new Parcela 
        { 
            Valor = valor,
            DataVencimento = new DateOnly(2026, 4, 12)
        };
        var dataReferencia = new DateOnly(2026, 4, 12).AddDays(diasAtraso);

        // Act
        var juros = parcela.CalcularJuros(percentualMensal, dataReferencia);

        // Assert
        juros.Should().Be(esperado);
    }

    [Fact]
    public void CalcularJuros_ParcelaNaoVencida_DeveRetornarZero()
    {
        // Arrange
        var parcela = new Parcela 
        { 
            Valor = 1000m,
            DataVencimento = new DateOnly(2026, 6, 12)
        };
        var dataHoje = new DateOnly(2026, 5, 12); // antes do vencimento

        // Act
        var juros = parcela.CalcularJuros(1m, dataHoje);

        // Assert
        juros.Should().Be(0m);
    }
}
