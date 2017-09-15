// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;

    public interface IMessageBusSubscription : IDisposable
    {
        void Unsubscribe();
    }
}