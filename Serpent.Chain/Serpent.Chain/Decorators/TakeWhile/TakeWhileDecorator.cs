namespace Serpent.Chain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TakeWhileDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private int isActive = 1;

        private IChain chain;

        public TakeWhileDecorator(TakeWhileDecoratorConfiguration<TMessageType> configuration)
        {
            this.handlerFunc = configuration.HandlerFunc;
            this.predicate = configuration.Predicate;
            configuration.Services.BuilderNotifier.AddNotification(this.SetChain);
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken)
        {
            if (this.isActive == 1)
            {
                if (this.predicate(message))
                {
                    return this.handlerFunc(message, cancellationToken);
                }

                this.chain?.Dispose();
                this.isActive = 0;
            }

            return Task.CompletedTask;
        }

        // ReSharper disable once ParameterHidesMember
        private void SetChain(IChain chain)
        {
            this.chain = chain;
        }
    }
}