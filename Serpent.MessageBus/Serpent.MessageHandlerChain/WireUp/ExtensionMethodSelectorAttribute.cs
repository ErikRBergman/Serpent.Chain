namespace Serpent.MessageHandlerChain.WireUp
{
    using System;

    internal class ExtensionMethodSelectorAttribute : Attribute
    {
        public ExtensionMethodSelectorAttribute(string identifier)
        {
            this.Identifier = identifier;
        }

        public string Identifier { get; }
    }
}