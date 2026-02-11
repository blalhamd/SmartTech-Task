namespace EduNexus.Core.Models.V1.ViewModels.Auth
{
    public class JwtProviderResponse
    {
        public string Token { get; set; } = null!;
        public DateTime TokenExpiration { get; set; }
    }
}
