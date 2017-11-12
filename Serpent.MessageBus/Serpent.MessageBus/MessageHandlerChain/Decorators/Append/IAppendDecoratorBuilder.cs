// ReSharper disable ParameterHidesMember
// ReSharper disable CheckNamespace
namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a way to configure the append decorator
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IAppendDecoratorBuilder<TMessageType>
    {
        /// <summary>
        ///     Set to true to run the selected message through this Append recursively as long as the
        ///     "where predicate" returns true. If the predicate is omitted, a default predicate that returns true unless the selector returns default(TMessageType) is used
        /// </summary>
        /// <param name="isRecursive">True for recursive and false for non recursive (default)</param>
        /// <returns>A builder</returns>
        IAppendDecoratorBuilder<TMessageType> Recursive(bool isRecursive = true);

        /// <summary>
        /// Selects the message to append the message handler chain
        /// </summary>
        /// <param name="messageSelector">The function used to select the message to append</param>
        /// <returns>A builder</returns>
        IAppendDecoratorBuilder<TMessageType> Select(Func<TMessageType, CancellationToken, Task<TMessageType>> messageSelector);

        /// <summary>
        /// Appends only a message when the predicate function returns true. 
        /// For recursion: Recursion will only occur when the predicate returns true.
        /// If no predicate is specified, a default predicate that returns true unless the selector returns default(TMessageType) is used
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>A builder</returns>
        IAppendDecoratorBuilder<TMessageType> Where(Func<TMessageType, CancellationToken, Task<bool>> predicate);
    }
}