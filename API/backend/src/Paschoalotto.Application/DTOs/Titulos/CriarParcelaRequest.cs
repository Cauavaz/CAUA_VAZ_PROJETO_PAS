using System.ComponentModel.DataAnnotations;

namespace Paschoalotto.Application.DTOs.Titulos;

public class CriarParcelaRequest
{
    [Required(ErrorMessage = "Informe o número da parcela.")]
    [Range(1, int.MaxValue, ErrorMessage = "O número da parcela deve ser maior que zero.")]
    public int Numero { get; set; }

    [Required(ErrorMessage = "Informe a data de vencimento.")]
    public DateOnly DataVencimento { get; set; }

    [Required(ErrorMessage = "Informe o valor da parcela.")]
    public decimal Valor { get; set; }
}
