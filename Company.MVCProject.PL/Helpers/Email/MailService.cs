using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Company.MVCProject.PL.Helpers.Email
{
    public class MailService : IMailService
    {
        private readonly IOptions<MailSettings> _options;

        public MailService(IOptions<MailSettings> options)
        {
            _options = options;
        }
        public bool SendEmail(Email email)
        {
            try
            {
                // Build Message
                var mail = new MimeMessage();
                mail.Subject = email.Subject;
                mail.From.Add(new MailboxAddress(_options.Value.DisplayName,_options.Value.Email));
                mail.To.Add(MailboxAddress.Parse(email.To));

                var builder = new BodyBuilder();
                builder.TextBody = email.Body;
                mail.Body = builder.ToMessageBody();

                // Establish Connection

                var smtp = new SmtpClient();

                smtp.Connect(_options.Value.Host, _options.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_options.Value.Email, _options.Value.Password);

                // Send Message
                smtp.Send(mail);
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }
    }
}
