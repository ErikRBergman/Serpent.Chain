namespace Serpent.Chain.Samples.NotificationSample
{
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class NotificationService : INotificationService
    {
        private readonly SmtpClient smtpClient = new SmtpClient();

        public Task SendNotificationAsync(NotificationData notification)
        {
            return this.smtpClient.SendMailAsync(new MailMessage("noreply@serpent.chain", notification.RecipientEmailAddress, notification.Subject, notification.Body));
        }
    }
}