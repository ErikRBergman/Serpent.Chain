namespace Serpent.MessageHandlerChain.Decorators.SkipWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SkipWhileDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private int isSkipping = 1;

        public SkipWhileDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, bool> predicate)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.isSkipping == 1)
            {
                if (this.predicate(message))
                {
                    return Task.CompletedTask;
                }

                this.isSkipping = 0;
            }

            return this.handlerFunc(message, token);
        }
    }
}