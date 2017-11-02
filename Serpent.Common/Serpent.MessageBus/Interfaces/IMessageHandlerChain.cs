namespace Serpent.MessageBus
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
    public interface IMessageHandlerChain<TMessageType> : IMessageHandlerChain
    {
        Func<TMessageType, CancellationToken, Task> MessageHandlerChainFunc { get; }
    }
}