using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CIMOB_IPS
{
    public class Email
    {
        public static void SendEmail(string toEmail)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("cimob.ips.1718g1@gmail.com", "f00n!l06");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("cimob.ips.1718g1@gmail.com");
            mailMessage.To.Add(toEmail);
            mailMessage.Body = "Não responder, e-mail de teste :)";
            mailMessage.Subject = "E-mail de teste";
            client.Send(mailMessage);

            Guid g;
            g = Guid.NewGuid();
            Console.WriteLine(g);
        }
    }
}
