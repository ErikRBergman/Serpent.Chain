namespace Serpent.Chain.Exceptions
{
    using System;

    /// <summary>
    /// The message handler chain already has a handler
    /// </summary>
    public class ChainHasAHandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChainHasAHandlerException"/> class. 
        /// </summary>
        /// <param name="message">The exception text message
        /// </param>
        public ChainHasAHandlerException(string message)
            : base(message)
        {
        }
    }
}