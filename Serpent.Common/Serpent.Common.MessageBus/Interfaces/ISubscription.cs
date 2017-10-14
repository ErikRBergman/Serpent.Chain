// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A message bus subscription
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface ISubscription<in TMessageType>
    {
        /// <summary>
        /// The method to invoke for messages
        /// </summary>
        Func<TMessageType, CancellationToken, Task> SubscriptionHandlerFunc { get; }
    }
}