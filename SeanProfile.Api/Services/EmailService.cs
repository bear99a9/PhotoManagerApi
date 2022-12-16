using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace SeanProfile.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppSettingsModel _appSettings;

        public EmailService(IOptions<AppSettingsModel> options)
        {
            _appSettings = options.Value;
        }

        public async Task SendNewUploadEmail(IEnumerable<UserModel> users)
        {
            try
            {

                var sendGridClient = new SendGridClient(_appSettings.SendGridApiKey)
                {
                    UrlPath = _appSettings.SendGridAPIBaseUrl + _appSettings.SendGridSendMailPath
                };

                if (!String.IsNullOrWhiteSpace(_appSettings.EmailOverride))
                {
                    var sendGridMessage = new SendGridMessage();

                    sendGridMessage.SetFrom(_appSettings.EmailFrom);

                    sendGridMessage.AddTo(_appSettings.EmailOverride);
                    sendGridMessage.SetTemplateId(_appSettings.NewPhotoTemplateID);

                    sendGridMessage.SetTemplateData(new
                    {
                        Subject = "New photos uploaded",
                        Name = $"Hi Sean"
                    });

                    var response = await sendGridClient.SendEmailAsync(sendGridMessage);


                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        return;
                    }

                    throw new Exception($"Status Code returned not valid: {response.StatusCode};  response content: {response.Body}");

                }
                else
                {
                    foreach (var user in users)
                    {
                        var sendGridMessage = new SendGridMessage();

                        sendGridMessage.SetFrom(_appSettings.EmailFrom);

                        sendGridMessage.AddTo(user.Email);

                        sendGridMessage.SetTemplateId(_appSettings.NewPhotoTemplateID);

                        sendGridMessage.SetTemplateData(new
                        {
                            Subject = "New photos uploaded",
                            Name = $"Hi {user.FirstName}"

                        });

                        var response = await sendGridClient.SendEmailAsync(sendGridMessage);


                        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                        {
                            return;
                        }

                        throw new Exception($"Status Code returned not valid: {response.StatusCode};  response content: {response.Body}");

                    }

                }

            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
