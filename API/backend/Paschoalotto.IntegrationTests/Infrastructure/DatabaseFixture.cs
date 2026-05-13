namespace Paschoalotto.IntegrationTests.Infrastructure;

/// <summary>
/// Fixture para criar e gerenciar banco de dados em memória para testes de integração.
/// Segue o padrão xUnit IDisposable para limpeza automática.
/// </summary>
public class DatabaseFixture : IDisposable
{
    private bool _disposed = false;

    /// <summary>
    /// Cria um novo contexto de banco de dados em memória para cada teste.
    /// Usa InMemory Database do EF Core para isolamento total entre testes.
    /// </summary>
    public AppDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new AppDbContext(options);
        
        // Garante que o banco está criado
        context.Database.EnsureCreated();
        
        return context;
    }

    /// <summary>
    /// Cria um contexto com dados de seed para testes que precisam de dados pré-existentes.
    /// </summary>
    public AppDbContext CriarContextoComDados()
    {
        var context = CriarContexto();
        SeedDados(context);
        return context;
    }

    /// <summary>
    /// Popula o banco com dados de teste padrão.
    /// </summary>
    private void SeedDados(AppDbContext context)
    {
        var titulo = new Titulo
        {
            Id = Guid.NewGuid(),
            NumeroTitulo = "T-SEED-001",
            NomeDevedor = "João da Silva Seed",
            CpfDevedor = "11144477735",
            PercentualJuros = 1m,
            PercentualMulta = 10m,
            CriadoEm = DateTime.UtcNow
        };

        titulo.AdicionarParcela(new Parcela
        {
            Id = Guid.NewGuid(),
            Numero = 1,
            DataVencimento = new DateOnly(2026, 4, 15),
            Valor = 1000m
        });

        titulo.AdicionarParcela(new Parcela
        {
            Id = Guid.NewGuid(),
            Numero = 2,
            DataVencimento = new DateOnly(2026, 5, 15),
            Valor = 1000m
        });

        context.Titulos.Add(titulo);
        context.SaveChanges();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Limpeza de recursos gerenciados
            }
            _disposed = true;
        }
    }
}
