// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    /// Make the rest of the message handler chain a weak reference
    /// </summary>
    public class WeakReferenceAttribute : WireUpAttribute
    {
    }
}