namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class FireAndForgetSoftDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        public FireAndForgetSoftDecorator(Func<TMessageType, Task> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            // Does not await it. Invokes it and minds it's own business
            this.handlerFunc(message);
            return Task.CompletedTask;
        }
    }
}