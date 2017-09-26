namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class FireAndForgetDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        public FireAndForgetDecorator(Func<TMessageType, Task> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public FireAndForgetDecorator(MessageHandlerChainDecorator<TMessageType> innerSubscription)
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