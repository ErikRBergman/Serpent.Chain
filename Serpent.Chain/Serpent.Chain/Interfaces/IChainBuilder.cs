// ReSharper disable once CheckNamespace

namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Notification;

    /// <summary>
    ///     The message handler chain builder interface
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IChainBuilder<TMessageType>
    {
        /// <summary>
        ///     Adds a decorator to the message handler chain
        /// </summary>
        /// <param name="addDecoratorFunc">A function that returns the method to call when building the Chain</param>
        /// <returns>The builder</returns>
        IChainBuilder<TMessageType> Decorate(
            Func<Func<TMessageType, CancellationToken, Task>, ChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addDecoratorFunc);

        /// <summary>
        ///     Adds the handler to the message handler chain
        /// </summary>
        /// <param name="addHandlerFunc">A function that returns the method to call when building the Chain</param>
        /// <returns>The builder</returns>
        IChainBuilder<TMessageType> Handle(
            Func<ChainBuilderSetupServices, Func<TMessageType, CancellationToken, Task>> addHandlerFunc);

        /// <summary>
        /// Builds a message handler chain from the decorators and the handler added
        /// </summary>
        /// <param name="disposeAction">
        /// The action to call when the chain dispose method is called.
        /// </param>
        /// <returns>
        /// The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;"/>.
        /// </returns>
        IChain<TMessageType> BuildChain(Action disposeAction = null);

        /// <summary>
        /// Builds the message handler chain
        /// </summary>
        /// <param name="services">
        /// The setup notification services.
        /// </param>
        /// <returns>
        /// The <see cref="Func&lt;TmessageType,CancellationToken,Task&gt;"/>.
        /// </returns>
        Func<TMessageType, CancellationToken, Task> BuildFunc(ChainBuilderSetupServices services);
    }
}