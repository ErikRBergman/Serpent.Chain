// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public class SingleReceiverPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<IMessageHandler<TMessageType>, TMessageType, CancellationToken, Task> handlerFunc;

        private int nextSubscriptionIndex = -1;

        public SingleReceiverPublisher(Func<IMessageHandler<TMessageType>, TMessageType, CancellationToken, Task> customHandlerMethod = null)
        {
            this.handlerFunc = customHandlerMethod ?? ((subscription, message, token) => subscription.HandleMessageAsync(message, token));
        }

        // This publisher assumes the subscriptions always come in the same order
        public override Task PublishAsync(IEnumerable<IMessageHandler<TMessageType>> subscriptions, TMessageType message, CancellationToken token)
        {
            while (true)
            {
                var nextIndex = Interlocked.Increment(ref this.nextSubscriptionIndex);

                var index = 0;
                foreach (var subscription in subscriptions)
                {
                    if (index == nextIndex)
                    {
                        return this.handlerFunc(subscription, message, token);
                    }

                    index++;
                }

                if (index == 0)
                {
                    // No subscriptions
                    return Task.CompletedTask;
                }

                // If nextSubscriptionIndex has not changed, let's reset and try again
                Interlocked.CompareExchange(ref this.nextSubscriptionIndex, -1, nextIndex - 1);
            }
        }
    }
}