namespace SeanProfile.Api.Model
{
    public class AppSettingsModel
    {
        public const string SectionName = "AppSettings";
        public string Token { get; set; } = string.Empty;
        public string BlobConnString { get; set; } = string.Empty;
        public string BlobContainer { get; set; } = string.Empty;
        public string SqlConnection { get; set; } = string.Empty;

    }
}
