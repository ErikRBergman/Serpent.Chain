#pragma warning disable 4014
namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class DelayDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly TimeSpan timeToWait;

        public DelayDecorator(Func<TMessageType, Task> handlerFunc, TimeSpan timeToWait)
        {
            this.handlerFunc = handlerFunc;
            this.timeToWait = timeToWait;
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
                await Task.Delay(this.timeToWait).ConfigureAwait(true);
                await this.handlerFunc(message).ConfigureAwait(true);
        }
    }
}