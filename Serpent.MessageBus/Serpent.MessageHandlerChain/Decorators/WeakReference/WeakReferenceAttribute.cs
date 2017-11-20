// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    ///     Make the rest of the message handler chain a weak reference
    /// </summary>
    public class WeakReferenceAttribute : WireUpAttribute
    {
    }
}