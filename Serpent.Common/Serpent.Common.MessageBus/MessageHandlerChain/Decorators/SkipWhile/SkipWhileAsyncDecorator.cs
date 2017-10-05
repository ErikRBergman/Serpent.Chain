namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SkipWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SkipWhileAsyncDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, Task<bool>> predicate;

        private int isSkipping = 1;

        public SkipWhileAsyncDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, Task<bool>> predicate)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.isSkipping == 1)
            {
                if (await this.predicate(message).ConfigureAwait(false))
                {
                    return;
                }

                this.isSkipping = 0;
            }

            await this.handlerFunc(message, token).ConfigureAwait(false);
        }
    }
}