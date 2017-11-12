// ReSharper disable CheckNamespace
namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides extensions for configuring the append decorator
    /// </summary>
    public static class AppendManyDecoratorBuilderExtensions
    {
        /// <summary>
        /// The function used to select the message to append
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="builder">The append decorator builder</param>
        /// <param name="messageSelector">The function used to select the message to append</param>
        /// <returns>A builder</returns>
        public static IAppendManyDecoratorBuilder<TMessageType> Select<TMessageType>(this IAppendManyDecoratorBuilder<TMessageType> builder, Func<TMessageType, Task<IEnumerable<TMessageType>>> messageSelector)
        {
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
        public static IAppendManyDecoratorBuilder<TMessageType> Select<TMessageType>(this IAppendManyDecoratorBuilder<TMessageType> builder, Func<TMessageType, IEnumerable<TMessageType>> messageSelector)
        {
            return builder.Select((message, token) => Task.FromResult(messageSelector(message)));
        }

        /// <summary>
        /// Appends only messages when the predicate function returns true. 
        /// For recursion: Recursion will only occur when the predicate returns true.
        /// If no predicate is specified, a default predicate that returns true unless the selector returns default(TMessageType) is used
        /// </summary>
        /// <typeparam name="TMessageType">The message type
        /// </typeparam>
        /// <param name="builder">The append decorator builder</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>A builder</returns>
        public static IAppendManyDecoratorBuilder<TMessageType> Where<TMessageType>(this IAppendManyDecoratorBuilder<TMessageType> builder, Func<TMessageType, Task<bool>> predicate)
        {
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
        public static IAppendManyDecoratorBuilder<TMessageType> Where<TMessageType>(this IAppendManyDecoratorBuilder<TMessageType> builder, Func<TMessageType, bool> predicate)
        {
            return builder.Where((message, token) => Task.FromResult(predicate(message)));
        }
    }
}