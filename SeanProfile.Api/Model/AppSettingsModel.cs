namespace SeanProfile.Api.Model
{
    public class AppSettingsModel
    {
        public const string SectionName = "AppSettings";
        public string Token { get; set; } = string.Empty;
        public string BlobConnString { get; set; } = string.Empty;
        public string BlobContainer { get; set; } = string.Empty;
        public string SqlConnection { get; set; } = string.Empty;
        public string ImagekitURL { get; set; } = string.Empty;
        public string ThumbNailSize { get; set; } = string.Empty;
        public string SRCSize { get; set; } = string.Empty;
        public string SendGridApiKey { get; set; } = string.Empty;
        public string NewPhotoTemplateID { get; set; } = string.Empty;
        public string PasswordResetTemplateID { get; set; } = string.Empty;
        public string SendGridAPIBaseUrl { get; set; } = string.Empty;
        public string SendGridSendMailPath { get; set; } = string.Empty;
        public string EmailOverride { get; set; } = string.Empty;
        public string EmailFrom { get; set; } = string.Empty;


    }
}
