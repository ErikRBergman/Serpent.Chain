namespace Serpent.MessageBus.MessageHandlerChain.Decorators.SoftFireAndForget
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SoftFireAndForgetDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        public SoftFireAndForgetDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            // Does not await it. Invokes it and minds it's own business
            this.handlerFunc(message, token);
            return Task.CompletedTask;
        }
    }
}