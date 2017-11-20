// ReSharper disable ParameterHidesMember
// ReSharper disable CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides a way to configure the action decorator
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IActionDecoratorBuilder<TMessageType>
    {
        /// <summary>
        ///     Invokes a method before the message is handled
        /// </summary>
        /// <param name="beforeFunc">The method</param>
        /// <returns>A builder</returns>
        IActionDecoratorBuilder<TMessageType> Before(Func<TMessageType, CancellationToken, Task> beforeFunc);

        /// <summary>
        ///     Invokes a method when a
        /// </summary>
        /// <param name="finallyFunc">The method</param>
        /// <returns>A builder</returns>
        IActionDecoratorBuilder<TMessageType> Finally(Func<TMessageType, CancellationToken, Exception, Task> finallyFunc);

        /// <summary>
        ///     Invokes a method when a message is cancelled
        /// </summary>
        /// <param name="onCancelFunc">The method</param>
        /// <returns>A builder</returns>
        IActionDecoratorBuilder<TMessageType> OnCancel(Func<TMessageType, Task> onCancelFunc);

        /// <summary>
        ///     Invokes a method when a message is cancelled
        /// </summary>
        /// <param name="onExceptionFunc">The method</param>
        /// <returns>A builder</returns>
        IActionDecoratorBuilder<TMessageType> OnException(Func<TMessageType, Exception, Task<bool>> onExceptionFunc);

        /// <summary>
        ///     Invokes a method when a message is handled successfully
        /// </summary>
        /// <param name="onSuccessFunc">The method</param>
        /// <returns>A builder</returns>
        IActionDecoratorBuilder<TMessageType> OnSuccess(Func<TMessageType, Task> onSuccessFunc);
    }
}