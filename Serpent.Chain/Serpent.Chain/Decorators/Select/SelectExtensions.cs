// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.Select;

    /// <summary>
    /// The .Select() decorator extensions type
    /// </summary>
    public static class SelectExtensions
    {
        /// <summary>
        /// Projects each message to a new form
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The current message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="chainBuilder">
        /// The mhc builder
        /// </param>
        /// <param name="selector">
        /// A transform function to apply to each message
        /// </param>
        /// <returns>
        /// A builder of the new message type <see cref="IChainBuilder&lt;TNewMessageType&gt;"/>.
        /// </returns>
        public static IChainBuilder<TNewMessageType> Select<TMessageType, TNewMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, TNewMessageType> selector)
        {
            var newMessagesTypeBuilder = new ChainBuilder<TNewMessageType>();

            chainBuilder.Handle(
                services =>
                    {
                        var handler = new SelectDecorator<TMessageType, TNewMessageType>(newMessagesTypeBuilder, selector);
                        services.BuilderNotifier.AddNotification(handler.ChainBuilt);
                        return handler.HandleMessageAsync;
                    });

            return newMessagesTypeBuilder;
        }

        /// <summary>
        /// Projects each message to a new form with async selector
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The current message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="chainBuilder">
        /// The mhc builder
        /// </param>
        /// <param name="selector">
        /// A transform function to apply to each message
        /// </param>
        /// <returns>
        /// The <see cref="IChainBuilder&lt;TNewMessageType&gt;"/>.
        /// </returns>
        public static IChainBuilder<TNewMessageType> Select<TMessageType, TNewMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task<TNewMessageType>> selector)
        {
            var newMessagesTypeBuilder = new ChainBuilder<TNewMessageType>();

            chainBuilder.Handle(
                services =>
                    {
                        var handler = new SelectAsyncDecorator<TMessageType, TNewMessageType>(newMessagesTypeBuilder, selector);
                        services.BuilderNotifier.AddNotification(handler.ChainBuilt);
                        return handler.HandleMessageAsync;
                    });

            return newMessagesTypeBuilder;
        }
    }
}