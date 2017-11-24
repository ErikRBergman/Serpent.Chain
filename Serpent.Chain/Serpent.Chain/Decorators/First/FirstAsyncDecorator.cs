namespace Serpent.Chain.Decorators.First
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Notification;

    internal class FirstAsyncDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate;

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private IChain chain;

        private int wasReceived;

        public FirstAsyncDecorator(
            Func<TMessageType, CancellationToken, Task> handlerFunc,
            Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate,
            ChainBuilderSetupServices subscriptionServices)
        {
            this.handlerFunc = handlerFunc;
            this.asyncPredicate = asyncPredicate;
            subscriptionServices.BuilderNotifier.AddNotification(this.ChainBuilt);
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.wasReceived == 0)
            {
                if (await this.asyncPredicate(message, token).ConfigureAwait(false))
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

        // ReSharper disable once ParameterHidesMember
        private void ChainBuilt(IChain chain)
        {
            this.chain = chain;
        }
    }
}