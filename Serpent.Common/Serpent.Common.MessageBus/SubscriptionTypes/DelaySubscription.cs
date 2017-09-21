namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class DelaySubscription<TMessageType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly TimeSpan timeToWait;

        public DelaySubscription(Func<TMessageType, Task> handlerFunc, TimeSpan timeToWait)
        {
            this.handlerFunc = handlerFunc;
            this.timeToWait = timeToWait;
        }

        public DelaySubscription(BusSubscription<TMessageType> innerSubscription, TimeSpan timeToWait)
        {
            this.handlerFunc = innerSubscription.HandleMessageAsync;
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            await Task.Delay(this.timeToWait);
            await this.handlerFunc(message);
        }
    }
}