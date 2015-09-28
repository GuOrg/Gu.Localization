namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Resources;

    public interface ITranslator
    {
        CultureInfo CurrentCulture { get; set; }

        IReadOnlyList<CultureInfo> AllCultures { get; }

        bool HasCulture(CultureInfo culture);

        bool HasKey(string key);

        string Translate(string key);

        string Translate(Expression<Func<string>> key);

        string Translate(ResourceManager resourceManager, Expression<Func<string>> key);
    }
}