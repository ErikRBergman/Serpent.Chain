// ReSharper disable CheckNamespace
namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extensions for configuring the append decorator
    /// </summary>
    public static class AppendDecoratorBuilderExtensions
    {
        /// <summary>
        /// The function used to select the message to append
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="builder">The append decorator builder</param>
        /// <param name="messageSelector">The function used to select the message to append</param>
        /// <returns>A builder</returns>
        public static IAppendDecoratorBuilder<TMessageType> Select<TMessageType>(this IAppendDecoratorBuilder<TMessageType> builder, Func<TMessageType, Task<TMessageType>> messageSelector)
        {
            if (messageSelector == null)
            {
                throw new ArgumentNullException(nameof(messageSelector));
            }

            return builder.Select((message, token) => messageSelector(message));
        }

        /// <summary>
        /// The function used to select the message to append
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="builder">
        /// The append decorator builder
        /// </param>
        /// <param name="messageSelector">
        /// The function used to select the message to append
        /// </param>
        /// <returns>
        /// A builder
        /// </returns>
        public static IAppendDecoratorBuilder<TMessageType> Select<TMessageType>(this IAppendDecoratorBuilder<TMessageType> builder, Func<TMessageType, TMessageType> messageSelector)
        {
            if (messageSelector == null)
            {
                throw new ArgumentNullException(nameof(messageSelector));
            }

            return builder.Select((message, token) => Task.FromResult(messageSelector(message)));
        }

        /// <summary>
        /// Appends only a message when the predicate function returns true. 
        /// For recursion: Recursion will only occur when the predicate returns true.
        /// If no predicate is specified, a default predicate that returns true unless the selector returns default(TMessageType) is used
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="builder">The append decorator builder</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>A builder</returns>
        public static IAppendDecoratorBuilder<TMessageType> Where<TMessageType>(this IAppendDecoratorBuilder<TMessageType> builder, Func<TMessageType, Task<bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return builder.Where((message, token) => predicate(message));
        }

        /// <summary>
        /// Appends only a message when the predicate function returns true. 
        /// For recursion: Recursion will only occur when the predicate returns true.
        /// If no predicate is specified, a default predicate that returns true unless the selector returns default(TMessageType) is used
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="builder">The append decorator builder</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>A builder</returns>
        public static IAppendDecoratorBuilder<TMessageType> Where<TMessageType>(this IAppendDecoratorBuilder<TMessageType> builder, Func<TMessageType, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return builder.Where((message, token) => Task.FromResult(predicate(message)));
        }
    }
}