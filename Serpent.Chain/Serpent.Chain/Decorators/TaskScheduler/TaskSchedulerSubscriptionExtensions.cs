// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System.Threading.Tasks;

    using Serpent.Chain.Decorators.TaskScheduler;

    /// <summary>
    /// Provides extensions for the .DispatchOn* decorators
    /// </summary>
    public static class TaskSchedulerSubscriptionExtensions
    {
        /// <summary>
        /// Makes the message handler chain dispatch all messages on the context of the caller, setting up the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> DispatchOnCurrentContext<TMessageType>(this IChainBuilder<TMessageType> chainBuilder)
        {
            return chainBuilder.AddDecorator(currentHandler => new TaskSchedulerDecorator<TMessageType>(currentHandler, TaskScheduler.FromCurrentSynchronizationContext()));
        }

        /// <summary>
        /// Makes the message handler chain dispatch all messages on a specified Task Scheduler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="chainBuilder">The message handler chain builder</param>
        /// <param name="taskScheduler">The task scheduler to dispatch all messages on</param>
        /// <returns>A message handler chain builder</returns>
        public static IChainBuilder<TMessageType> DispatchOnTaskScheduler<TMessageType>(
            this IChainBuilder<TMessageType> chainBuilder,
            TaskScheduler taskScheduler)
        {
            return chainBuilder.AddDecorator(currentHandler => new TaskSchedulerDecorator<TMessageType>(currentHandler, taskScheduler).HandleMessageAsync);
        }
    }
}