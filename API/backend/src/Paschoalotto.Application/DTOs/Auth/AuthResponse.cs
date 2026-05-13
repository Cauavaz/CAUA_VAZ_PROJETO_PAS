namespace Paschoalotto.Application.DTOs.Auth;

/// <summary>
/// Resposta retornada após uma autenticação ou registro bem-sucedido.
/// </summary>
/// <param name="Token">Token JWT a ser usado no cabeçalho <c>Authorization: Bearer {token}</c>.</param>
/// <param name="Name">Nome do usuário autenticado.</param>
/// <param name="Email">E-mail do usuário autenticado.</param>
/// <param name="ExpiresAt">Data e hora (UTC) de expiração do token.</param>
public record AuthResponse(
    string Token,
    string Name,
    string Email,
    DateTime ExpiresAt
);
