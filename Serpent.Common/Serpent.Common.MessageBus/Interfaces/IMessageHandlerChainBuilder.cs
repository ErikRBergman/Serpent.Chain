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
        IMessageHandlerChainBuilder<TMessageType> Handle(
            Func<MessageHandlerChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addHandlerFunc);

        /// <summary>
        ///     Builds a message handler chain from the decorators and the handler added
        /// </summary>
        /// <returns>
        ///     The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;" />.
        /// </returns>
        IMessageHandlerChain<TMessageType> BuildChain();

        /// <summary>
        /// Builds the message handler chain
        /// </summary>
        /// <param name="services">
        /// The setup notification services.
        /// </param>
        /// <returns>
        /// The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;"/>.
        /// </returns>
        Func<TMessageType, CancellationToken, Task> BuildFunc(MessageHandlerChainBuilderSetupServices services);
    }
}