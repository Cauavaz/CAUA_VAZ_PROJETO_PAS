namespace Paschoalotto.Application.DTOs.Common;

/// <summary>
/// Representa uma resposta de erro padronizada retornada pela API.
/// </summary>
/// <param name="StatusCode">Código HTTP do erro (ex: 400, 401, 404, 500).</param>
/// <param name="Message">Mensagem descritiva do erro, voltada ao consumidor da API.</param>
/// <param name="Timestamp">Data e hora (UTC) em que o erro foi gerado.</param>
public record ErrorResponse(
    int StatusCode,
    string Message,
    DateTime Timestamp
)
{
    /// <summary>
    /// Cria um <see cref="ErrorResponse"/> preenchendo automaticamente o timestamp em UTC.
    /// </summary>
    public static ErrorResponse Create(int statusCode, string message) =>
        new(statusCode, message, DateTime.UtcNow);
}
