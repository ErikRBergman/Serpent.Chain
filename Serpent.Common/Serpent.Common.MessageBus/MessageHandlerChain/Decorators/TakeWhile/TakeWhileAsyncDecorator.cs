namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TakeWhileAsyncDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, Task<bool>> predicate;

        private int isActive = 1;

        public TakeWhileAsyncDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, Task<bool>> predicate)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.isActive == 1)
            {
                if (await this.predicate(message).ConfigureAwait(false))
                {
                    await this.handlerFunc(message, token).ConfigureAwait(false);
                }

                this.isActive = 0;
            }
        }
    }
}