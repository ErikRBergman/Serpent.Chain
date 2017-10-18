// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.Helpers;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WeakReference;

    /// <summary>
    ///     The delay decorator extensions
    /// </summary>
    public static class WeakReferenceExtensions
    {
        /// <summary>
        /// Make the rest of the message handler chain a Weak Reference. 
        /// This means that this message handler chain will not prevent the handler from being garbage collected. If the handler is garbage collected, the subscription will be disposed the next time a message passes through.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> WeakReference<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.WeakReference(WeakReferenceGarbageCollector.Default);
        }

        /// <summary>
        /// Make the rest of the message handler chain a Weak Reference. 
        /// This means that this message handler chain will not prevent the handler from being garbage collected. If the handler is garbage collected, the subscription will be disposed the next time a message passes through.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="weakReferenceGarbageCollector">A weak reference garbage collector</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> WeakReference<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
        {
            messageHandlerChainBuilder.Add((currentHandler, services) => new WeakReferenceDecorator<TMessageType>(currentHandler, services.SubscriptionNotification, weakReferenceGarbageCollector).HandleMessageAsync);
            return new MessageHandlerChainBuilder<TMessageType>(new MessageHandlerChainBridge<TMessageType>(messageHandlerChainBuilder));
        }
    }
}