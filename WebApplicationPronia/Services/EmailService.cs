using MimeKit;

using WebApplicationPronia.Abstractions;
using WebApplicationPronia.ViewModels.EmailViewModels;
using MailKit.Net.Smtp;

namespace WebApplicationPronia.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettingsVM _settings;
        private readonly IConfiguration _configuration;  //builder.Configuration

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _settings = _configuration.GetSection("SmtpSettings").Get<SmtpSettingsVM>() ?? new();
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body
                };


                using var client = new SmtpClient();

                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_settings.Server, _settings.Port, true);


                await client.AuthenticateAsync(_settings.Username, _settings.Password);

                await client.SendAsync(message);

            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}
