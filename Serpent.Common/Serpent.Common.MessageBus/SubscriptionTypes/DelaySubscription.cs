#pragma warning disable 4014
namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class DelaySubscription<TMessageType> : BusSubscription<TMessageType>
    {
        private readonly bool dontAwait;

        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly TimeSpan timeToWait;

        public DelaySubscription(Func<TMessageType, Task> handlerFunc, TimeSpan timeToWait, bool dontAwait = false)
        {
            this.handlerFunc = handlerFunc;
            this.timeToWait = timeToWait;
            this.dontAwait = dontAwait;
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            if (this.dontAwait)
            {
                // DONT AWAIT
                this.HandleMessageInternalAsync(message);
            }
            else
            {
                await Task.Delay(this.timeToWait).ConfigureAwait(true);
                await this.handlerFunc(message).ConfigureAwait(true);
            }
        }

        private async Task HandleMessageInternalAsync(TMessageType message)
        {
            await Task.Delay(this.timeToWait).ConfigureAwait(true);
            await this.handlerFunc(message).ConfigureAwait(true);
        }
    }
}