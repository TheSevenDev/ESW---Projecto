using System.Net;
using System.Net.Mail;

namespace CIMOB_IPS
{
    public class Email
    {
        public static void SendEmail(string toEmail, string subject, string body)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("cimob.ips.1718g1@gmail.com", "f00n!l06");

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("cimob.ips.1718g1@gmail.com")
            };
            mailMessage.To.Add(toEmail);
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            client.Send(mailMessage);
        }
    }
}
