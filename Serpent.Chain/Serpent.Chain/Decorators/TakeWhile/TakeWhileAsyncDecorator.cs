namespace Serpent.Chain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TakeWhileAsyncDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, Task<bool>> predicate;

        private int isActive = 1;

        private IChain chain;

        public TakeWhileAsyncDecorator(TakeWhileAsyncDecoratorConfiguration<TMessageType> configuration)
        {
            this.handlerFunc = configuration.HandlerFunc;
            this.predicate = configuration.Predicate;
            configuration.Services.BuilderNotifier.AddNotification(this.SetChain);
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.isActive == 1)
            {
                if (await this.predicate(message).ConfigureAwait(false))
                {
                    await this.handlerFunc(message, token).ConfigureAwait(false);
                    return;
                }

                this.chain?.Dispose();

                this.isActive = 0;
            }
        }

        // ReSharper disable once ParameterHidesMember
        private void SetChain(IChain chain)
        {
            this.chain = chain;
        }
    }
}