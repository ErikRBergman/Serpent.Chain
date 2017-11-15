namespace Serpent.MessageBus.MessageHandlerChain.Decorators.TakeWhile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal struct TakeWhileDecoratorConfiguration<TMessageType>
    {
        public Func<TMessageType, CancellationToken, Task> HandlerFunc { get; set; }

        public Func<TMessageType, bool> Predicate { get; set; }

        public MessageHandlerChainBuilderSetupServices Services { get; set; }
    }
}