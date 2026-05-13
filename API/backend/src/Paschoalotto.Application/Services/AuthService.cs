using Paschoalotto.Application.DTOs.Auth;
using Paschoalotto.Application.Interfaces;
using Paschoalotto.Domain.Entities;

namespace Paschoalotto.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Email já está em uso.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(
            Token: token,
            Name: user.Name,
            Email: user.Email,
            ExpiresAt: DateTime.UtcNow.AddHours(24)
        );
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Email ou senha inválidos.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Usuário inativo.");
        }

        var token = _jwtService.GenerateToken(user);

        return new AuthResponse(
            Token: token,
            Name: user.Name,
            Email: user.Email,
            ExpiresAt: DateTime.UtcNow.AddHours(24)
        );
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null) return null;

        return new UserDto(
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            CreatedAt: user.CreatedAt
        );
    }
}
