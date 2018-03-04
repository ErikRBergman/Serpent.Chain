namespace Serpent.Chain.Samples.NotificationSample
{
    using System;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class NotificationServiceWithDecorators : INotificationService
    {
        private readonly SmtpClient smtpClient = new SmtpClient();

        private readonly Func<NotificationData, Task> sendMailFunc;

        public NotificationServiceWithDecorators()
        {
            this.sendMailFunc = Create.SimpleFunc<NotificationData>(
                b => b
                    .Retry(r => r.MaximumNumberOfAttempts(3).RetryDelay(TimeSpan.FromSeconds(15)))
                    .Concurrent(10)
                    .Handler(notification => this.smtpClient.SendMailAsync(new MailMessage("noreply@serpent.chain", notification.RecipientEmailAddress, notification.Subject, notification.Body))));
        }

        public Task SendNotificationAsync(NotificationData notification)
        {
            return this.sendMailFunc(notification);
        }
    }
}