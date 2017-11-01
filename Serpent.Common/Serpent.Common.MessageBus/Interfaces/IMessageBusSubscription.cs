// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;

    /// <summary>
    /// The message bus subscription interface. Call .Dispose() to unsubscribe.
    /// </summary>
    public interface IMessageBusSubscription : IDisposable
    {
    }
}