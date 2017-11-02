namespace Serpent.MessageBus.Helpers
{
    internal class NullMessageBusSubscription : IMessageBusSubscription
    {
        private NullMessageBusSubscription()
        {
        }

        public static IMessageBusSubscription Default { get; } = new NullMessageBusSubscription();

        public void Dispose()
        {
        }
    }
}