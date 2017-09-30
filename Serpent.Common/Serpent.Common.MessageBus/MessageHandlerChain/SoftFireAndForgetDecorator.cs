namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading.Tasks;

    public class SoftFireAndForgetDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        public SoftFireAndForgetDecorator(Func<TMessageType, Task> handlerFunc)
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