namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    internal static class SelectorSetup<TMessageType, TIdentifier>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ConcurrentDictionary<string, Container> Getters = new ConcurrentDictionary<string, Container>();

        public static void WireUp(string propertyName, Func<MethodInfo> methodInfoGenerator, Action<MethodInfo, Delegate> invoker)
        {
            var container = Getters.GetOrAdd(
                propertyName,
                pn =>
                    {
                        var propertyInfo = ExpressionHelpers.GetPropertyInfo<TMessageType>(propertyName);
                        var selector = ExpressionHelpers.CreateGetter<TMessageType>(propertyInfo);
                        var methodInfo = methodInfoGenerator();
                        var method = methodInfo.MakeGenericMethod(typeof(TMessageType), propertyInfo.PropertyType);

                        return new Container
                                   {
                                       Method = method,
                                       Selector = selector
                                   };
                    });

            invoker(container.Method, container.Selector);
        }

        private struct Container
        {
            public Delegate Selector { get; set; }

            public MethodInfo Method { get; set; }
        }
    }
}