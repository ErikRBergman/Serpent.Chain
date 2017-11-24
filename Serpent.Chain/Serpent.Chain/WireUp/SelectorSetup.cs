namespace Serpent.Chain.WireUp
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    internal static class SelectorSetup<TMessageType>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ConcurrentDictionary<string, Container> Getters = new ConcurrentDictionary<string, Container>();

        public static void WireUp(string propertyName, MethodInfo genericMethodInfo, Action<MethodInfo, Delegate> invokeDecoratorAction)
        {
            if (invokeDecoratorAction == null)
            {
                throw new ArgumentNullException(nameof(invokeDecoratorAction));
            }

            var container = Getters.GetOrAdd(
                propertyName,
                pn =>
                    {
                        var propertyInfo = ExpressionHelpers.GetPropertyInfo<TMessageType>(propertyName);
                        var selector = ExpressionHelpers.CreateGetter<TMessageType>(propertyInfo);
                        var method = genericMethodInfo.MakeGenericMethod(typeof(TMessageType), propertyInfo.PropertyType);

                        return new Container
                                   {
                                       Method = method,
                                       Selector = selector
                                   };
                    });

            invokeDecoratorAction(container.Method, container.Selector);
        }

        private struct Container
        {
            public Delegate Selector { get; set; }

            public MethodInfo Method { get; set; }
        }
    }
}