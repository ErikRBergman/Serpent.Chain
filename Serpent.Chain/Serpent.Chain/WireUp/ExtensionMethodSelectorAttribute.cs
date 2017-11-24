namespace Serpent.Chain.WireUp
{
    using System;

    internal sealed class ExtensionMethodSelectorAttribute : Attribute
    {
        public ExtensionMethodSelectorAttribute(string identifier)
        {
            this.Identifier = identifier;
        }

        public string Identifier { get; }
    }
}