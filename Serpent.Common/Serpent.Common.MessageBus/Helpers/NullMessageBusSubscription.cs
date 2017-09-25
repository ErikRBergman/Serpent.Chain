namespace Serpent.Common.MessageBus.Helpers
{
    internal struct NullMessageBusSubscription : IMessageBusSubscription
    {
        public void Dispose()
        {
        }

        public void Unsubscribe()
        {
        }
    }
}