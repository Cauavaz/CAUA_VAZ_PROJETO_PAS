using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Paschoalotto.Application.DTOs.Titulos;
using Paschoalotto.Application.Interfaces;
using Paschoalotto.Application.Services;
using Paschoalotto.Domain.Entities;
using Xunit;

namespace Paschoalotto.Tests.Application.Tests.Services;

public class TituloServiceTests
{
    private readonly Mock<ITituloRepository> _repositoryMock;
    private readonly TituloService _tituloService;

    public TituloServiceTests()
    {
        _repositoryMock = new Mock<ITituloRepository>();
        _tituloService = new TituloService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarTituloComSucesso()
    {
        // Arrange
        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "111.444.777-35", // CPF válido
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 12), Valor = 1000m },
                new() { Numero = 2, DataVencimento = new DateOnly(2026, 7, 12), Valor = 1000m }
            }
        };

        _repositoryMock.Setup(r => r.ExisteNumeroAsync(request.NumeroTitulo, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        Titulo tituloSalvo = null;
        _repositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Titulo>(), It.IsAny<CancellationToken>()))
                      .Callback<Titulo, CancellationToken>((t, ct) => tituloSalvo = t)
                      .ReturnsAsync((Titulo t, CancellationToken ct) => t);

        // Act
        var resultado = await _tituloService.CriarAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NumeroTitulo.Should().Be(request.NumeroTitulo);
        resultado.NomeDevedor.Should().Be(request.NomeDevedor);
        resultado.CpfDevedor.Should().Be("11144477735"); // CPF sanitizado
        resultado.QuantidadeParcelas.Should().Be(2);
        resultado.ValorOriginal.Should().Be(2000m);

        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Titulo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_CpfInvalido_DeveLancarExcecao()
    {
        // Arrange
        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "123.456.789-00", // CPF inválido
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 12), Valor = 1000m }
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tituloService.CriarAsync(request));

        exception.Message.Should().Be("CPF do devedor é inválido.");
    }

    [Fact]
    public async Task CriarAsync_NumeroDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "111.444.777-35", // CPF válido
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 12), Valor = 1000m }
            }
        };

        _repositoryMock.Setup(r => r.ExisteNumeroAsync(request.NumeroTitulo, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tituloService.CriarAsync(request));

        exception.Message.Should().Be("Já existe um título com o número 'T-001'.");
    }

    [Fact]
    public async Task CriarAsync_SemParcelas_DeveLancarExcecao()
    {
        // Arrange
        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "111.444.777-35", // CPF válido
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = null // Sem parcelas
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tituloService.CriarAsync(request));

        exception.Message.Should().Be("Informe ao menos uma parcela.");
    }

    [Fact]
    public async Task CriarAsync_ParcelasDuplicadas_DeveLancarExcecao()
    {
        // Arrange
        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "111.444.777-35", // CPF válido
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 12), Valor = 1000m },
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 7, 12), Valor = 2000m } // Número duplicado
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tituloService.CriarAsync(request));

        exception.Message.Should().Be("Existem parcelas duplicadas com o número 1.");
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarListaDeTitulos()
    {
        // Arrange
        var titulos = new List<Titulo>
        {
            new() 
            { 
                Id = Guid.NewGuid(),
                NumeroTitulo = "T-001",
                NomeDevedor = "João Silva",
                CpfDevedor = "12345678909",
                PercentualJuros = 1m,
                PercentualMulta = 10m,
                CriadoEm = DateTime.UtcNow
            }
        };

        // Adiciona parcela ao título
        titulos[0].AdicionarParcela(new Parcela 
        { 
            Valor = 1000m, 
            DataVencimento = new DateOnly(2026, 4, 12) 
        });

        _repositoryMock.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(titulos);

        // Act
        var resultado = await _tituloService.ListarAsync();

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].NumeroTitulo.Should().Be("T-001");
        resultado[0].NomeDevedor.Should().Be("João Silva");
        resultado[0].QuantidadeParcelas.Should().Be(1);
        resultado[0].ValorOriginal.Should().Be(1000m);

        _repositoryMock.Verify(r => r.ListarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
