namespace Serpent.MessageHandlerChain.WireUp
{
    using System;

    /// <summary>
    /// Sets the name a type has when retrieving wire up from configuration
    /// </summary>
    public sealed class WireUpConfigurationNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WireUpConfigurationNameAttribute"/> class. 
        /// </summary>
        /// <param name="configurationName">
        /// The name of the configuration
        /// </param>
        public WireUpConfigurationNameAttribute(string configurationName)
        {
            this.ConfigurationName = configurationName;
        }

        /// <summary>
        /// The configuration name
        /// </summary>
        public string ConfigurationName { get; }
    }
}