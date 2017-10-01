#pragma warning disable 4014
namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class DelayDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
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