namespace Serpent.Common.MessageBus.MessageHandlerChain.WireUp
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionHelpers
    {
        public static Delegate CreateGetter<TMessageType>(PropertyInfo property)
        {
            var messageType = typeof(TMessageType);

            var parameter = Expression.Parameter(messageType, "m");

            var getterExpression = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(messageType, property.PropertyType),
                Expression.MakeMemberAccess(parameter, property),
                parameter);

            return getterExpression.Compile();
        }

        public static Delegate CreateGetter<TMessageType>(string propertyName)
        {
            var messageType = typeof(TMessageType);

            var property = messageType.GetProperties().FirstOrDefault(p => p.Name == propertyName);
            if (property == null)
            {
                throw new Exception($"Property {propertyName} not found");
            }

            return CreateGetter<TMessageType>(property);
        }
    }
}