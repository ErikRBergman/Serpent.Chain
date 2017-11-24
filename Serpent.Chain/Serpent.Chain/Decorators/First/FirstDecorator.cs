namespace Serpent.Chain.Decorators.First
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Notification;

    internal class FirstDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private IChain chain;

        private int wasReceived;

        public FirstDecorator(
            Func<TMessageType, CancellationToken, Task> handlerFunc,
            Func<TMessageType, bool> predicate,
            ChainBuilderSetupServices subscriptionServices)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
            subscriptionServices.BuilderNotifier.AddNotification(this.ChainBuilt);
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.wasReceived == 0)
            {
                if (this.predicate(message))
                {
                    if (Interlocked.CompareExchange(ref this.wasReceived, 1, 0) == 0)
                    {
                        try
                        {
                            await this.handlerFunc(message, token).ConfigureAwait(false);
                        }
                        finally
                        {
                            this.chain?.Dispose();
                        }
                    }
                }
            }
        }

        private void ChainBuilt(IChain sub)
        {
            this.chain = sub;
        }
    }
}