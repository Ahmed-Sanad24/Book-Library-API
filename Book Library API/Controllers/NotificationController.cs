using Bill_system_API.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;


namespace Bill_system_API.Controllers
{
    /// <summary>
    /// Handles notifications related to overdue books.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public NotificationController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Notifies users about their overdue books.
        /// </summary>
        /// <returns>A status message indicating the result of the notification process.</returns>
        [HttpPost("notify-overdue")]
        public IActionResult NotifyOverdueBooks()
        {
            var overdueBooks = unitOfWork.BorrowedBooks.GetOverdueBooks();
            foreach (var book in overdueBooks)
            {
                if (book.ApplicationUser != null && book != null)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Book Library Alef", "identity.server.2022@gmail.com"));
                    message.To.Add(new MailboxAddress("A. Sanad", book.ApplicationUser.Email));
                    message.Subject = "Overdue Book Notification";  // Customize subject as needed
                    message.Body = new TextPart("plain")
                    {
                        Text = $"Hello, this is a notification about your overdue book: {book.Book.Title} \n " +
                        $"that you borrowed at : {book.BorrowedDate}"
                    };  // Customize the body of the email
                    using (var client = new SmtpClient())
                    {
                        // Connect to Gmail's SMTP server on port 465 for SSL
                        client.Connect("smtp.gmail.com", 465, true);  // 'true' for SSL connection
                        client.Authenticate("identity.server.2022@gmail.com", "password-for-account");  // Authenticate using your email and password
                        client.Send(message);  // Send the email
                        client.Disconnect(true);  // Disconnect after sending the email
                    }
                }
            }
            return Ok("Notifications sent to users with overdue books.");
        }
    }

}
