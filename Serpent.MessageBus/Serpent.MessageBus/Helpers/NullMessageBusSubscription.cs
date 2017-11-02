namespace Serpent.MessageBus.Helpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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