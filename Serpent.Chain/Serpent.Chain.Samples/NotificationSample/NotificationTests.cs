namespace Serpent.Chain.Samples.NotificationSample
{
    using System.Threading.Tasks;

    using Xunit;

    public class NotificationTests
    {
        //[Fact]
        public async Task TestSendNotificationsAsync()
        {
            INotificationService notificationService = new NotificationService();

            var recipients = new[] { "a@test.com", "b@test.com", "c@test.com" };

            foreach (var recipient in recipients)
            {
                await notificationService.SendNotificationAsync(
                    new NotificationData
                        {
                            RecipientEmailAddress = recipient,
                            Subject = "Your stay at the Rizzo Hotel",
                            Body = "Welcome ..."
                        });
            }
        }
    }
}