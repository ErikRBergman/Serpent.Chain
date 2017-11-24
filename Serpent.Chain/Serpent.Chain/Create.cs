// ReSharper disable once CheckNamespace
namespace Serpent.Chain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Notification;

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
        public static IChainBuilder<T> Builder<T>() => new ChainBuilder<T>();

        /// <summary>
        /// Creates a new message handler chain
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="disposeAction">The action called when the chain is disposed</param>
        /// <typeparam name="T">
        /// The message type
        /// </typeparam>
        /// <returns>
        /// A message handler chain builder
        /// </returns>
        public static IChain<T> CreateChain<T>(Action<IChainBuilder<T>> config, Action disposeAction = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var builder = Builder<T>();
            config(builder);
            return builder.BuildChain(disposeAction);
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
        public static Func<T, CancellationToken, Task> Func<T>(Action<IChainBuilder<T>> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var builder = Builder<T>();
            config(builder);
            var notifier = new ChainBuilderNotifier();

            var func = builder.BuildFunc(new ChainBuilderSetupServices(notifier));

#pragma warning disable CC0022 // Should dispose object
            notifier.Notify(new Chain<T>(func));
#pragma warning restore CC0022 // Should dispose object

            return func;
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
        public static Func<T, Task> SimpleFunc<T>(Action<IChainBuilder<T>> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var builder = Builder<T>();
            config(builder);
            var notifier = new ChainBuilderNotifier();

            var func = builder.BuildFunc(new ChainBuilderSetupServices(notifier));

#pragma warning disable CC0022 // Should dispose object
            notifier.Notify(new Chain<T>(func));
#pragma warning restore CC0022 // Should dispose object

            return msg => func(msg, CancellationToken.None);
        }
    }
}