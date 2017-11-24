namespace Serpent.Chain.Exceptions
{
    using System;

    /// <summary>
    ///  Provides an exception when key selector is missing
    /// </summary>
    public class KeySelectorMissingException : Exception
    {
        /// <inheritdoc />
        public KeySelectorMissingException(string message) : base(message)
        {
        }
    }
}