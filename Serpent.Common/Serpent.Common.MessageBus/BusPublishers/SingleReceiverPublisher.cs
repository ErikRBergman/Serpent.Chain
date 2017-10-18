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
        private readonly Func<Func<TMessageType, CancellationToken, Task>, TMessageType, CancellationToken, Task> handlerFunc;

        private int nextSubscriptionIndex = -1;

        public SingleReceiverPublisher(Func<Func<TMessageType, CancellationToken, Task>, TMessageType, CancellationToken, Task> customHandlerMethod = null)
        {
            this.handlerFunc = customHandlerMethod ?? ((subscription, message, token) => subscription(message, token));
        }

        // This publisher assumes the handlers always come in the same order
        public override Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken token)
        {
            while (true)
            {
                var nextIndex = Interlocked.Increment(ref this.nextSubscriptionIndex);

                var index = 0;
                foreach (var handler in handlers)
                {
                    if (index == nextIndex)
                    {
                        return this.handlerFunc(handler, message, token);
                    }

                    index++;
                }

                if (index == 0)
                {
                    // No handlers
                    return Task.CompletedTask;
                }

                // If nextSubscriptionIndex has not changed, let's reset and try again
                Interlocked.CompareExchange(ref this.nextSubscriptionIndex, -1, nextIndex - 1);
            }
        }
    }
}