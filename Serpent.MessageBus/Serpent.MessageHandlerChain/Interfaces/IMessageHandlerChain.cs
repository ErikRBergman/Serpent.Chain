// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface to a message handler chain
    /// </summary>
    public interface IMessageHandlerChain : IDisposable
    {
    }

    /// <summary>
    /// Provides an interface to a message handler chain
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageHandlerChain<in TMessageType> : IMessageHandlerChain
    {
        /// <summary>
        /// Gets the message handler chain function
        /// </summary>
        Func<TMessageType, CancellationToken, Task> MessageHandlerChainFunc { get; }
    }
}