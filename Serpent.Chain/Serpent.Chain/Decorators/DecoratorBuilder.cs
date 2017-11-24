// ReSharper disable StyleCop.SA1402
namespace Serpent.Chain.Decorators
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

    /// <summary>
    /// Provides a base for decorator builders
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    /// <typeparam name="TNewType">The new message type</typeparam>
    public abstract class DecoratorBuilder<TMessageType, TNewType> : IDecoratorBuilder<TMessageType, TNewType>
    {
        /// <inheritdoc />
        public abstract Func<Func<TMessageType, CancellationToken, Task>, Func<TNewType, CancellationToken, Task>> BuildDecorator();
    }
}