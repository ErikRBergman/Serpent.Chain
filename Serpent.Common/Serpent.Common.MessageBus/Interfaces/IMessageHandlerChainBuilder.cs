// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public interface IMessageHandlerChainBuilder<TMessageType>
    {
        IMessageHandlerChainBuilder<TMessageType> Add(Func<Func<TMessageType, Task>, Func<TMessageType, Task>> addFunc);

        IMessageBusSubscription Factory<THandler>(Func<THandler> handlerFactory)
            where THandler : IMessageHandler<TMessageType>;

        IMessageBusSubscription Handler(Func<TMessageType, Task> handlerFunc);
    }
}