using System.ComponentModel.DataAnnotations;

namespace Paschoalotto.Application.DTOs.Auth;

/// <summary>
/// Dados necessários para autenticar um usuário existente.
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// E-mail cadastrado do usuário. Exemplo: <c>candidato@paschoalotto.com.br</c>
    /// </summary>
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário (mínimo 6 caracteres).
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; init; } = string.Empty;
}
