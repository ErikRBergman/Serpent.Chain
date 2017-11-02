namespace Serpent.MessageBus.MessageHandlerChain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TakeWhileDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private int isActive = 1;

        public TakeWhileDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, bool> predicate)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken)
        {
            if (this.isActive == 1)
            {
                if (this.predicate(message))
                {
                    return this.handlerFunc(message, cancellationToken);
                }

                this.isActive = 0;
            }

            return Task.CompletedTask;
        }
    }
}