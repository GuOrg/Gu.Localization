// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="">
//   
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// General Information about an assembly is controlled through the following 

// set of attributes. Change these attribute values to modify the information

// associated with an assembly.

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("Gu.Wpf.Localization")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Johan Larsson")]
[assembly: AssemblyProduct("Gu.Wpf.Localization.Properties")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8b5013e4-ebfd-48b4-b6b4-34d6df4fb982")]

// Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
[assembly: AssemblyVersion("1.4.0.0")]
[assembly: AssemblyFileVersion("1.4.0.0")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: InternalsVisibleTo("Gu.Wpf.Localization.Tests", AllInternalsVisible = true)]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, // where theme specific resource dictionaries are located
                                     // (used if a resource is not found in the page, 
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly // where the generic resource dictionary is located
                                              // (used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]
[assembly: XmlnsDefinition("http://gu.se/Localization", "Gu.Localization", AssemblyName = "Gu.Localization")]
[assembly: XmlnsDefinition("http://gu.se/Localization", "Gu.Wpf.Localization", AssemblyName = "Gu.Wpf.Localization")]
[assembly: XmlnsPrefix("http://gu.se/Localization", "localization")]