namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public interface ILanguageManager : IDisposable
    {
        IReadOnlyList<CultureInfo> Languages { get; }

        string Translate(CultureInfo culture, string key);
    }
}