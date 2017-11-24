namespace Serpent.Chain.Decorators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a common interface to build decorators
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IDecoratorBuilder<TMessageType>
    {
        /// <summary>
        /// Builds the decorator
        /// </summary>
        /// <returns>The method used to call the decorator</returns>
        Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator();
    }

    /// <summary>
    /// Provides a common interface to build decorators
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    /// <typeparam name="TNewType">The new message type</typeparam>
    public interface IDecoratorBuilder<TMessageType, TNewType>
    {
        /// <summary>
        /// Builds the decorator
        /// </summary>
        /// <returns>The method used to call the decorator</returns>
        Func<Func<TMessageType, CancellationToken, Task>, Func<TNewType, CancellationToken, Task>> BuildDecorator();
    }
}