// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.SubscriptionTypes;

    public static class ExceptionExtensions
    {
        private static readonly Task<bool> FalseTask = Task.FromResult(false);

        public static SubscriptionBuilder<TMessageType> Exception<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc)
        {
            return subscriptionBuilder.Add(currentHandler => new ExceptionSubscription<TMessageType>(currentHandler, exceptionHandlerFunc));
        }

        public static SubscriptionBuilder<TMessageType> Exception<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            Func<TMessageType, Exception, Task> exceptionHandlerFunc)
        {
            return subscriptionBuilder.Add(
                currentHandler => new ExceptionSubscription<TMessageType>(
                    currentHandler,
                    async (message, exception) =>
                        {
                            await exceptionHandlerFunc(message, exception);
                            return false;
                        }));
        }

        public static SubscriptionBuilder<TMessageType> Exception<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            Func<TMessageType, Exception, bool> exceptionHandlerFunc)
        {
            return subscriptionBuilder.Add(
                currentHandler => new ExceptionSubscription<TMessageType>(
                    currentHandler,
                    (message, exception) => Task.FromResult(exceptionHandlerFunc(message, exception))));
        }

        public static SubscriptionBuilder<TMessageType> Exception<TMessageType>(
            this SubscriptionBuilder<TMessageType> subscriptionBuilder,
            Action<TMessageType, Exception> exceptionHandlerAction)
        {
            return subscriptionBuilder.Add(
                currentHandler => new ExceptionSubscription<TMessageType>(
                    currentHandler,
                    (message, exception) =>
                        {
                            exceptionHandlerAction(message, exception);
                            return FalseTask;
                        }));
        }
    }
}