// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.MessageHandlerChain;

    /// <summary>
    ///  Provides a quick interface to create message handler chain and builders
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///  Creates a new message handler chain builder
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<T> Builder<T>() => new MessageHandlerChainBuilder<T>();

        /// <summary>
        /// Creates a new message handler chain
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <typeparam name="T">
        /// The message type
        /// </typeparam>
        /// <returns>
        /// A message handler chain builder
        /// </returns>
        public static IMessageHandlerChain<T> CreateChain<T>(Action<IMessageHandlerChainBuilder<T>> config)
        {
            var builder = Builder<T>();
            config(builder);
            return builder.BuildChain();
        }

        /// <summary>
        /// Creates a new message handler func
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <typeparam name="T">
        /// The message type
        /// </typeparam>
        /// <returns>
        /// A message handler chain builder
        /// </returns>
        public static Func<T, CancellationToken, Task> Func<T>(Action<IMessageHandlerChainBuilder<T>> config)
        {
            var builder = Builder<T>();
            config(builder);
            var notifier = new MessageHandlerChainBuildNotification();

            var func = builder.BuildFunc(new MessageHandlerChainBuilderSetupServices(notifier));

            notifier.Notify(new MessageHandlerChain<T>(func));

            return func;
        }
    }
}