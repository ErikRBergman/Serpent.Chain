namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class FireAndForgetSubscription<TMessageType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        public FireAndForgetSubscription(Func<TMessageType, Task> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public FireAndForgetSubscription(BusSubscription<TMessageType> innerSubscription)
        {
            this.handlerFunc = innerSubscription.HandleMessageAsync;
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            Task.Run(() => this.handlerFunc(message));
            return Task.CompletedTask;
        }
    }
}