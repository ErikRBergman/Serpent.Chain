namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry
{
    using System;
    using System.Threading.Tasks;

    public interface IMessageHandlerRetry<in TMessageType>
    {
        Task HandleRetryAsync(TMessageType message, Exception exception, TimeSpan delay, int attemptNumber, int maxNumberOfAttemps);
    }
}