namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq.Expressions;

    public interface ITranslator
    {
        CultureInfo CurrentCulture { get; set; }

        IReadOnlyList<CultureInfo> AllCultures { get; }

        string Translate(Expression<Func<string>> key);

        string Translate(Type typeInAsembly, string key);
    }
}