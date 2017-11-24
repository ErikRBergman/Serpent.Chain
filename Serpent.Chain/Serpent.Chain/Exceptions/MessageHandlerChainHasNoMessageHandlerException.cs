namespace Serpent.Chain.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when a chain that requires a handler has none
    /// </summary>
    public class ChainHasNoMessageHandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChainHasNoMessageHandlerException"/> class. 
        /// </summary>
        /// <param name="message">The exception message
        /// </param>
        public ChainHasNoMessageHandlerException(string message)
            : base(message)
        {
        }
    }
}