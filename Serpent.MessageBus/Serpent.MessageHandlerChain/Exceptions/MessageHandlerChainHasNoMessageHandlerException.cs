namespace Serpent.MessageHandlerChain.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when a chain that requires a handler has none
    /// </summary>
    public class MessageHandlerChainHasNoMessageHandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerChainHasNoMessageHandlerException"/> class. 
        /// </summary>
        /// <param name="message">The exception message
        /// </param>
        public MessageHandlerChainHasNoMessageHandlerException(string message)
            : base(message)
        {
        }
    }
}