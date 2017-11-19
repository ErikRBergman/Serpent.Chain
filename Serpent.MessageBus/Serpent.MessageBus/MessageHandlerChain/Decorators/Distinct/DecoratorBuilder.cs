namespace Serpent.MessageBus.MessageHandlerChain.Decorators.Distinct
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base for decorator builders
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public abstract class DecoratorBuilder<TMessageType> : IDecoratorBuilder<TMessageType>
    {
        /// <inheritdoc />
        public abstract Func<Func<TMessageType, CancellationToken, Task>, Func<TMessageType, CancellationToken, Task>> BuildDecorator();
    }
}