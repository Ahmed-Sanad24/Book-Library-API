using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly string smtpServer;
    private readonly int smtpPort;
    private readonly string smtpUser;
    private readonly string smtpPass;

    public EmailService(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
        this.smtpServer = smtpServer;
        this.smtpPort = smtpPort;
        this.smtpUser = smtpUser;
        this.smtpPass = smtpPass;
    }

    public void SendOverdueNotification(string toEmail, string bookTitle)
    {
        var fromAddress = new MailAddress(smtpUser, "Library System");
        var toAddress = new MailAddress(toEmail);
        const string subject = "Overdue Book Notification";
        string body = $"Dear User,\n\nThe book '{bookTitle}' is overdue. Please return it as soon as possible.\n\nThank you.";

        var smtp = new SmtpClient
        {
            Host = smtpServer,
            Port = smtpPort,
            EnableSsl = true,
            Timeout = 30000,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = true,
            Credentials = new NetworkCredential(fromAddress.Address, smtpPass)
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        })
        {
            smtp.Send(message);
        }
    }
}
