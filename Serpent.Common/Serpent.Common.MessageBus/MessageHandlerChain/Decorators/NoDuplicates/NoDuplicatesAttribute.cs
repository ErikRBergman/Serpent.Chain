// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// The NoDuplicates attribute
    /// </summary>
    public class NoDuplicatesAttribute : WireUpAttribute
    {
        /// <summary>
        /// Initializes an attribute where the property named propertyName is used as key to eliminate duplicates
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public NoDuplicatesAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public NoDuplicatesAttribute()
        {
        }

        public string PropertyName { get; }
    }
}