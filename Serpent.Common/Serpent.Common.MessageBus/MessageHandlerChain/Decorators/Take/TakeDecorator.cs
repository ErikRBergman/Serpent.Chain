namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Take
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TakeDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private int count;

        public TakeDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int numberOfMessages)
        {
            this.handlerFunc = handlerFunc;
            this.count = numberOfMessages;
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.count > 0)
            {
                var ourCount = Interlocked.Decrement(ref this.count);

                if (ourCount >= 0)
                {
                    return this.handlerFunc(message, token);
                }
            }

            return Task.CompletedTask;
        }
    }
}