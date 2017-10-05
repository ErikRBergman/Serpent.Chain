// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.BranchOut;

    public static class BranchOutExtensions
    {
        public static IMessageHandlerChainBuilder<TMessageType> BranchOut<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            params Action<IMessageHandlerChainBuilder<TMessageType>>[] branches)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new BranchOutDecorator<TMessageType>(currentHandler, branches));
        }
    }
}