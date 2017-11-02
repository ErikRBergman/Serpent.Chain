namespace Serpent.MessageBus.MessageHandlerChain.Decorators.Delay
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class DelayDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly TimeSpan timeToWait;

        public DelayDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, TimeSpan timeToWait)
        {
            this.handlerFunc = handlerFunc;
            this.timeToWait = timeToWait;
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            await Task.Delay(this.timeToWait, token).ConfigureAwait(true);
            await this.handlerFunc(message, token).ConfigureAwait(true);
        }
    }
}