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
                        Name = $"Hi Test"
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

                        sendGridMessage.SetTemplateId(_appSettings.PasswordResetTemplateID);

                        sendGridMessage.SetTemplateData(new
                        {
                            Subject = "New photos uploaded",
                            Name = $"Hi {NameBuilder(user)}"

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

        public async Task SendPasswordResetEmail(UserModel user, PasswordReset passwordReset)
        {
            try
            {
                var name = NameBuilder(user);

                var sendGridClient = new SendGridClient(_appSettings.SendGridApiKey)
                {
                    UrlPath = _appSettings.SendGridAPIBaseUrl + _appSettings.SendGridSendMailPath
                };

                var sendGridMessage = new SendGridMessage();

                sendGridMessage.SetFrom(_appSettings.EmailFrom);

                sendGridMessage.AddTo(user.Email);
                sendGridMessage.SetTemplateId(_appSettings.PasswordResetTemplateID);

                sendGridMessage.SetTemplateData(new
                {
                    Subject = "Reset Password",
                    Name = $"Hi {name}",
                    Body = @$"Here is the link to reset your password: www.rosannaandsean.co.uk/reset-password/{passwordReset.PasswordResetKey}.",
                    Body1 = "This link is only valid for 12 hours. A new link will need to be created after this time."

                });

                if (!String.IsNullOrWhiteSpace(_appSettings.EmailOverride))
                {
                    sendGridMessage.AddTo(_appSettings.EmailOverride);
                }

                var response = await sendGridClient.SendEmailAsync(sendGridMessage);


                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return;
                }

                throw new Exception($"Status Code returned not valid: {response.StatusCode};  response content: {response.Body}");

            }
            catch (Exception)
            {
                throw;
            }

        }

        private string NameBuilder(UserModel user)
        {
            if (!String.IsNullOrWhiteSpace(_appSettings.EmailOverride))
            {
                return "Test";
            }

            if (user.FirstName == "Tony" || user.FirstName == "Des")
            {
                return "Dad";
            }

            if (user.FirstName == "Alexia" || user.FirstName == "Lisa")
            {
                return "Mum";
            }

            return user.FirstName;
        }
    }
}
