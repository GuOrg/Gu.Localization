namespace Gu.Localization
{
    using System.Globalization;
    using System.Resources;

    public class ResourceSetAndCulture
    {
        public ResourceSetAndCulture(ResourceSet resourceSet, CultureInfo culture)
        {
            this.ResourceSet = resourceSet;
            this.Culture = culture;
        }
        public ResourceSet ResourceSet { get; private set; }
        public CultureInfo Culture { get; private set; }
    }
}
