// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.SelectMany;

    /// <summary>
    /// The .SelectMany() decorator extensions type
    /// </summary>
    public static class SelectManyExtensions
    {
        /// <summary>
        /// Projects each element of a sequence, producing a new chain with the inner element type
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="chainBuilder">
        /// The builder
        /// </param>
        /// <param name="selector">
        /// A selector
        /// </param>
        /// <returns>
        /// The <see cref="IChainBuilder&lt;TMessageType&gt;"/>.
        /// </returns>
        public static IChainBuilder<TNewMessageType> SelectMany<TMessageType, TNewMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, IEnumerable<TNewMessageType>> selector)
        {
            var newMessagesTypeBuilder = new ChainBuilder<TNewMessageType>();

            chainBuilder.Handle(
                services =>
                    {
                        var handler = new SelectManyDecorator<TMessageType, TNewMessageType>(newMessagesTypeBuilder, selector);
                        services.BuilderNotifier.AddNotification(handler.ChainBuilt);
                        return handler.HandleMessageAsync;
                    });

            return newMessagesTypeBuilder;
        }

        /// <summary>
        /// Projects each element of a sequence, producing a new chain with the inner element type
        /// </summary>
        /// <typeparam name="TMessageType">
        /// The message type
        /// </typeparam>
        /// <typeparam name="TNewMessageType">
        /// The new message type
        /// </typeparam>
        /// <param name="chainBuilder">
        /// The builder
        /// </param>
        /// <param name="selector">
        /// A selector
        /// </param>
        /// <returns>
        /// The <see cref="IChainBuilder&lt;TMessageType&gt;"/>.
        /// </returns>
        public static IChainBuilder<TNewMessageType> SelectMany<TMessageType, TNewMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            Func<TMessageType, Task<IEnumerable<TNewMessageType>>> selector)
        {
            var newMessagesTypeBuilder = new ChainBuilder<TNewMessageType>();

            chainBuilder.Handle(
                services =>
                    {
                        var handler = new SelectManyAsyncDecorator<TMessageType, TNewMessageType>(newMessagesTypeBuilder, selector);
                        services.BuilderNotifier.AddNotification(handler.ChainBuilt);
                        return handler.HandleMessageAsync;
                    });

            return newMessagesTypeBuilder;
        }
    }
}