// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain
{
    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    /// The distinct attribute
    /// </summary>
    public class DistinctAttribute : WireUpAttribute
    {
        /// <summary>
        /// Ensures a message of a certain key only passes once
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public DistinctAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Ensures the bus will only allow a single message
        /// </summary>
        public DistinctAttribute()
        {
        }

        /// <summary>
        /// The property name
        /// </summary>
        public string PropertyName { get; }
    }
}