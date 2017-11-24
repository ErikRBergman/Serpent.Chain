namespace Serpent.Chain.Decorators.Take
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;

    internal class TakeDecorator<TMessageType> : ChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private int count;

        private IChain chain;

        public TakeDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int numberOfMessages, IChainBuilderNotifier builderNotifier)
        {
            this.handlerFunc = handlerFunc;
            this.count = numberOfMessages;
            builderNotifier.AddNotification(this.SetChain);
        }

        /// <summary>
        ///     Handles a message.
        /// </summary>
        /// <param name="message">
        ///     The message to handle.
        /// </param>
        /// <param name="cancellationToken">
        ///     A cancellation token that should be used to cancel the message handler.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that completes when the message is handled.
        /// </returns>
        public override Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken)
        {
            if (this.count > 0)
            {
                var ourCount = Interlocked.Decrement(ref this.count);

                if (ourCount >= 0)
                {
                    return this.handlerFunc(message, cancellationToken);
                }
            }

            this.chain?.Dispose();

            return Task.CompletedTask;
        }

        // ReSharper disable once ParameterHidesMember
        private void SetChain(IChain chain)
        {
            this.chain = chain;
        }
    }
}