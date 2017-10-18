namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Branch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class BranchHandler<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IMessageBusSubscriptions<TMessageType>
    {
        private readonly object listLock = new object();

        /// <summary>
        ///     The message handlers
        /// </summary>
        private List<Func<TMessageType, CancellationToken, Task>> handlers;

        private IMessageBusSubscription subscription;

        public BranchHandler(IEnumerable<Action<IMessageHandlerChainBuilder<TMessageType>>> branches)
        {
            this.handlers = new List<Func<TMessageType, CancellationToken, Task>>();

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

        public void SetSubscription(IMessageBusSubscription subscription)
        {
            if (subscription != null)
            {
                lock (this.listLock)
                {
                    if (this.handlers.Count == 0)
                    {
                        subscription.Dispose();
                        subscription = null;
                    }
                }
            }

            this.subscription = subscription;
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