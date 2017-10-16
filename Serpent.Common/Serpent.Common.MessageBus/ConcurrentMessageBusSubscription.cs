namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.Helpers;

    internal struct ConcurrentMessageBusSubscription : IMessageBusSubscription
    {
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
            var action = this.unsubscribeAction;
            this.unsubscribeAction = ActionHelpers.NoAction;
            action.Invoke();
        }
    }
}