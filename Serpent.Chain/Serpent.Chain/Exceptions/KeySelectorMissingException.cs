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

        public static KeySelectorMissingException CreateDefault() => new KeySelectorMissingException("KeySelector not set and it can not be inferred from equality comparer");
    }
}