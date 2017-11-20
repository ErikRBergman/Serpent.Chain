// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides a message handler chain
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct MessageHandlerChain<TMessageType> : IMessageHandlerChain<TMessageType>
    {
        private readonly Action disposeAction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageHandlerChain{TMessageType}" /> struct.
        /// </summary>
        /// <param name="messageHandlerChainFunc">
        ///     The method for the chain
        /// </param>
        /// <param name="disposeAction">
        ///     An action used to dispose this message handler chain
        /// </param>
        public MessageHandlerChain(Func<TMessageType, CancellationToken, Task> messageHandlerChainFunc, Action disposeAction)
        {
            this.disposeAction = disposeAction;
            this.MessageHandlerChainFunc = messageHandlerChainFunc;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageHandlerChain{TMessageType}" /> struct.
        /// </summary>
        /// <param name="messageHandlerChainFunc">
        ///     The method for the chain
        /// </param>
        public MessageHandlerChain(Func<TMessageType, CancellationToken, Task> messageHandlerChainFunc)
        {
            this.disposeAction = null;
            this.MessageHandlerChainFunc = messageHandlerChainFunc;
        }

        /// <summary>
        ///     The message handler chain method
        /// </summary>
        public Func<TMessageType, CancellationToken, Task> MessageHandlerChainFunc { get; }

        /// <summary>
        ///     Disposes the message handler chain
        /// </summary>
        public void Dispose()
        {
            this.disposeAction?.Invoke();
        }
    }
}