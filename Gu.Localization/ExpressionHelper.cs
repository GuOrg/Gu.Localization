namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Resources;

    internal static class ExpressionHelper
    {
        private static ConcurrentDictionary<Type, ResourceManager> _typeResourceManagerMap = new ConcurrentDictionary<Type, ResourceManager>();
        public static bool IsResourceKey(Expression<Func<string>> key)
        {
            return GetResourceManager(key) != null;
        }

        public static ResourceManager GetResourceManager(Expression<Func<string>> key)
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

        public static string GetResourceKey(Expression<Func<string>> key)
        {
            if (!IsResourceKey(key))
            {
                return null;
            }
            if (key == null)
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
            ResourceManager manager;
            if (!_typeResourceManagerMap.TryGetValue(type, out manager))
            {
                var propertyInfo = type.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (propertyInfo == null)
                {
                    return null;
                }
                manager = propertyInfo.GetValue(null) as ResourceManager;
                if (manager != null)
                {
                    _typeResourceManagerMap.TryAdd(type, manager);
                }
            }
            return manager;
        }
    }
}
