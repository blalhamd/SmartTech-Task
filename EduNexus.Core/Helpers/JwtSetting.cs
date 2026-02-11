namespace EduNexus.Core.Helpers
{

    public class JwtSetting
    {
        public const string SectionName = "JwtSettings";
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int LifeTime { get; set; }
        public string Key { get; set; } = null!;
    }

}
