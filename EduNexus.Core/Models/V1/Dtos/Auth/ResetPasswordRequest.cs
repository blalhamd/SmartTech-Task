namespace EduNexus.Core.Models.V1.Dtos.Auth
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
