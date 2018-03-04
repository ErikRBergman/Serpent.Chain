// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using Serpent.Chain.Decorators.WeakReference;
    using Serpent.Chain.Interfaces;

    /// <summary>
    ///     The .WeakReference() decorator extensions
    /// </summary>
    public static class WeakReferenceExtensions
    {
        /// <summary>
        /// Make the rest of the message handler chain a Weak Reference. 
        /// This means that this message handler chain will not prevent the handler from being garbage collected. If the handler is garbage collected, the subscription will be disposed the next time a message passes through.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="handler">The object to monitor for garbage collection</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> WeakReference<TMessageType>(this IChainBuilder<TMessageType> chainBuilder, IMessageHandler<TMessageType> handler)
        {
            return chainBuilder.WeakReference(handler, WeakReferenceGarbageCollector.Default);
        }

        /// <summary>
        /// Make the rest of the message handler chain a Weak Reference. 
        /// This means that this message handler chain will not prevent the handler from being garbage collected. If the handler is garbage collected, the subscription will be disposed the next time a message passes through.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="handler">The object to monitor for garbage collection</param>
        /// <param name="weakReferenceGarbageCollector">A weak reference garbage collector</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> WeakReference<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            IMessageHandler<TMessageType> handler,
            IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
        {
            chainBuilder.Handle(services => new WeakReferenceDecorator<TMessageType>(handler, services.BuilderNotifier, weakReferenceGarbageCollector).HandleMessageAsync);
            return chainBuilder;
        }
    }
}