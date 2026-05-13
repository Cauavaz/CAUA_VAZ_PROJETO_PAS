using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paschoalotto.Application.DTOs.Auth;
using Paschoalotto.Application.DTOs.Common;
using Paschoalotto.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Paschoalotto.API.Controllers;

/// <summary>
/// Endpoints de autenticacao e gerenciamento de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Registrar novo usuario", Description = "Cria uma nova conta de usuario com validacao de email unico e senha minima de 6 caracteres")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ErrorResponse.Create(StatusCodes.Status400BadRequest, ex.Message));
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Autenticar usuario", Description = "Autentica usuario e retorna token JWT valido por 24 horas")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ErrorResponse.Create(StatusCodes.Status401Unauthorized, ex.Message));
        }
    }

    [Authorize]
    [HttpGet("me")]
    [SwaggerOperation(Summary = "Obter dados do usuario autenticado", Description = "Retorna dados do usuario baseado no token JWT")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ErrorResponse.Create(
                StatusCodes.Status401Unauthorized,
                "Token inválido ou sem identificação de usuário."));
        }

        var user = await _authService.GetCurrentUserAsync(userId);

        if (user is null)
        {
            return NotFound(ErrorResponse.Create(
                StatusCodes.Status404NotFound,
                "Usuário não encontrado."));
        }

        return Ok(user);
    }
}
