namespace Serpent.MessageBus.Exceptions
{
    using System;

    public class CouldNotParseConfigTextToTimeSpanException : Exception
    {
        /// <inheritdoc />
        public CouldNotParseConfigTextToTimeSpanException(string message, string invalidText)
            : base(message)
        {
            this.InvalidText = invalidText;
        }

        public string InvalidText { get; }
    }
}