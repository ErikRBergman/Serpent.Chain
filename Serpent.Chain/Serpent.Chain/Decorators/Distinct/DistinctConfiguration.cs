// ReSharper disable CheckNamespace
namespace Serpent.Chain
{
    using Serpent.Chain.WireUp;

    /// <summary>
    /// The distinct decorator wire up configuration
    /// </summary>
    [WireUpConfigurationName("Dinstinct")]
    public class DistinctConfiguration
    {
        /// <summary>
        /// The name of the message property used for message key
        /// </summary>
        public string PropertyName { get; set; }
    }
}