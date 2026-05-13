namespace Paschoalotto.Application.DTOs.Auth;

/// <summary>
/// Representação pública do usuário autenticado (sem informações sensíveis).
/// </summary>
/// <param name="Id">Identificador único do usuário (GUID).</param>
/// <param name="Name">Nome completo do usuário.</param>
/// <param name="Email">E-mail cadastrado.</param>
/// <param name="CreatedAt">Data e hora (UTC) em que o usuário foi criado.</param>
public record UserDto(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt
);
