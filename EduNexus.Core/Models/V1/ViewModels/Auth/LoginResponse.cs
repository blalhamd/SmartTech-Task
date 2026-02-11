namespace EduNexus.Core.Models.V1.ViewModels.Auth
{
    public class LoginResponse
    {
        public Guid UserId { get; set; } 
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public DateTime AccessTokenExpiration { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
