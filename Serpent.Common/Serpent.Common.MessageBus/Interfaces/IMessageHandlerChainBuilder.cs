// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public interface IMessageHandlerChainBuilder<TMessageType>
    {
        IMessageHandlerChainBuilder<TMessageType> Add(Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> addFunc);

        IMessageBusSubscription Factory<THandler>(Func<THandler> handlerFactory)
            where THandler : IMessageHandler<TMessageType>;

        IMessageBusSubscription Handler(Func<TMessageType, CancellationToken, Task> handlerFunc);
    }
}