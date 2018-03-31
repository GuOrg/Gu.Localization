Flags from: https://github.com/lipis/flag-icon-css (Converted to png)

[Test]
public void DumpCultures()
{
    var cultureInfos = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures)
                                  .OrderBy(x => x.DisplayName)
                                  .Select(x => x.TwoLetterISOLanguageName)
                                  .Distinct()
                                  .ToArray();
    foreach (var name in cultureInfos)
    {
        try
        {
            var cultureInfo = CultureInfo.GetCultureInfo(name);
            Console.WriteLine($"{cultureInfo.DisplayName}, {cultureInfo.TwoLetterISOLanguageName}");
        }
        catch (Exception)
        {
            //Console.WriteLine();
        }
    }
}