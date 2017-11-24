namespace Serpent.Chain.Decorators.Branch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.Notification;

    internal class BranchHandler<TMessageType> : IMessageHandler<TMessageType>
    {
        private readonly IEnumerable<Action<IChainBuilder<TMessageType>>> branches;

        private readonly object listLock = new object();

        /// <summary>
        ///     The message handlers
        /// </summary>
        private List<Func<TMessageType, CancellationToken, Task>> handlers = new List<Func<TMessageType, CancellationToken, Task>>();

        private IChain parentChain;

        public BranchHandler(IEnumerable<Action<IChainBuilder<TMessageType>>> branches)
        {
            this.branches = branches;
        }

        public Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
#pragma warning disable CC0031 // Check for null before calling a delegate
            return Task.WhenAll(this.handlers.Select(handler => handler(message, token)));
#pragma warning restore CC0031 // Check for null before calling a delegate
        }

        public void ChainBuilt(IChain chain)
        {
            this.parentChain = chain;

            foreach (var branch in this.branches)
            {
                var builder = new ChainBuilder<TMessageType>();
                branch(builder);

                var notifier = new ChainBuilderNotifier();
                var services = new ChainBuilderSetupServices(notifier);
                var chainFunc = builder.BuildFunc(services);

                this.handlers.Add(chainFunc);

                var newChain = new Chain<TMessageType>(chainFunc, () => this.Unsubscribe(chainFunc));
                notifier.Notify(newChain);
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
                    this.parentChain?.Dispose();
                }
            }
        }
    }
}