////// ReSharper disable StyleCop.SA1126 - invalid warning
////namespace Serpent.MessageBus.MessageHandlerChain.Decorators.HandleAndChain
////{
////    using System;
////    using System.Threading;
////    using System.Threading.Tasks;

////    using Serpent.MessageBus.Interfaces;
////    using Serpent.MessageBus.MessageHandlerChain.Decorators.WeakReference;

////    internal class HandleAndChainDecorator<TMessageType, TNewMessageType> : MessageHandlerChainDecorator<TMessageType>
////    {
////        private readonly WeakReference<Func<TMessageType, CancellationToken, Task>> handlerFunc;

////        private IMessageBusSubscription subscription;

////        public HandleAndChainDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, IMessageHandlerChainSubscriptionNotification subscriptionNotification, MessageHandlerChainBuilder<TNewMessageType> chainedMessageHandlerBuilder)
////        {
////            this.handlerFunc = new WeakReference<Func<TMessageType, CancellationToken, Task>>(handlerFunc);
////            subscriptionNotification.AddNotification(sub => this.subscription = sub);
////        }

////        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
////        {
////            if (this.handlerFunc.TryGetTarget(out var target))
////            {
////                return target(message, token);
////            }

////            if (this.subscription != null)
////            {
////                this.subscription.Dispose();
////                this.subscription = null;
////            }

////            return Task.CompletedTask;
////        }

////        /// <inheritdoc />
////        public bool DisposeSubscriptionIfReclamiedByGarbageCollection()
////        {
////            if (this.handlerFunc.TryGetTarget(out var _))
////            {
////                return false;
////            }

////            if (this.subscription != null)
////            {
////                this.subscription.Dispose();
////                this.subscription = null;
////            }

////            return true;
////        }
////    }
////}