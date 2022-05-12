using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("Gu.Wpf.Localization.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001008F3FADDAC2AF5D3A713943D1661FF19562CB93322F985E0A9EE5EA8FBA5D2DE865E27EF3F90B85AE33EABE89449BA2EF391046E9E55080F4E895AA51C298A810445F8C2E00A3EDE5AD9750801E0120421F2747BEDC112D082001C10F3255E33CC7DC4D0263792B195CFCF935C5D723EB85A4906C4B9C5B9D1FC980DE1594FE98", AllInternalsVisible = true)]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]
[assembly: XmlnsDefinition("http://gu.se/Localization", "Gu.Wpf.Localization")]
[assembly: XmlnsPrefix("http://gu.se/Localization", "l")]

#if NET48
#pragma warning disable SA1402, SA1502, SA1600, SA1649, GU0073
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    internal sealed class AllowNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    internal sealed class DisallowNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class MaybeNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class NotNullAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue) => this.ReturnValue = returnValue;

        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => this.ReturnValue = returnValue;

        public bool ReturnValue { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName) => this.ParameterName = parameterName;

        public string ParameterName { get; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class DoesNotReturnAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class DoesNotReturnIfAttribute : Attribute
    {
        public DoesNotReturnIfAttribute(bool parameterValue) => this.ParameterValue = parameterValue;

        public bool ParameterValue { get; }
    }
}
#endif
