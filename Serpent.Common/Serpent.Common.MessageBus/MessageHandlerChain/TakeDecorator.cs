namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TakeDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly int numberOfMessages;

        private int count;

        public TakeDecorator(Func<TMessageType, Task> handlerFunc, int numberOfMessages)
        {
            this.handlerFunc = handlerFunc;
            this.numberOfMessages = numberOfMessages;
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            if (this.count < this.numberOfMessages)
            {
                var ourCount = Interlocked.Increment(ref this.count);

                if (ourCount < this.numberOfMessages)
                {
                    return this.handlerFunc(message);
                }
            }

            return Task.CompletedTask;
        }
    }
}