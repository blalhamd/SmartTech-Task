using EduNexus.Core.Models.V1.ViewModels.Auth;
using EduNexus.Domain.Entities.Identity;

namespace EduNexus.Core.IServices
{
    public interface IJwtProvider
    {
        JwtProviderResponse GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        
        // to validate tokens that come with requests that ask new jwt token by refresh token
        string? ValidateToken(string token);
    }
}
