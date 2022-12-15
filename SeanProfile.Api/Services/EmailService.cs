using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace SeanProfile.Api.Services
{
    public class EmailService
    {
        private readonly AppSettingsModel _appSettings;

        public EmailService(IOptions<AppSettingsModel> options)
        {
            _appSettings = options.Value;
        }

        public async Task SendNewUploadEmail()
        {
            try
            {


                var sendGridClient = new SendGridClient(_appSettings.SendGridApiKey)
                {
                    UrlPath = _appSettings.SendGridAPIBaseUrl + _appSettings.SendGridSendMailPath
                };
                var sendGridMessage = new SendGridMessage();
                sendGridMessage.SetFrom(_emailFrom);


                if (!String.IsNullOrWhiteSpace(_emailOverride))
                {
                    sendGridMessage.AddTo(_emailOverride);
                }
                else
                {
                    sendGridMessage.AddTo(contact.EmailAddress);
                }

                if (documentType == "CrownCommercialService" && companyId == 2)
                {
                    foreach (var email in _emailCC)
                    {
                        sendGridMessage.AddCc(email);
                    }
                }

                sendGridMessage.SetTemplateId(_emailTemplateAgreement);

                sendGridMessage.SetTemplateData(new
                {
                    Subject = SetAgreementName(documentType),
                    Preheader = SetAgreementName(documentType),
                    EmailBody = CreateEmailBodyForAgreement(contact.FullName),
                    ButtonText = "Click to Sign",
                    ButtonUrl = urlLink,
                });

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

    }
}
