namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;

    public class WireUpConfigurationNameAttribute : Attribute
    {
        public WireUpConfigurationNameAttribute(string configurationName)
        {
            this.ConfigurationName = configurationName;
        }

        public string ConfigurationName { get; }
    }
}