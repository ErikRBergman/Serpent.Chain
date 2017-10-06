namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WireUp
{
    using System;
    using System.Collections.Generic;

    using Serpent.Common.MessageBus.Interfaces;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Retry;

    public class WireUpMap
    {
        private readonly Dictionary<Type, IWireUp> wireUpItems = new Dictionary<Type, IWireUp>();

        private WireUpMap()
        {
            // Setup defaults
            this.AddWireUp(new RetryWireUp());
            this.AddWireUp(new ConcurrentWireUp());
        }

        public static WireUpMap Default { get; } = new WireUpMap();

        public WireUpMap AddWireUp(IWireUp mapItem)
        {
            this.wireUpItems.Add(mapItem.AttributeType, mapItem);
            return this;
        }

        public IMessageHandlerChainBuilder<TMessageType> WireUpHandler<TMessageType, TWireUpType, THandlerType>(IMessageHandlerChainBuilder<TMessageType> builder, THandlerType handler)
            where THandlerType : IMessageHandler<TMessageType>
        {
            var type = typeof(TWireUpType);
            var attributes = type.GetCustomAttributes(true);

            foreach (Attribute attribute in attributes)
            {
                var attributeType = attribute.GetType();

                if (this.wireUpItems.TryGetValue(attributeType, out var mapItem))
                {
                    mapItem.WireUp(attribute, builder, handler);
                }
            }

            builder.Handler(handler);
            return builder;
        }
    }
}