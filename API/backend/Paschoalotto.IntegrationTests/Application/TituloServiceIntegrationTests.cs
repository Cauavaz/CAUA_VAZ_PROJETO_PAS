using Paschoalotto.IntegrationTests.Infrastructure;

namespace Paschoalotto.IntegrationTests.Application;

/// <summary>
/// Testes de integração para TituloService com Repository e Database.
/// Valida o fluxo completo: Service -> Repository -> EF Core -> InMemory Database.
/// </summary>
public class TituloServiceIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public TituloServiceIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DevePersistirTituloNoBanco()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);
        var service = new TituloService(repository);

        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-INT-001",
            NomeDevedor = "Maria Santos",
            CpfDevedor = "111.444.777-35",
            PercentualJuros = 1.5m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 15), Valor = 500m },
                new() { Numero = 2, DataVencimento = new DateOnly(2026, 7, 15), Valor = 500m }
            }
        };

        // Act
        var resultado = await service.CriarAsync(request);

        // Assert - Verifica o retorno do service
        resultado.Should().NotBeNull();
        resultado.NumeroTitulo.Should().Be("T-INT-001");
        resultado.NomeDevedor.Should().Be("Maria Santos");
        resultado.CpfDevedor.Should().Be("11144477735"); // CPF sanitizado
        resultado.QuantidadeParcelas.Should().Be(2);
        resultado.ValorOriginal.Should().Be(1000m);

        // Assert - Verifica se foi realmente persistido no banco
        var tituloSalvo = await context.Titulos
            .Include(t => t.Parcelas)
            .FirstOrDefaultAsync(t => t.NumeroTitulo == "T-INT-001");

        tituloSalvo.Should().NotBeNull();
        tituloSalvo!.NomeDevedor.Should().Be("Maria Santos");
        tituloSalvo.Parcelas.Should().HaveCount(2);
        tituloSalvo.Parcelas.Sum(p => p.Valor).Should().Be(1000m);
    }

    [Fact]
    public async Task CriarAsync_ComNumeroDuplicado_DeveLancarExcecao()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);
        var service = new TituloService(repository);

        // Criar primeiro título
        var primeiroTitulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-DUP-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m
        };
        context.Titulos.Add(primeiroTitulo);
        await context.SaveChangesAsync();

        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-DUP-001", // Mesmo número
            NomeDevedor = "Maria Santos",
            CpfDevedor = "111.444.777-35",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 15), Valor = 1000m }
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CriarAsync(request));

        exception.Message.Should().Contain("Já existe um título com o número");
    }

    [Fact]
    public async Task ListarAsync_ComTitulosNoBanco_DeveRetornarListaOrdenada()
    {
        // Arrange
        using var context = _fixture.CriarContextoComDados();
        var repository = new TituloRepository(context);
        var service = new TituloService(repository);

        // Adicionar mais títulos para teste
        var titulo2 = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-LIST-002",
            NomeDevedor = "Pedro Oliveira",
            CpfDevedor = "11144477735",
            PercentualJuros = 2m,
            PercentualMulta = 15m,
            CriadoEm = DateTime.UtcNow.AddMinutes(1)
        };
        titulo2.AdicionarParcela(new Parcela
        {
            Id = Guid.NewGuid(),
            Numero = 1,
            DataVencimento = new DateOnly(2026, 6, 15),
            Valor = 2000m
        });
        context.Titulos.Add(titulo2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await service.ListarAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCountGreaterThanOrEqualTo(2);
        
        // Verifica se está ordenado por data de criação (mais recente primeiro)
        resultado.First().NumeroTitulo.Should().Be("T-LIST-002");
        
        // Verifica se os cálculos estão corretos
        var primeiroTitulo = resultado.First();
        primeiroTitulo.ValorOriginal.Should().Be(2000m);
        primeiroTitulo.QuantidadeParcelas.Should().Be(1);
    }

    [Fact]
    public async Task CriarAsync_ComCpfInvalido_DeveLancarExcecao()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);
        var service = new TituloService(repository);

        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-CPF-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "111.111.111-11", // CPF inválido
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 15), Valor = 1000m }
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CriarAsync(request));

        exception.Message.Should().Contain("CPF do devedor é inválido");
    }

    [Fact]
    public async Task CriarAsync_ComParcelasComValoresCorretos_DeveCalcularValorOriginalCorretamente()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);
        var service = new TituloService(repository);

        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-CALC-001",
            NomeDevedor = "Ana Costa",
            CpfDevedor = "111.444.777-35",
            PercentualJuros = 2m,
            PercentualMulta = 5m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 15), Valor = 300m },
                new() { Numero = 2, DataVencimento = new DateOnly(2026, 7, 15), Valor = 400m },
                new() { Numero = 3, DataVencimento = new DateOnly(2026, 8, 15), Valor = 500m }
            }
        };

        // Act
        var resultado = await service.CriarAsync(request);

        // Assert
        resultado.ValorOriginal.Should().Be(1200m); // 300 + 400 + 500
        resultado.QuantidadeParcelas.Should().Be(3);

        // Verifica no banco
        var tituloSalvo = await context.Titulos
            .Include(t => t.Parcelas)
            .FirstAsync(t => t.NumeroTitulo == "T-CALC-001");

        tituloSalvo.CalcularValorOriginal().Should().Be(1200m);
        tituloSalvo.Parcelas.Should().HaveCount(3);
    }

    [Fact]
    public async Task CriarAsync_ComMultiplasParcelas_DeveManterRelacionamentoCorreto()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);
        var service = new TituloService(repository);

        var request = new CriarTituloRequest
        {
            NumeroTitulo = "T-REL-001",
            NomeDevedor = "Carlos Mendes",
            CpfDevedor = "111.444.777-35",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            Parcelas = new List<CriarParcelaRequest>
            {
                new() { Numero = 1, DataVencimento = new DateOnly(2026, 6, 15), Valor = 1000m },
                new() { Numero = 2, DataVencimento = new DateOnly(2026, 7, 15), Valor = 1000m },
                new() { Numero = 3, DataVencimento = new DateOnly(2026, 8, 15), Valor = 1000m }
            }
        };

        // Act
        await service.CriarAsync(request);

        // Assert - Verifica relacionamento no banco
        var tituloSalvo = await context.Titulos
            .Include(t => t.Parcelas)
            .FirstAsync(t => t.NumeroTitulo == "T-REL-001");

        tituloSalvo.Parcelas.Should().HaveCount(3);
        
        // Verifica se todas as parcelas têm o TituloId correto
        foreach (var parcela in tituloSalvo.Parcelas)
        {
            parcela.TituloId.Should().Be(tituloSalvo.Id);
        }

        // Verifica se as parcelas estão ordenadas
        var parcelas = tituloSalvo.Parcelas.OrderBy(p => p.Numero).ToList();
        parcelas[0].Numero.Should().Be(1);
        parcelas[1].Numero.Should().Be(2);
        parcelas[2].Numero.Should().Be(3);
    }
}
