using System.Net;
using System.Net.Mail;

namespace CIMOB_IPS
{
    public class Email
    {
        public static void SendEmail(string strToEmail, string strSubject, string strBody)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("cimob.ips.1718g1@gmail.com", "f00n!l06");

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("cimob.ips.1718g1@gmail.com")
            };

            mailMessage.To.Add(strToEmail);
            mailMessage.Body = strBody;
            mailMessage.Subject = strSubject;
            mailMessage.IsBodyHtml = true;
            smtpClient.Send(mailMessage);
        }
    }
}
