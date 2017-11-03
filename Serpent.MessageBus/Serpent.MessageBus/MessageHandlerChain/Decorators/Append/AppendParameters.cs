namespace Serpent.MessageBus.MessageHandlerChain.Decorators.Append
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal struct AppendParameters<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> InnerMessageHandler;

        public bool IsRecursive;

        public TMessageType Message;

        public Func<TMessageType, TMessageType> MessageSelector;

        public Func<TMessageType, bool> Predicate;

        public CancellationToken CancellationToken;

        public AppendParameters<TMessageType> CloneForMessage(TMessageType message)
        {
            return new AppendParameters<TMessageType>
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