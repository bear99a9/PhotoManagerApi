namespace SeanProfile.Api.Model
{
    public class AppSettingsModel
    {
        public const string SectionName = "AppSettings";
        public string Secret { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
    }
}
