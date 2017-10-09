namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionHelpers
    {
        public static Delegate CreateGetter<TType>(PropertyInfo property)
        {
            var messageType = typeof(TType);

            var parameter = Expression.Parameter(messageType, "m");

            var getterExpression = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(messageType, property.PropertyType),
                Expression.MakeMemberAccess(parameter, property),
                parameter);

            return getterExpression.Compile();
        }

        public static Delegate CreateGetter<TType>(string propertyName)
        {
            return CreateGetter<TType>(GetPropertyInfo<TType>(propertyName));
        }

        public static PropertyInfo GetPropertyInfo<TType>(string propertyName)
        {
            var messageType = typeof(TType);

            var property = messageType.GetProperties().FirstOrDefault(p => p.Name == propertyName);
            if (property == null)
            {
                throw new Exception($"Property {propertyName} not found");
            }

            return property;
        }
    }
}