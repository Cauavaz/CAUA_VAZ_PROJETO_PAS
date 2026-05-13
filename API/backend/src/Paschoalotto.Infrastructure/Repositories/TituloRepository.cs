using Microsoft.EntityFrameworkCore;
using Paschoalotto.Application.Interfaces;
using Paschoalotto.Domain.Entities;
using Paschoalotto.Infrastructure.Data;

namespace Paschoalotto.Infrastructure.Repositories;

public class TituloRepository : ITituloRepository
{
    private readonly AppDbContext _context;

    public TituloRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Titulo> AdicionarAsync(Titulo titulo, CancellationToken cancellationToken = default)
    {
        _context.Titulos.Add(titulo);
        await _context.SaveChangesAsync(cancellationToken);
        return titulo;
    }

    public async Task<IReadOnlyList<Titulo>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Titulos
            .AsNoTracking()
            .Include(t => t.Parcelas)
            .OrderByDescending(t => t.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExisteNumeroAsync(string numeroTitulo, CancellationToken cancellationToken = default)
    {
        return _context.Titulos.AnyAsync(t => t.NumeroTitulo == numeroTitulo, cancellationToken);
    }
}
