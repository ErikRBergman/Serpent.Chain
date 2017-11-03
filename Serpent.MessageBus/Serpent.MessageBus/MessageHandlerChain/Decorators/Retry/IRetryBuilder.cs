namespace Serpent.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a way to configure the retry message handler chain decorator
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IRetryBuilder<TMessageType>
    {
        /// <summary>
        /// The maximum number of tries (first attempts + retries)
        /// </summary>
        int MaximumNumberOfAttempts { get; set; }

        /// <summary>
        /// The delay between retries
        /// </summary>
        TimeSpan RetryDelay { get; set; }
        
        /// <summary>
        /// The method invoked after an attempt to handle a message fails (throws an exception)
        /// </summary>
        Func<TMessageType, Exception, int, int, TimeSpan, CancellationToken, Task> HandlerFailedFunc { get; set; }

        /// <summary>
        /// The method invoked after an attempt to handle a message fails (throws an exception)
        /// </summary>
        Func<TMessageType, int, int, TimeSpan, Task> HandlerSucceededFunc { get; set; }
    }
}