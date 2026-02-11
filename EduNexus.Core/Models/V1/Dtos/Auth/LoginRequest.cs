namespace EduNexus.Core.Models.V1.Dtos.Auth
{
    public class LoginRequest 
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
