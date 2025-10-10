using System.Net;
using System.Net.Mail;

namespace Company.MVCProject.PL.Helpers
{
    public static class EmailSettings
    {
        public static bool SendEmail(Email email)
        {
            try
            {
                // Mail Server : Gmail
                // SMTP

                var client = new SmtpClient("smtp.gmail.com", 587);

                client.EnableSsl = true;
                //fpqchfokmqzxerha
                client.Credentials = new NetworkCredential("albqrym58@gmail.com", "fpqchfokmqzxerha");
                client.Send("albqrym58@gmail.com",email.To,email.Subject,email.Body);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }
    }
}
