using System;
using System.Net;
using System.Net.Mail;

namespace HeroAdventureApp.Classes
{
    public class NotificationService
    {
        public void SendEmail(string toEmail, string message)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com") // Byt till din SMTP-server
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your-email@gmail.com", "your-app-password"), // Använd app-password för Gmail
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@gmail.com"),
                    Subject = "Hero Quest Reminder",
                    Body = message,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
