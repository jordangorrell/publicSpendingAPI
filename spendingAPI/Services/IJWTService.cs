using Microsoft.AspNetCore.Identity;

namespace spendingAPI.Services
{
    public interface IJWTService
    {
        string ExtractUserIdFromToken(string authHeader);
        dynamic GenerateToken(IdentityUser user);
    }
}