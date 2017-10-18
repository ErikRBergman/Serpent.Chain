using System;
using System.Text;

namespace Serpent.Common.MessageBus.Interfaces
{
    public interface IMessageHandlerChainSubscriptionNotification
    {
        void AddNotification(Action<IMessageBusSubscription> subscription);
    }
}
