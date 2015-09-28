namespace Gu.Localization
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Resources;

    internal static class ExpressionHelper
    {
        internal static bool IsResourceKey(Expression<Func<string>> key)
        {
            if (key == null)
            {
                return false;
            }

            var memberExpression = key.Body as MemberExpression;
            if (memberExpression == null)
            {
                return false;
            }

            if (memberExpression.Type != typeof(string))
            {
                return false;
            }

            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                return false;
            }

            if (memberExpression.Expression != null)
            {
                return false;
            }

            var declaringType = memberExpression.Member.DeclaringType;
            return ResourceManagerWrapper.HasResourceManager(declaringType);
        }

        internal static ResourceManager GetResourceManager(Expression<Func<string>> key)
        {
            if (key == null)
            {
                return null;
            }

            var memberExpression = key.Body as MemberExpression;
            if (memberExpression == null)
            {
                return null;
            }

            var declaringType = memberExpression.Member.DeclaringType;
            return FromType(declaringType);
        }

        internal static string GetResourceKey(Expression<Func<string>> key)
        {
            if (key == null)
            {
                return null;
            }

            if (!IsResourceKey(key))
            {
                return null;
            }

            var memberExpression = key.Body as MemberExpression;
            if (memberExpression == null)
            {
                return null;
            }
            return memberExpression.Member.Name;
        }

        private static ResourceManager FromType(Type type)
        {
            return ResourceManagerWrapper.FromType(type);
        }
    }
}
