using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paschoalotto.Application.DTOs.Common;
using Paschoalotto.Application.DTOs.Titulos;
using Paschoalotto.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Paschoalotto.API.Controllers;

/// <summary>
/// Endpoints de gestao de titulos financeiros
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[AllowAnonymous]
public class TitulosController : ControllerBase
{
    private readonly ITituloService _tituloService;

    public TitulosController(ITituloService tituloService)
    {
        _tituloService = tituloService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Criar titulo", Description = "Cadastra titulo com parcelas, validacao de CPF e calculo automatico de juros e multa")]
    [ProducesResponseType(typeof(TituloListagemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TituloListagemResponse>> Criar([FromBody] CriarTituloRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _tituloService.CriarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Listar), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse.Create(StatusCodes.Status400BadRequest, ex.Message));
        }
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Listar titulos", Description = "Retorna lista de titulos com valores atualizados (juros e multa calculados)")]
    [ProducesResponseType(typeof(IEnumerable<TituloListagemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TituloListagemResponse>>> Listar(CancellationToken cancellationToken)
    {
        var titulos = await _tituloService.ListarAsync(cancellationToken);
        return Ok(titulos);
    }
}
