namespace Serpent.MessageHandlerChain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TakeWhileDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private int isActive = 1;

        private IMessageHandlerChain messageHandlerChain;

        public TakeWhileDecorator(TakeWhileDecoratorConfiguration<TMessageType> configuration)
        {
            this.handlerFunc = configuration.HandlerFunc;
            this.predicate = configuration.Predicate;
            configuration.Services.BuildNotification.AddNotification(this.SetChain);
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken)
        {
            if (this.isActive == 1)
            {
                if (this.predicate(message))
                {
                    return this.handlerFunc(message, cancellationToken);
                }

                this.messageHandlerChain?.Dispose();
                this.isActive = 0;
            }

            return Task.CompletedTask;
        }

        private void SetChain(IMessageHandlerChain chain)
        {
            this.messageHandlerChain = chain;
        }
    }
}