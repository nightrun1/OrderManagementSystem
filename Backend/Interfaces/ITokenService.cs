using OrderManagementSystem.Models;

namespace OrderManagementSystem.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
