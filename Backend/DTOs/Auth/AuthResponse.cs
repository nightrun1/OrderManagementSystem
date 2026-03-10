using OrderManagementSystem.Models;

namespace OrderManagementSystem.DTOs.Auth;

public record AuthResponse(string Token, string Email, string FullName, UserRole Role);
