namespace Serpent.Common.MessageBus.Exceptions
{
    using System;

    /// <summary>
    /// The no handler exception
    /// </summary>
    public class NoHandlerException : Exception
    {
        /// <summary>
        /// Creates a new instance of no handler exception
        /// </summary>
        /// <param name="message">The message text</param>
        public NoHandlerException(string message)
            : base(message)
        {
        }
    }
}