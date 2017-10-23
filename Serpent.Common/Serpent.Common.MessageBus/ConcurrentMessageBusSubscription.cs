namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.Helpers;

    internal class ConcurrentMessageBusSubscription : IMessageBusSubscription
    {
        private readonly object lockObject = new object();
        private Action unsubscribeAction;

        public ConcurrentMessageBusSubscription(Action unsubscribeAction)
        {
            this.unsubscribeAction = unsubscribeAction;
        }

        public void Dispose()
        {
            this.Unsubscribe();
        }

        private void Unsubscribe()
        {
            lock (this.lockObject)
            {
                var action = this.unsubscribeAction;
                this.unsubscribeAction = ActionHelpers.NoAction;
                action.Invoke();
            }
        }
    }
}