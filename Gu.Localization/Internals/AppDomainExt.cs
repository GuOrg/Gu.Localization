namespace Gu.Localization
{
    using System;
    using System.Linq;

    public static class AppDomainExt
    {
        public static bool IsDebug(this AppDomain domain)
        {
            if (domain.IsDesignTime())
            {
                return false;
            }
            return domain.GetAssemblies()
                         .Any(x => x.ManifestModule.Name.EndsWith(".vshost.exe"));
        }

        public static bool IsDesignTime(this AppDomain domain)
        {
            return domain.GetAssemblies()
                         .Any(x => x.ManifestModule.Name.EndsWith("XDesProc.exe"));
        }
    }
}
