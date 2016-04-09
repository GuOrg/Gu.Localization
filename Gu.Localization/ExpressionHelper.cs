namespace Gu.Localization
{
    using System;
    using System.Linq.Expressions;
    using System.Resources;

    internal static class ExpressionHelper
    {
        internal static bool IsResourceKey(Expression<Func<string>> key)
        {
            return GetResourceManager(key) != null;
        }

        internal static ResourceManager GetResourceManager(Expression<Func<string>> key)
        {
            var memberExpression = key?.Body as MemberExpression;
            if (memberExpression == null)
            {
                return null;
            }
            var declaringType = memberExpression.Member.DeclaringType;
            return FromType(declaringType);
        }

        internal static string GetResourceKey(Expression<Func<string>> key)
        {
            if (!IsResourceKey(key))
            {
                return null;
            }

            var memberExpression = key?.Body as MemberExpression;
            return memberExpression?.Member.Name;
        }

        internal static string PropertyName<T>(Expression<Func<T>> prop)
        {
            var memberExpression = prop.Body as MemberExpression;
            return memberExpression?.Member.Name;
        }

        private static ResourceManager FromType(Type type)
        {
            return ResourceManagerWrapper.FromType(type);
        }
    }
}
