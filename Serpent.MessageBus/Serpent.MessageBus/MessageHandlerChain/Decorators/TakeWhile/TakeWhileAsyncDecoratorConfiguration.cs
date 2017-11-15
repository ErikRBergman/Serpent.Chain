namespace Serpent.MessageBus.MessageHandlerChain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal struct TakeWhileAsyncDecoratorConfiguration<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> HandlerFunc { get; set; }

        public Func<TMessageType, Task<bool>> Predicate { get; set; }

        public MessageHandlerChainBuilderSetupServices Services { get; set; }
    }
}