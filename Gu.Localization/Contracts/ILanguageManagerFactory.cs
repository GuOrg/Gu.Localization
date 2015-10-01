namespace Gu.Localization
{
    using System;
    using System.Reflection;

    public interface ILanguageManagerFactory
    {
        ILanguageManager GetOrCreate(Type typeInAssembly);

        ILanguageManager GetOrCreate(Assembly assembly);
    }
}