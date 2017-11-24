// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using Serpent.Chain.Decorators.WeakReference;

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
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> WeakReference<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.WeakReference(WeakReferenceGarbageCollector.Default);
        }

        /// <summary>
        /// Make the rest of the message handler chain a Weak Reference. 
        /// This means that this message handler chain will not prevent the handler from being garbage collected. If the handler is garbage collected, the subscription will be disposed the next time a message passes through.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The builder</param>
        /// <param name="weakReferenceGarbageCollector">A weak reference garbage collector</param>
        /// <returns>The builder</returns>
        public static IChainBuilder<TMessageType> WeakReference<TMessageType>(this IChainBuilder<TMessageType> chainBuilder, IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
        {
            chainBuilder.Decorate((currentHandler, services) => new WeakReferenceDecorator<TMessageType>(currentHandler, services.BuilderNotifier, weakReferenceGarbageCollector).HandleMessageAsync);
            return chainBuilder;
        }
    }
}