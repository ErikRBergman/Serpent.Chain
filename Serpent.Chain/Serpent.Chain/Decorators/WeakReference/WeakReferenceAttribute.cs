// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using Serpent.Chain.WireUp;

    /// <summary>
    ///     Make the rest of the message handler chain a weak reference
    /// </summary>
    public sealed class WeakReferenceAttribute : WireUpAttribute
    {
    }
}