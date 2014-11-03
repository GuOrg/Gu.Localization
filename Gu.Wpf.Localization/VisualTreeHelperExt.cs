namespace Gu.Wpf.Localization
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public static class VisualTreeHelperExt
    {
        public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                yield break;
            }
            yield return dependencyObject;
            while (dependencyObject != null)
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
                if (dependencyObject != null)
                {
                    yield return dependencyObject;
                }
            }
        }
    }
}
