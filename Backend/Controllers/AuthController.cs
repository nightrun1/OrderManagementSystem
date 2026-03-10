using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs.Auth;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.EmailExistsAsync(email))
        {
            return BadRequest(new { message = "Email is already in use." });
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = UserRole.Customer
        };

        await userRepository.AddAsync(user);

        var token = tokenService.GenerateToken(user);
        return Ok(new AuthResponse(token, user.Email, user.FullName, user.Role));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = tokenService.GenerateToken(user);
        return Ok(new AuthResponse(token, user.Email, user.FullName, user.Role));
    }
}
