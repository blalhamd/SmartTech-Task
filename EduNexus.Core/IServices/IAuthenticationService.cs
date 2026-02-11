using EduNexus.Core.Models.V1.Dtos.Auth;
using EduNexus.Core.Models.V1.ViewModels.Auth;

namespace EduNexus.Core.IServices
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<string> GeneratePasswordResetTokenAsync(ForgetPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
