namespace Serpent.Chain.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when a wire up could not parse the configuration text
    /// </summary>
    public class CouldNotParseConfigTextToTimeSpanException : Exception
    {
        /// <inheritdoc />
        public CouldNotParseConfigTextToTimeSpanException(string message, string invalidText)
            : base(message)
        {
            this.InvalidText = invalidText;
        }

        /// <summary>
        /// Gets the text that failed parsing
        /// </summary>
        public string InvalidText { get; }
    }
}