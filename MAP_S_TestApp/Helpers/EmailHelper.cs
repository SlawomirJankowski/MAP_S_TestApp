using System.Net;
using System.Net.Mail;
using System.Text;

namespace MAP_S_TestApp.Helpers;

public class EmailHelper
{
    private readonly string _hostSmtp = "smtp.gmail.com";
    private readonly bool _enableSsl = true;
    private readonly int _port = 587;
    private readonly string _senderEmail = "EMAIL";
    private readonly string _senderEmailPassword = "PASSWORD";
    private readonly string _senderName = "MAP_S_TestApp";

    private static string GenerateActivationMessage(string firstName, string lastName, string link)
    {
        var activationMessage = new StringBuilder();
        activationMessage.Append($"Witaj {firstName} {lastName},");
        activationMessage.Append("<br /><br />Kliknij proszę w poniższy link aby aktywować swoje konto w aplikacji MAP-S_TestApp:");
        activationMessage.Append("<br /><a href = '" + link + "'>Link aktywacyjny</a>");
        activationMessage.Append("<br /><br />Dziękujemy");

        return activationMessage.ToString();
    }

    public async Task SendActivationLink(string link, string firstName, string lastName, string userEmail)
    {

        using (var mailMessage = new MailMessage(_senderEmail, userEmail))
        {
            mailMessage.From = new MailAddress(_senderEmail, _senderName);
            mailMessage.To.Add(new MailAddress(userEmail, $"{firstName} {lastName}"));

            mailMessage.Subject = "Aktywacja konta - MAP_S_TestApp";
            mailMessage.SubjectEncoding = Encoding.UTF8;

            mailMessage.Body = GenerateActivationMessage(firstName, lastName, link);
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;

            var smtp = new SmtpClient
            {
                Host = _hostSmtp,
                EnableSsl = _enableSsl,
                Port = _port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_senderEmail, _senderEmailPassword)
            };

            await smtp.SendMailAsync(mailMessage);
        }
    }
}


        
