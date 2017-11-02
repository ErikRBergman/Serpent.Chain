namespace Serpent.MessageBus.MessageHandlerChain.Decorators.BranchOut
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;

    /// <summary>
    /// The branch out decorator - used bu the branch out extensions
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    internal class BranchOutDecorator<TMessageType> : IMessageHandler<TMessageType>
    {
        private readonly IEnumerable<Action<IMessageHandlerChainBuilder<TMessageType>>> branches;

        private readonly object listLock = new object();

        /// <summary>
        ///     The message handlers
        /// </summary>
        private List<Func<TMessageType, CancellationToken, Task>> handlers = new List<Func<TMessageType, CancellationToken, Task>>();

        private IMessageHandlerChain parentMessageHandlerChain;

        public BranchOutDecorator(IEnumerable<Action<IMessageHandlerChainBuilder<TMessageType>>> branches)
        {
            this.branches = branches;
        }

        public Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            return Task.WhenAll(this.handlers.Select(h => h(message, token)));
        }

        public void MessageHandlerChainBuilt(IMessageHandlerChain messageHandlerChain)
        {
            this.parentMessageHandlerChain = messageHandlerChain;

            foreach (var branch in this.branches)
            {
                var builder = new MessageHandlerChainBuilder<TMessageType>();
                branch(builder);

                var subscriptionNotification = new MessageHandlerChainBuildNotification();
                var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);
                var chainFunc = builder.BuildFunc(services);

                this.handlers.Add(chainFunc);

                var chain = new MessageHandlerChain<TMessageType>(chainFunc, () => this.Unsubscribe(chainFunc));
                subscriptionNotification.Notify(chain);
            }
        }

        private void Unsubscribe(Func<TMessageType, CancellationToken, Task> handlerFunc)
        {
            var newList = new List<Func<TMessageType, CancellationToken, Task>>(this.handlers.Count - 1);
            lock (this.listLock)
            {
                newList.AddRange(this.handlers.Where(h => h != handlerFunc));

                this.handlers = newList;

                if (this.handlers.Count == 0)
                {
                    this.parentMessageHandlerChain?.Dispose();
                }
            }
        }
    }
}