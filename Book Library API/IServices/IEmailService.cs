public interface IEmailService
{
    void SendOverdueNotification(string toEmail, string bookTitle);
}
