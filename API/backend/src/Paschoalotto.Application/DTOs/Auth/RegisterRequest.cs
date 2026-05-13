using System.ComponentModel.DataAnnotations;

namespace Paschoalotto.Application.DTOs.Auth;

/// <summary>
/// Dados necessários para registrar um novo usuário no sistema.
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// Nome completo do usuário (entre 3 e 100 caracteres).
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// E-mail válido e único para identificação no sistema.
    /// </summary>
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha de acesso (mínimo 6 caracteres).
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; init; } = string.Empty;
}
