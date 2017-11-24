// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an interface to a message handler chain
    /// </summary>
    public interface IChain : IDisposable
    {
    }

    /// <summary>
    /// Provides an interface to a message handler chain
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IChain<in TMessageType> : IChain
    {
        /// <summary>
        /// Gets the message handler chain function
        /// </summary>
        Func<TMessageType, CancellationToken, Task> ChainFunc { get; }
    }
}