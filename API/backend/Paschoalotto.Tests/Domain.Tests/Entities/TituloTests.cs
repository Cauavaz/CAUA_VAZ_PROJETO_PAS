using FluentAssertions;
using Paschoalotto.Domain.Entities;
using Xunit;

namespace Paschoalotto.Tests.Domain.Tests.Entities;

public class TituloTests
{
    [Fact]
    public void CalcularValorOriginal_DeveSomarValoresDasParcelas()
    {
        // Arrange
        var titulo = new Titulo();
        titulo.AdicionarParcela(new Parcela { Valor = 100m });
        titulo.AdicionarParcela(new Parcela { Valor = 200m });
        titulo.AdicionarParcela(new Parcela { Valor = 300m });

        // Act
        var valorOriginal = titulo.CalcularValorOriginal();

        // Assert
        valorOriginal.Should().Be(600m);
    }

    [Fact]
    public void CalcularValorOriginal_SemParcelas_DeveRetornarZero()
    {
        // Arrange
        var titulo = new Titulo();

        // Act
        var valorOriginal = titulo.CalcularValorOriginal();

        // Assert
        valorOriginal.Should().Be(0m);
    }

    [Theory]
    [InlineData(1000, 10, 100)] // 1000 * 10%
    [InlineData(500, 5, 25)]    // 500 * 5%
    [InlineData(2000, 2, 40)]  // 2000 * 2%
    public void CalcularMulta_DeveCalcularPercentualSobreValorOriginal(
        decimal valorOriginal, decimal percentualMulta, decimal esperado)
    {
        // Arrange
        var titulo = new Titulo 
        { 
            PercentualMulta = percentualMulta 
        };
        
        // Adiciona parcelas para ter valor original
        titulo.AdicionarParcela(new Parcela { Valor = valorOriginal });

        // Act
        var multa = titulo.CalcularMulta();

        // Assert
        multa.Should().Be(esperado);
    }

    [Fact]
    public void CalcularDiasAtraso_DeveRetornarDiasDaParcelaMaisAntiga()
    {
        // Arrange
        var titulo = new Titulo();
        var parcela1 = new Parcela { DataVencimento = new DateOnly(2026, 4, 12) };
        var parcela2 = new Parcela { DataVencimento = new DateOnly(2026, 3, 12) }; // mais antiga
        var parcela3 = new Parcela { DataVencimento = new DateOnly(2026, 5, 12) };
        
        titulo.AdicionarParcela(parcela1);
        titulo.AdicionarParcela(parcela2);
        titulo.AdicionarParcela(parcela3);

        var dataHoje = new DateOnly(2026, 5, 12);

        // Act
        var diasAtraso = titulo.CalcularDiasAtraso(dataHoje);

        // Assert
        // Parcela mais antiga (março) tem 61 dias de atraso em maio
        diasAtraso.Should().Be(61);
    }

    [Fact]
    public void CalcularValorAtualizado_DeveSomarOriginalMultaJuros()
    {
        // Arrange
        var titulo = new Titulo 
        { 
            PercentualMulta = 10m,
            PercentualJuros = 1m
        };
        
        // Parcela vencida há 30 dias
        var parcela = new Parcela 
        { 
            Valor = 1000m,
            DataVencimento = new DateOnly(2026, 4, 12)
        };
        titulo.AdicionarParcela(parcela);

        var dataHoje = new DateOnly(2026, 5, 12); // 30 dias depois

        // Act
        var valorAtualizado = titulo.CalcularValorAtualizado(dataHoje);

        // Assert
        // Original: 1000
        // Multa: 1000 * 10% = 100
        // Juros: 1000 * 1% / 30 * 30 = 10
        // Total: 1000 + 100 + 10 = 1110
        valorAtualizado.Should().Be(1110m);
    }

    [Fact]
    public void AdicionarParcela_DeveConfigurarRelacionamento()
    {
        // Arrange
        var titulo = new Titulo { Id = Guid.NewGuid() };
        var parcela = new Parcela { Id = Guid.NewGuid() };

        // Act
        titulo.AdicionarParcela(parcela);

        // Assert
        parcela.TituloId.Should().Be(titulo.Id);
        parcela.Titulo.Should().Be(titulo);
        titulo.Parcelas.Should().Contain(parcela);
    }
}
