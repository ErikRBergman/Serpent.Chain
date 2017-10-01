namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class FireAndForgetDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        public FireAndForgetDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public FireAndForgetDecorator(MessageHandlerChainDecorator<TMessageType> innerSubscription)
        {
            this.handlerFunc = innerSubscription.HandleMessageAsync;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            Task.Run(() => this.handlerFunc(message, token), token);
            return Task.CompletedTask;
        }
    }
}