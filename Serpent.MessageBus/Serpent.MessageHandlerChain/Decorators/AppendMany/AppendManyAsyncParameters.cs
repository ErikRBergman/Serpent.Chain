namespace Serpent.MessageHandlerChain.Decorators.AppendMany
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal struct AppendManyAsyncParameters<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> InnerMessageHandler;

        public bool IsRecursive;

        public TMessageType Message;

        public Func<TMessageType, CancellationToken, Task<IEnumerable<TMessageType>>> MessageSelector;

        public Func<TMessageType, CancellationToken, Task<bool>> Predicate;

        public CancellationToken CancellationToken;

        public AppendManyAsyncParameters<TMessageType> CloneForMessage(TMessageType message)
        {
            return new AppendManyAsyncParameters<TMessageType>
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