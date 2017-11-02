namespace Serpent.MessageBus.Exceptions
{
    using System;

    public class MessageHandlerChainHasAHandlerException : Exception
    {
        public MessageHandlerChainHasAHandlerException(string message)
            : base(message)
        {
        }
    }
}