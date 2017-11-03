namespace Serpent.MessageBus.MessageHandlerChain.Decorators.AppendMany
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal struct AppendManyParameters<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> InnerMessageHandler;

        public bool IsRecursive;

        public TMessageType Message;

        public Func<TMessageType, IEnumerable<TMessageType>> MessageSelector;

        public Func<TMessageType, bool> Predicate;

        public CancellationToken Token;

        public AppendManyParameters<TMessageType> CloneForMessage(TMessageType message)
        {
            return new AppendManyParameters<TMessageType>
                       {
                           Message = message,
                           InnerMessageHandler = this.InnerMessageHandler,
                           IsRecursive = this.IsRecursive,
                           Token = this.Token,
                           MessageSelector = this.MessageSelector,
                           Predicate = this.Predicate
                       };
        }
    }
}