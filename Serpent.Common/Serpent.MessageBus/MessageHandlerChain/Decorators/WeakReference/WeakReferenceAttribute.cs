// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using Serpent.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    ///     Make the rest of the message handler chain a weak reference
    /// </summary>
    public class WeakReferenceAttribute : WireUpAttribute
    {
    }
}