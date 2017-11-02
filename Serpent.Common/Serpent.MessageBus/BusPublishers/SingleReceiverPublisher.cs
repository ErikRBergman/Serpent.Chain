// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Dispatches a message to a single subscription, alternating between subscriptions
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class SingleReceiverPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        private readonly Func<Func<TMessageType, CancellationToken, Task>, TMessageType, CancellationToken, Task> handlerFunc;

        private int nextSubscriptionIndex = -1;

        /// <summary>
        /// Creates a new instance of the single receiver publisher
        /// </summary>
        /// <param name="customHandlerMethod">A custom handler</param>
        public SingleReceiverPublisher(Func<Func<TMessageType, CancellationToken, Task>, TMessageType, CancellationToken, Task> customHandlerMethod = null)
        {
            this.handlerFunc = customHandlerMethod ?? ((subscription, message, token) => subscription(message, token));
        }

        /// <summary>
        /// Publish a message to all handlers. This method is called when a message is published to a message bus.
        /// </summary>
        /// <param name="handlers">The handlers</param>
        /// <param name="message">The message to publish</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The task</returns>
        public override Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken cancellationToken)
        {
            while (true)
            {
                var nextIndex = Interlocked.Increment(ref this.nextSubscriptionIndex);

                var index = 0;
                foreach (var handler in handlers)
                {
                    if (index == nextIndex)
                    {
                        return this.handlerFunc(handler, message, cancellationToken);
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