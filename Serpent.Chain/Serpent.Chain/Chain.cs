// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides a message handler chain
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct Chain<TMessageType> : IChain<TMessageType>
    {
        private readonly Action disposeAction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Chain{TMessageType}" /> struct.
        /// </summary>
        /// <param name="chainFunc">
        ///     The method for the chain
        /// </param>
        /// <param name="disposeAction">
        ///     An action used to dispose this message handler chain
        /// </param>
        public Chain(Func<TMessageType, CancellationToken, Task> chainFunc, Action disposeAction)
        {
            this.disposeAction = disposeAction;
            this.ChainFunc = chainFunc;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Chain{TMessageType}" /> struct.
        /// </summary>
        /// <param name="chainFunc">
        ///     The method for the chain
        /// </param>
        public Chain(Func<TMessageType, CancellationToken, Task> chainFunc)
        {
            this.disposeAction = null;
            this.ChainFunc = chainFunc;
        }

        /// <summary>
        ///     The message handler chain method
        /// </summary>
        public Func<TMessageType, CancellationToken, Task> ChainFunc { get; }

        /// <summary>
        ///     Disposes the message handler chain
        /// </summary>
        public void Dispose()
        {
            this.disposeAction?.Invoke();
        }
    }
}