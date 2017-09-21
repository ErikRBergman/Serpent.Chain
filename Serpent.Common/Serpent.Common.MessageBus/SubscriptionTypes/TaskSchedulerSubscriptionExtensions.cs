// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class TaskSchedulerSubscriptionExtensions
    {
        public static SubscriptionBuilder<TMessageType> DispatchOnCurrentContext<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder)
        {
            return subscriptionBuilder.Add(currentHandler => new TaskSchedulerSubscription<TMessageType>(currentHandler, TaskScheduler.FromCurrentSynchronizationContext()));
        }

        public static SubscriptionBuilder<TMessageType> DispatchOnTaskScheduler<TMessageType>(this SubscriptionBuilder<TMessageType> subscriptionBuilder, TaskScheduler taskScheduler)
        {
            return subscriptionBuilder.Add(currentHandler => new TaskSchedulerSubscription<TMessageType>(currentHandler, taskScheduler).HandleMessageAsync);
        }
    }
}