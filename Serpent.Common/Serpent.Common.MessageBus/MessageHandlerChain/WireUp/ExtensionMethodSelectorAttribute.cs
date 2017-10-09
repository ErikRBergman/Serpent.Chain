namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;

    public class ExtensionMethodSelectorAttribute : Attribute
    {
        public ExtensionMethodSelectorAttribute(string identifier)
        {
            this.Identifier = identifier;
        }

        public string Identifier { get; }
    }
}