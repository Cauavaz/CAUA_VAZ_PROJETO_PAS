using Paschoalotto.Domain.Entities;

namespace Paschoalotto.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    Guid? ValidateToken(string token);
}
