// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    public static class TaskSchedulerSubscriptionExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> DispatchOnCurrentContext<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new TaskSchedulerDecorator<TMessageType>(currentHandler, TaskScheduler.FromCurrentSynchronizationContext()));
        }

        public static IMessageHandlerChainBuilder<TMessageType> DispatchOnTaskScheduler<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            TaskScheduler taskScheduler)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new TaskSchedulerDecorator<TMessageType>(currentHandler, taskScheduler).HandleMessageAsync);
        }
    }
}