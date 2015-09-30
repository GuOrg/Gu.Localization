//namespace Gu.Wpf.Localization
//{
//    using System;
//    using System.Collections;
//    using System.ComponentModel;
//    using System.ComponentModel.Design.Serialization;
//    using System.Diagnostics;
//    using System.Globalization;
//    using System.Reflection;
//    using System.Security;

//    internal class StaticExtensionConverter : TypeConverter
//    {
//        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
//        {
//            Debugger.Break();
//            return base.CanConvertFrom(context, sourceType);
//        }

//        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
//        {
//            Debugger.Break();
//            return base.ConvertFrom(context, culture, value);
//        }

//        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
//        {
//            Debugger.Break();
//            return base.CreateInstance(context, propertyValues);
//        }

//        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
//        {
//            Debugger.Break();
//            return base.GetCreateInstanceSupported(context);
//        }

//        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
//        {
//            Debugger.Break();
//            return base.GetProperties(context, value, attributes);
//        }

//        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
//        {
//            Debugger.Break();
//            return base.GetPropertiesSupported(context);
//        }

//        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
//        {
//            Debugger.Break();
//            return base.GetStandardValues(context);
//        }

//        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
//        {
//            Debugger.Break();
//            return base.GetStandardValuesExclusive(context);
//        }

//        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
//        {
//            Debugger.Break();
//            return base.GetStandardValuesSupported(context);
//        }

//        public override bool IsValid(ITypeDescriptorContext context, object value)
//        {
//            Debugger.Break();
//            return base.IsValid(context, value);
//        }

//        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
//        {
//            Debugger.Break();
//            if (destinationType == typeof(InstanceDescriptor))
//            {
//                return true;
//            }
//            else
//            {
//                return base.CanConvertTo(context, destinationType);
//            }
//        }

//        [SecurityCritical]
//        [SecurityTreatAsSafe]
//        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
//        {
//            Debugger.Break();
//            if (!(destinationType == typeof(InstanceDescriptor)))
//            {
//                return base.ConvertTo(context, culture, value, destinationType);
//            }
//            var resourceExtension = value as StaticExtension;
//            if (resourceExtension == null)
//            {
//                throw new ArgumentException("MustBeOfType: " + typeof(StaticExtension).Name);
//            }
//            else
//            {
//                return (object)new InstanceDescriptor(
//                    (MemberInfo)typeof(StaticExtension).GetConstructor(new[] { typeof(string) }),
//                    (ICollection)new[] { (object)resourceExtension.Member });
//            }
//        }
//    }
//}