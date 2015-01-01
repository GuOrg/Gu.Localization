// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticExtensionConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The static extension converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Wpf.Localization
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Reflection;
    using System.Security;

    /// <summary>
    /// The static extension converter.
    /// </summary>
    internal class StaticExtensionConverter : TypeConverter
    {
        /// <summary>
        /// The can convert to.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// The convert to.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="destinationType">
        /// The destination type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
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
                    (MemberInfo)typeof(StaticExtension).GetConstructor(new[]{typeof (string)}), 
                    (ICollection)new[]{(object) resourceExtension.Member});
        }
    }
}