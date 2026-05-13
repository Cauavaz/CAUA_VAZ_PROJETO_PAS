using System.ComponentModel.DataAnnotations;

namespace Paschoalotto.Application.DTOs.Titulos;

public class CriarTituloRequest
{
    [Required(ErrorMessage = "Informe o número do título.")]
    [StringLength(50, ErrorMessage = "O número do título deve ter no máximo 50 caracteres.")]
    public string NumeroTitulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o nome do devedor.")]
    [StringLength(200, ErrorMessage = "O nome do devedor deve ter no máximo 200 caracteres.")]
    public string NomeDevedor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o CPF do devedor.")]
    public string CpfDevedor { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "O percentual de juros deve estar entre 0 e 100.")]
    public decimal PercentualJuros { get; set; }

    [Range(0, 100, ErrorMessage = "O percentual de multa deve estar entre 0 e 100.")]
    public decimal PercentualMulta { get; set; }

    [Required(ErrorMessage = "Informe pelo menos uma parcela.")]
    [MinLength(1, ErrorMessage = "Informe pelo menos uma parcela.")]
    public List<CriarParcelaRequest> Parcelas { get; set; } = new();
}
