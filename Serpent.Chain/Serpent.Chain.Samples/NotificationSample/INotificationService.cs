namespace Serpent.Chain.Samples.NotificationSample
{
    using System.Threading.Tasks;

    public interface INotificationService
    {
        Task SendNotificationAsync(NotificationData notificationDataData);
    }
}
