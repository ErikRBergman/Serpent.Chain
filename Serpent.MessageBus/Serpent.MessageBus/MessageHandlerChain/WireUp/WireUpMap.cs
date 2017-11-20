namespace Serpent.MessageBus.MessageHandlerChain.WireUp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Serpent.MessageBus.Interfaces;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.Concurrent;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.Delay;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.Distinct;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.LimitedThroughput;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.NoDuplicates;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.Retry;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.SoftFireAndForget;
    using Serpent.MessageBus.MessageHandlerChain.Decorators.WeakReference;

    /// <summary>
    /// Wires up message handler chain subscriptions
    /// </summary>
    public class WireUpMap
    {
        private readonly Dictionary<Type, IWireUp> attributeWireUpItems = new Dictionary<Type, IWireUp>();
        private readonly Dictionary<Type, IWireUp> configurationWireUpItems = new Dictionary<Type, IWireUp>();
        private readonly Dictionary<string, IWireUp> configurationWireUpItemsByName = new Dictionary<string, IWireUp>();

        private WireUpMap()
        {
            // Setup defaults
            this.AddWireUp(new RetryWireUp())
                .AddWireUp(new ConcurrentWireUp())
                .AddWireUp(new DelayWireUp())
                .AddWireUp(new SoftFireAndForgetWireUp())
                .AddWireUp(new LimitedThroughputWireUp())
                .AddWireUp(new DistinctWireUp())
                .AddWireUp(new WeakReferenceWireUp())
                .AddWireUp(new NoDuplicatesWireUp());
        }

        /// <summary>
        /// The default wire up map, containing all the built in message bus wire up types
        /// </summary>
        public static WireUpMap Default { get; } = new WireUpMap();

        /// <summary>
        /// Adds a WireUp type to the mpa
        /// </summary>
        /// <param name="wireUp">The wire up to add</param>
        /// <returns>This WireUpMap</returns>
        public WireUpMap AddWireUp(IWireUp wireUp)
        {
            this.attributeWireUpItems.Add(wireUp.AttributeType, wireUp);
            this.configurationWireUpItems.Add(wireUp.ConfigurationType, wireUp);

            var name = wireUp.ConfigurationType.GetCustomAttribute<WireUpConfigurationNameAttribute>().ConfigurationName;
            this.configurationWireUpItemsByName.Add(name, wireUp);
            return this;
        }

        /// <summary>
        /// Wires up message handler decorators and handler from the TWireUpType's attributes
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TWireUpType">The type to get wire up decorator attributes from</typeparam>
        /// <typeparam name="THandlerType">The message handler type</typeparam>
        /// <param name="builder">The message handler chain builder</param>
        /// <param name="handler">The message handler</param>
        /// <returns>A message handler chain builder</returns>
        public IMessageHandlerChainBuilder<TMessageType> WireUpHandlerFromAttributes<TMessageType, TWireUpType, THandlerType>(IMessageHandlerChainBuilder<TMessageType> builder, THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            var type = typeof(TWireUpType);
            var attributes = type.GetCustomAttributes(true);

            foreach (Attribute attribute in attributes)
            {
                var attributeType = attribute.GetType();

                if (this.attributeWireUpItems.TryGetValue(attributeType, out var mapItem))
                {
                    mapItem.WireUpFromAttribute(attribute, builder, handler);
                }
            }

            builder.Handler(handler);
            return builder;
        }

        /// <summary>
        /// Wires up message handler decorators and handler from the a collection of wire up configuration objects
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="THandlerType">The message handler type</typeparam>
        /// <param name="builder">The message handler chain builder</param>
        /// <param name="handler">The message handler</param>
        /// <param name="wireUpConfigurationObjects">The configuration object</param>
        /// <returns>A message handler chain builder</returns>
        public IMessageHandlerChainBuilder<TMessageType> WireUpHandlerFromConfiguration<TMessageType, THandlerType>(IMessageHandlerChainBuilder<TMessageType> builder, THandlerType handler, IEnumerable<object> wireUpConfigurationObjects)
            where THandlerType : IMessageHandler<TMessageType>
        {
            foreach (var configurationItem in wireUpConfigurationObjects.Where(c => c != null))
            {
                if (this.configurationWireUpItems.TryGetValue(configurationItem.GetType(), out var mapItem))
                {
                    mapItem.WireUpFromConfiguration(configurationItem, builder, handler);
                }
            }

            if (handler != null)
            {
                builder.Handler(handler);
            }

            return builder;
        }
    }
}