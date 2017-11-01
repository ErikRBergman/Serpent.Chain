namespace Serpent.Common.MessageBus.Exceptions
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