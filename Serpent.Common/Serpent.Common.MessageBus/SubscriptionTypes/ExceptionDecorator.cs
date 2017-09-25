namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Threading.Tasks;

    public class ExceptionDecorator<TMessageType> : MessageHandlerDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc;

        public ExceptionDecorator(Func<TMessageType, Task> handlerFunc, Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc)
        {
            this.handlerFunc = handlerFunc;
            this.exceptionHandlerFunc = exceptionHandlerFunc;
        }

        public ExceptionDecorator(MessageHandlerDecorator<TMessageType> innerSubscription, Func<TMessageType, Exception, Task<bool>> exceptionHandlerFunc)
        {
            this.handlerFunc = innerSubscription.HandleMessageAsync;
            this.exceptionHandlerFunc = exceptionHandlerFunc;
        }

        public override async Task HandleMessageAsync(TMessageType message)
        {
            try
            {
                await this.handlerFunc(message);
            }
            catch (Exception exception)
            {
                if (await this.exceptionHandlerFunc(message, exception))
                {
                    throw;
                }
            }
        }
    }
}