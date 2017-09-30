namespace Serpent.Common.MessageBus.Exceptions
{
    using System;

    public class NoHandlerException : Exception
    {
        public NoHandlerException(string message)
            : base(message)
        {
        }
    }
}