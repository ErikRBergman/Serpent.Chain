namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.BranchOut
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    /// <summary>
    /// The branch out decorator - used bu the branch out extensions
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    internal class BranchOutDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IMessageBusSubscriptions<TMessageType>
    {
        private readonly object listLock = new object();

        /// <summary>
        ///     The message handlers
        /// </summary>
        private List<Func<TMessageType, CancellationToken, Task>> handlers;

        private IMessageBusSubscription subscription;

        public BranchOutDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, IMessageHandlerChainSubscriptionNotification servicesSubscriptionNotification, params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            servicesSubscriptionNotification.AddNotification(
                sub => { this.subscription = sub; });

            var numberOfHandlers = (branches?.Length ?? 0) + 1;
            this.handlers = new List<Func<TMessageType, CancellationToken, Task>>(numberOfHandlers)
                {
                    handlerFunc
                };

            if (branches == null || branches.Length == 0)
            {
                return;
            }

            foreach (var branch in branches)
            {
                var builder = new MessageHandlerChainBuilder<TMessageType>(this);
                branch(builder);
            }
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(this.handlers.Select(h => h(message, token)));
        }

        public IMessageBusSubscription Subscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            this.handlers.Add(handlerFunc);
            return new ConcurrentMessageBusSubscription(() => this.Unsubscribe(handlerFunc));
        }

        private void Unsubscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            var newList = new List<Func<TMessageType, CancellationToken, Task>>(this.handlers.Count - 1);
            lock (this.listLock)
            {
                newList.AddRange(this.handlers.Where(h => h != handlerFunc));

                this.handlers = newList;

                var sub = this.subscription;

                if (this.handlers.Count == 0)
                {
                    sub?.Dispose();
                }
            }
        }
    }
}