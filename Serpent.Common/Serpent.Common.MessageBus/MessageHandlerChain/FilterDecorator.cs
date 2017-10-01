namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class FilterDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, Task<bool>> beforeInvoke;

        private readonly Func<TMessageType, Task> afterInvoke;

        public FilterDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, Task<bool>> beforeInvoke = null, Func<TMessageType, Task> afterInvoke = null)
        {
            this.handlerFunc = handlerFunc;
            this.beforeInvoke = beforeInvoke;
            this.afterInvoke = afterInvoke;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            bool invoke = false;

            if (this.beforeInvoke != null)
            {
                invoke = await this.beforeInvoke(message).ConfigureAwait(false);
            }

            if (invoke)
            {
                await this.handlerFunc(message, token).ConfigureAwait(false);
            }

            if (this.afterInvoke != null)
            {
                await this.afterInvoke(message).ConfigureAwait(false);
            }
        }
    }
}