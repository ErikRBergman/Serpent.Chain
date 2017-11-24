// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    /// <summary>
    /// Provides extensions for subscription wrapper.
    /// </summary>
    public static class ChainWrapperExtensions 
    {
        /// <summary>
        /// Creates a new subscription wrapper for this subscription
        /// </summary>
        /// <param name="chain">The subscription</param>
        /// <returns>A new subscription wrapper</returns>
        public static ChainWrapper Wrapper(this IChain chain)
        {
            return new ChainWrapper(chain);
        }
    }
}