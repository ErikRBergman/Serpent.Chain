//// ReSharper disable once CheckNamespace

//namespace Serpent.Common.MessageBus
//{
//    using System;
//    using System.Threading.Tasks;

//    using Serpent.Common.MessageBus.Helpers;
//    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.HandleAndChain;

//    /// <summary>
//    ///     The delay decorator extensions
//    /// </summary>
//    public static class HandleAndChainExtensions
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="TMessageType">The message type</typeparam>
//        /// <param name="messageHandlerChainBuilder">The builder</param>
//        /// <returns>A new message handler chain builder used to describe the chain that handles the message returned from the handler</returns>
//        public static IMessageHandlerChainBuilder<TNewMessageType> HandleAndChain<TMessageType, TNewMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, Task<TNewMessageType>> handler)
//        {
//            var builder = new MessageHandlerChainBuilder<TNewMessageType>();

//            // (message => new HandleAndChainDecorator<TMessageType, TNewMessageType>(currentHandler, services.SubscriptionNotification, builder)

//            var subscription = messageHandlerChainBuilder.Handler(
//                async message =>
//                    {
                        
//                    });

//            return builder;
//        }


//        public static IMessageHandlerChainBuilder<TNewMessageType> HandleAndChain<TMessageType, TNewMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, TNewMessageType> handler)
//        {
//            return messageHandlerChainBuilder.HandleAndChain(msg => Task.FromResult(handler(msg)));
//        }
//    }
//}