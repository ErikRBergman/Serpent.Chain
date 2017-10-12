namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Delay;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.LimitedThroughput;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.NoDuplicates;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SoftFireAndForget;

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
                .AddWireUp(new NoDuplicatesWireUp());
        }

        public static WireUpMap Default { get; } = new WireUpMap();

        public WireUpMap AddWireUp(IWireUp mapItem)
        {
            this.attributeWireUpItems.Add(mapItem.AttributeType, mapItem);
            this.configurationWireUpItems.Add(mapItem.ConfigurationType, mapItem);

            var name = mapItem.ConfigurationType.GetCustomAttribute<WireUpConfigurationNameAttribute>().ConfigurationName;
            this.configurationWireUpItemsByName.Add(name, mapItem);
            return this;
        }

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

        public IMessageHandlerChainBuilder<TMessageType> WireUpHandlerFromConfiguration<TMessageType, THandlerType>(IMessageHandlerChainBuilder<TMessageType> builder, THandlerType handler, IEnumerable<object> configuration)
            where THandlerType : IMessageHandler<TMessageType>
        {
            foreach (object configurationItem in configuration.Where(c => c != null))
            {
                if (this.attributeWireUpItems.TryGetValue(configurationItem.GetType(), out var mapItem))
                {
                    mapItem.WireUpFromConfiguration(configurationItem, builder, handler);
                }
            }

            builder.Handler(handler);
            return builder;
        }
    }
}