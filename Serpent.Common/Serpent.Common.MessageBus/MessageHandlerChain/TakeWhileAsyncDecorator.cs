namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TakeWhileAsyncDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly Func<TMessageType, Task<bool>> predicate;

        private int isActive = 1;

        public TakeWhileAsyncDecorator(Func<TMessageType, Task> handlerFunc, Func<TMessageType, Task<bool>> predicate)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            if (this.isActive == 1)
            {
                if (await this.predicate(message))
                {
                    await this.handlerFunc(message);
                }

                this.isActive = 0;
            }
        }
    }
}