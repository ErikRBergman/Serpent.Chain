namespace Serpent.MessageBus.MessageHandlerChain.Decorators.Append
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal struct AppendAsyncParameters<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> InnerMessageHandler;

        public bool IsRecursive;

        public TMessageType Message;

        public Func<TMessageType, Task<TMessageType>> MessageSelector;

        public Func<TMessageType, Task<bool>> Predicate;

        public CancellationToken CancellationToken;

        public AppendAsyncParameters<TMessageType> CloneForMessage(TMessageType message)
        {
            return new AppendAsyncParameters<TMessageType>
                       {
                           Message = message,
                           InnerMessageHandler = this.InnerMessageHandler,
                           IsRecursive = this.IsRecursive,
                           CancellationToken = this.CancellationToken,
                           MessageSelector = this.MessageSelector,
                           Predicate = this.Predicate
                       };
        }
    }
}