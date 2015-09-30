namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
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
            var match = GetResourceManagerProperty(declaringType);
            return match != null;
        }

        internal static PropertyInfo GetResourceManagerProperty(Type type)
        {
            var match = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                               .SingleOrDefault(x => (typeof(ResourceManager).IsAssignableFrom(x.PropertyType)));
            return match;
        }

        internal static ResourceManager GetResourceManager(Expression<Func<string>> key)
        {
            var type = GetRootType(key);
            var match = GetResourceManagerProperty(type);
            if (match == null)
            {
                return null;
            }
            return (ResourceManager)match.GetValue(null);
        }

        internal static Type GetRootType(Expression<Func<string>> key)
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
            return declaringType;
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
    }
}
