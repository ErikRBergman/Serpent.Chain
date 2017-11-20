namespace Serpent.MessageHandlerChain.Exceptions
{
    using System;

    /// <summary>
    /// The message handler chain already has a handler
    /// </summary>
    public class MessageHandlerChainHasAHandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerChainHasAHandlerException"/> class. 
        /// </summary>
        /// <param name="message">The exception text message
        /// </param>
        public MessageHandlerChainHasAHandlerException(string message)
            : base(message)
        {
        }
    }
}