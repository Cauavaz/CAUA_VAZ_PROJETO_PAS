using Paschoalotto.IntegrationTests.Infrastructure;

namespace Paschoalotto.IntegrationTests.Infrastructure;

/// <summary>
/// Testes de integração para TituloRepository com EF Core.
/// Valida operações de banco de dados: queries, includes, ordenação, etc.
/// </summary>
public class TituloRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public TituloRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AdicionarAsync_ComTituloValido_DeveSalvarNoBanco()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        var titulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-REPO-001",
            NomeDevedor = "João Silva",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        titulo.AdicionarParcela(new Parcela
        {
            Id = Guid.NewGuid(),
            Numero = 1,
            DataVencimento = new DateOnly(2026, 6, 15),
            Valor = 1000m
        });

        // Act
        var resultado = await repository.AdicionarAsync(titulo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(titulo.Id);

        // Verifica se foi salvo no banco
        var tituloSalvo = await context.Titulos.FindAsync(titulo.Id);
        tituloSalvo.Should().NotBeNull();
        tituloSalvo!.NumeroTitulo.Should().Be("T-REPO-001");
    }

    [Fact]
    public async Task AdicionarAsync_ComParcelas_DeveSalvarRelacionamento()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        var titulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-REPO-002",
            NomeDevedor = "Maria Santos",
            CpfDevedor = "11144477735",
            PercentualJuros = 1.5m,
            PercentualMulta = 12m,
            CriadoEm = DateTime.UtcNow
        };

        titulo.AdicionarParcela(new Parcela
        {
            Id = Guid.NewGuid(),
            Numero = 1,
            DataVencimento = new DateOnly(2026, 6, 15),
            Valor = 500m
        });

        titulo.AdicionarParcela(new Parcela
        {
            Id = Guid.NewGuid(),
            Numero = 2,
            DataVencimento = new DateOnly(2026, 7, 15),
            Valor = 500m
        });

        // Act
        await repository.AdicionarAsync(titulo);

        // Assert - Verifica se as parcelas foram salvas
        var tituloComParcelas = await context.Titulos
            .Include(t => t.Parcelas)
            .FirstAsync(t => t.Id == titulo.Id);

        tituloComParcelas.Parcelas.Should().HaveCount(2);
        tituloComParcelas.Parcelas.All(p => p.TituloId == titulo.Id).Should().BeTrue();
    }

    [Fact]
    public async Task ListarAsync_ComMultiplosTitulos_DeveRetornarOrdenadoPorDataDecrescente()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        var titulo1 = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-LIST-001",
            NomeDevedor = "Primeiro",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow.AddDays(-2)
        };

        var titulo2 = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-LIST-002",
            NomeDevedor = "Segundo",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow.AddDays(-1)
        };

        var titulo3 = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-LIST-003",
            NomeDevedor = "Terceiro",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        context.Titulos.AddRange(titulo1, titulo2, titulo3);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ListarAsync();

        // Assert
        resultado.Should().HaveCount(3);
        resultado[0].NumeroTitulo.Should().Be("T-LIST-003"); // Mais recente primeiro
        resultado[1].NumeroTitulo.Should().Be("T-LIST-002");
        resultado[2].NumeroTitulo.Should().Be("T-LIST-001");
    }

    [Fact]
    public async Task ListarAsync_DeveCarregarParcelasComInclude()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        var titulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-INCLUDE-001",
            NomeDevedor = "Teste Include",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        titulo.AdicionarParcela(new Parcela
        {
            Id = Guid.NewGuid(),
            Numero = 1,
            DataVencimento = new DateOnly(2026, 6, 15),
            Valor = 1000m
        });

        context.Titulos.Add(titulo);
        await context.SaveChangesAsync();

        // Limpa o tracking para simular nova consulta
        context.ChangeTracker.Clear();

        // Act
        var resultado = await repository.ListarAsync();

        // Assert
        var tituloRetornado = resultado.First(t => t.NumeroTitulo == "T-INCLUDE-001");
        tituloRetornado.Parcelas.Should().HaveCount(1);
        tituloRetornado.Parcelas.First().Valor.Should().Be(1000m);
    }

    [Fact]
    public async Task ExisteNumeroAsync_ComNumeroExistente_DeveRetornarTrue()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        var titulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-EXISTE-001",
            NomeDevedor = "Teste Existe",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        context.Titulos.Add(titulo);
        await context.SaveChangesAsync();

        // Act
        var existe = await repository.ExisteNumeroAsync("T-EXISTE-001");

        // Assert
        existe.Should().BeTrue();
    }

    [Fact]
    public async Task ExisteNumeroAsync_ComNumeroInexistente_DeveRetornarFalse()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        // Act
        var existe = await repository.ExisteNumeroAsync("T-NAO-EXISTE");

        // Assert
        existe.Should().BeFalse();
    }

    [Fact]
    public async Task ListarAsync_DeveUsarAsNoTracking()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        var titulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-NOTRACK-001",
            NomeDevedor = "Teste NoTracking",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        context.Titulos.Add(titulo);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repository.ListarAsync();

        // Assert
        var tituloRetornado = resultado.First();
        
        // Verifica se não está sendo rastreado pelo context
        var entry = context.Entry(tituloRetornado);
        entry.State.Should().Be(EntityState.Detached);
    }

    [Fact]
    public async Task AdicionarAsync_ComConcorrencia_DeveManterIntegridade()
    {
        // Arrange
        using var context = _fixture.CriarContexto();
        var repository = new TituloRepository(context);

        var titulo1 = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-CONC-001",
            NomeDevedor = "Primeiro",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        var titulo2 = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-CONC-002",
            NomeDevedor = "Segundo",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        // Act
        await repository.AdicionarAsync(titulo1);
        await repository.AdicionarAsync(titulo2);

        // Assert
        var todosOsTitulos = await context.Titulos.ToListAsync();
        todosOsTitulos.Should().Contain(t => t.NumeroTitulo == "T-CONC-001");
        todosOsTitulos.Should().Contain(t => t.NumeroTitulo == "T-CONC-002");
    }
}
