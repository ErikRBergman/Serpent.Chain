// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain;

    /// <summary>
    ///     The message handler chain builder interface
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageHandlerChainBuilder<TMessageType>
    {
        /// <summary>
        ///     Adds a decorator to the message handler chain
        /// </summary>
        /// <param name="addDecoratorFunc">A function that returns the method to call when building the chain</param>
        /// <returns>The builder</returns>
        IMessageHandlerChainBuilder<TMessageType> Decorate(
            Func<Func<TMessageType, CancellationToken, Task>, MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addDecoratorFunc);

        /// <summary>
        ///     Adds the handler to the message handler chain
        /// </summary>
        /// <param name="addHandlerFunc">A function that returns the method to call when building the chain</param>
        //// <returns>The builder</returns>
        void Handler(
            Func<MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addHandlerFunc);
    }
}