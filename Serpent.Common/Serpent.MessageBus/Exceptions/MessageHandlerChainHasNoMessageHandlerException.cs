namespace Serpent.MessageBus.Exceptions
{
    using System;

    public class MessageHandlerChainHasNoMessageHandlerException : Exception
    {
        public MessageHandlerChainHasNoMessageHandlerException(string message)
            : base(message)
        {
        }
    }
}