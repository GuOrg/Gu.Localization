namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Reflection;
    using System.Security;

    internal class ResourceExtensionConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        [SecurityCritical]
        [SecurityTreatAsSafe]
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(destinationType == typeof(InstanceDescriptor)))
                return base.ConvertTo(context, culture, value, destinationType);
            var resourceExtension = value as StaticExtension;
            if (resourceExtension == null)
                throw new ArgumentException("MustBeOfType: " + typeof(StaticExtension).Name);
            else
                return (object)new InstanceDescriptor(
                    (MemberInfo)typeof(StaticExtension).GetConstructor(new Type[1]{typeof (string)}), 
                    (ICollection)new object[1]{(object) resourceExtension.Member});
        }
    }
}