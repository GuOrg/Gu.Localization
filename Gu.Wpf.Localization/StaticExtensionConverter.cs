namespace Gu.Wpf.Localization
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Security;

    /// <inheritdoc />
    internal class StaticExtensionConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        [SecurityCritical]
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(destinationType == typeof(InstanceDescriptor)))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            var staticExtension = value as StaticExtension;
            if (staticExtension == null)
            {
                throw new ArgumentException("MustBeOfType: " + typeof(StaticExtension).Name);
            }

            if (staticExtension.ResourceManager != null)
            {
                Debugger.Break();
                var constructorInfo = typeof(StaticExtension).GetConstructor(new[] { typeof(string), typeof(ResourceManager) });
                return new InstanceDescriptor(constructorInfo, new[] { (object)staticExtension.Member, staticExtension.ResourceManager });
            }
            else
            {
                Debugger.Break();
                var constructorInfo = typeof(StaticExtension).GetConstructor(new[] { typeof(string) });
                return new InstanceDescriptor(constructorInfo, new[] { (object)staticExtension.Member });
            }
        }
    }
}