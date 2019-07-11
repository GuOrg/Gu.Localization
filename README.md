# Gu.Localization.

[![Join the chat at https://gitter.im/JohanLarsson/Gu.Localization](https://badges.gitter.im/JohanLarsson/Gu.Localization.svg)](https://gitter.im/JohanLarsson/Gu.Localization?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![Build status](https://ci.appveyor.com/api/projects/status/ili1qk8amyjmd71t/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-localization/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Gu.Localization.svg)](https://www.nuget.org/packages/Gu.Localization/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Wpf.Localization.svg)](https://www.nuget.org/packages/Gu.Wpf.Localization/)


# Contents.
- [Quickstart](#quickstart)
- [Usage in XAML.](#usage-in-xaml)
  - [Simple example](#simple-example)
  - [Bind a localized string.](#bind-a-localized-string)
  - [Errorhandling.](#errorhandling)
  - [CurrentCulture.](#currentculture)
  - [Binding to Culture and Culture in XAML.](#binding-to-culture-and-culture-in-xaml)
- [Usage in code.](#usage-in-code)
  - [Translator.](#translator)
    - [Culture.](#culture)
    - [Culture.](#culture)
    - [CurrentCulture.](#currentculture)
    - [Cultures.](#cultures)
    - [ErrorHandling.](#errorhandling)
    - [Translate.](#translate)
      - [Translate to neutral culture:](#translate-to-neutral-culture)
      - [Translate to explicit culture:](#translate-to-explicit-culture)
      - [Override global error handling (throw on error):](#override-global-error-handling-throw-on-error)
      - [Override global error handling (return info about error):](#override-global-error-handling-return-info-about-error)
      - [Translate with parameter:](#translate-with-parameter)
  - [Translator&lt;T&gt;.](#translatort)
  - [Translation.](#translation)
  - [GetOrCreate.](#getorcreate)
  - [StaticTranslation.](#statictranslation)
- [ErrorHandling.](#errorhandling)
  - [Global setting](#global-setting)
  - [ErrorFormats](#errorformats)
- [Validate.](#validate)
  - [Translations.](#translations)
  - [EnumTranslations&lt;T&gt;.](#enumtranslationst)
  - [TranslationErrors](#translationerrors)
  - [Format](#format)
- [FormatString.](#formatstring)
  - [IsFormatString](#isformatstring)
  - [IsValidFormatString](#isvalidformatstring)
- [LanguageSelector](#languageselector)
  - [AutogenerateLanguages](#autogeneratelanguages)
  - [Explicit languages.](#explicit-languages)
- [Examples](#examples)
  - [Simple ComboBox language select.](#simple-combobox-language-select)
  - [ComboBox Language selector](#combobox-language-selector)
  - [CultureToFlagPathConverter](#culturetoflagpathconverter)
- [Embedded resource files (weaving)](#embedded-resource-files-weaving)
  - [Weaving Setup](#weaving-setup)
- [Analyzer](#analyzer)


# Quickstart
1. PM> Install-Package Gu.Wpf.Localization
2. Add a resx resource. (Project > Properties > Resources)
3. Create resx files for some languages. In this example I used `en-US` and `sv`
3. Use the markupextension like this:

```xaml
<Window
    x:Class="WpfApp1.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:l="http://gu.se/Localization"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:properties="clr-namespace:WpfApp1.Properties"
    Title="MainWindow"
    mc:Ignorable="d">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>
        <l:LanguageSelector AutogenerateLanguages="True" />
        <TextBlock Grid.Column="1" Text="{l:Static properties:Resources.Some_text}" />
    </Grid>
</Window>
```

![Localize](https://user-images.githubusercontent.com/1640096/61053560-1288a600-a3ee-11e9-9939-930dfa4d911f.gif)

NOTE: It ses `l:Static` where `xmlns:l="http://gu.se/Localization"`

NOTE: `xmlns:properties="clr-namespace:YourApp.Properties"`

For working with resx in Visual Studio [ResXManager](https://marketplace.visualstudio.com/items?itemName=TomEnglert.ResXManager) is a nice extension.

# Usage in XAML.

The library has a `StaticExtension` markupextension that is used when translating.
The reason for naming it `StaticExtension` and not `TranslateExtension` is that Resharper provides intellisense when named `StaticExtension`
Binding the text like below updates the text when `Translator.CurrentCulture`changes enabling runtime selection of language.

The markupextension has ErrorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral as default, it encodes errors in the result, see [ErrorFormats](#3-errorhandling))

## Simple example
For each language, create a resource.xx.resx file. You can use [ResXManager](https://marketplace.visualstudio.com/items?itemName=TomEnglert.ResXManager#overview) to do this for you.

```xaml
<UserControl ...
             xmlns:l="clr-namespace:Gu.Wpf.Localization;assembly=Gu.Wpf.Localization"
             xmlns:p="clr-namespace:AppNamespace.Properties"
             xmlns:local="clr-namespace:YourNamespace;assembly=Gu.Localization">
    ...
    <!-- Dropbownbox to select a language -->
    <ComboBox x:Name="LanguageComboBox"
              ItemsSource="{Binding Path=(localization:Translator.Cultures)}"
              SelectedItem="{Binding Path=(localization:Translator.Culture),
                                              Converter={x:Static l:CultureOrDefaultConverter.Default}}" />

    <!-- Label that changes translation upon language selection -->
    <Label Content="{l:Static p:Resources.ResourceKeyName}" />
```

## Bind a localized string.

```xaml
<Window ...
        xmlns:p="clr-namespace:Gu.Wpf.Localization.Demo.WithResources.Properties"
        xmlns:l="http://gu.se/Localization">
    ...
    <TextBlock Text="{l:Static p:Resources.SomeResource}" />
    <TextBlock Text="{l:Enum ResourceManager={x:Static p:Resources.ResourceManager},
                             Member={x:Static local:SomeEnum.SomeMember}}" />
    ...
```

The above will show SomeResource in the `Translator.CurrentCulture` and update when culture changes.

## Errorhandling.
By setting the attached property `ErrorHandling.Mode` we override how translation errors are handled by the `StaticExtension` for the child elements.
When null the `StaticExtension` uses ReturnErrorInfoPreserveNeutral
```xaml
<Grid l:ErrorHandling.Mode="ReturnErrorInfo"
     ...   >
    ...
    <TextBlock Text="{l:Static p:Resources.SomeResource}" />
    <TextBlock Text="{l:Enum ResourceManager={x:Static p:Resources.ResourceManager},
                             Member={x:Static local:SomeEnum.SomeMember}}" />
    ...
```

## CurrentCulture.
A markupextension for accessing `Translator.CurrentCulture` from xaml. Retruns a binding that updates when CurrentCulture changes.

```xaml
<Grid numeric:NumericBox.Culture="{l:CurrentCulture}"
     ...   >
    ...
        <StackPanel Orientation="Horizontal">
            <TextBlock Text="Effective culture: " />
            <TextBlock Text="{l:CurrentCulture}" />
        </StackPanel>
    ...
```

## Binding to Culture and Culture in XAML.
The static properties support binding. Use this XAML for a twoway binding:
```xaml
<Window ...
        xmlns:localization="clr-namespace:Gu.Localization;assembly=Gu.Localization">
    ...
<TextBox Text="{Binding Path=(localization:Translator.Culture)}" />
```

# Usage in code.

The API is not super clean, introducing a helper like this can clean things up a bit.

Creating it like the above is pretty verbose. Introducing a helper like below can clean it up some.
The analyzer checks calls to this method but it assumes:
1. That the class is named `Translate`
2. That the namespace the class is in has a class named `Resources`
3. That the first argument is of type `string`.
4. That the return type is `string` or `ITranslation`

```c#
namespace YourNamespace.Properties
{
    using Gu.Localization;
    using Gu.Localization.Properties;

    public static class Translate
    {
        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name="key">A key in Properties.Resources</param>
        /// <param name="errorHandling">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static string Key(string key, ErrorHandling errorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral)
        {
            return TranslationFor(key, errorHandling).Translated;
        }

        /// <summary>Call like this: Translate.Key(nameof(Resources.Saved_file__0_)).</summary>
        /// <param name="key">A key in Properties.Resources</param>
        /// <param name="errorHandling">How to handle translation errors like missing key or culture.</param>
        /// <returns>A translation for the key.</returns>
        public static ITranslation TranslationFor(string key, ErrorHandling errorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral)
        {
            return Gu.Localization.Translation.GetOrCreate(Resources.ResourceManager, key, errorHandling);
        }
    }
}
```

## Translator.

### Culture.
Get or set the current culture. The default is `null`
Changing culture updates all translations. Setting culture to a culture for which there is no translation throws. Check ContainsCulture() first.

### Culture.
Get or set the current culture. The default is `null`
Changing culture updates all translations. Setting culture to a culture for which there is no translation throws. Check ContainsCulture() first.

### CurrentCulture.
Get the culture used in translations. By the following mechanism:
  1) CurrentCulture if not null.
  2) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
  3) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
  4) CultureInfo.InvariantCulture
When this value changes CurrentCultureChanged is raised and all translatins updates and notifies.

### Cultures.
Get a list with the available cultures. Cultures are found by looking in current directory and scanning for satellite assemblies.

### ErrorHandling.
Get or set how errors are handled. The default value is `ReturnErrorInfoPreserveNeutral`.

### Translate.
Translate a key in a ResourceManager.

Use global culture & error handling:
```c#
Translator.Culture = CultureInfo.GetCultureInfo("en"); // no need to set this every time, just for illustration purposes here.
string inEnglish = Translator.Translate(Properties.Resources.ResourceManager,
                                        nameof(Properties.Resources.SomeResource));
```

#### Translate to neutral culture:
```c#
string neutral = Translator.Translate(Properties.Resources.ResourceManager,
                                      nameof(Properties.Resources.SomeResource),
                                      CultureInfo.InvariantCulture);
```

#### Translate to explicit culture:
```c#
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager,
                                        nameof(Properties.Resources.SomeResource),
                                        CultureInfo.GetCultureInfo("sv"));
```

#### Override global error handling (throw on error):
```c#
Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo; // no need to set this every time, just for illustration purposes here.
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager,
                                        nameof(Properties.Resources.SomeResource),
                                        ErrorHandling.Throw);
```

#### Override global error handling (return info about error):
```c#
Translator.ErrorHandling = ErrorHandling.Throw; // no need to set this every time, just for illustration purposes here.
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager,
                                        nameof(Properties.Resources.SomeResource),
                                        ErrorHandling.ReturnErrorInfo);
```

#### Translate with parameter:
```c#
Translator.Culture = CultureInfo.GetCultureInfo("en");
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager,
                                        nameof(Properties.Resources.SomeResource__0__),
                                        foo);
```

## Translator&lt;T&gt;.

Same as translator but used like `Translator<Properties.Resources>.Translate(...)`

## Translation.
An object with a Translated property that is a string with the value in `Translator.CurrentCulture`
Implements `INotifyPropertyChanged` and notifies when for the property `Translated` if a change in `Translator.CurrentCulture` updates the translation.

## GetOrCreate.
Returns an `ITranslation` from cache or creates and caches a new instance.
If ErrorHandling is Throw it throws if the key is missing. If other than throw a `StaticTranslation` is returned.

```c#
Translation translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeResource))
```

## StaticTranslation.
An implementation of `ITranslation` that never updates the `Translated`property and returns the value of `Translated` when calling `Translate()`on it with any paramaters.
This is returned from `Translation.GetOrCreate(...)` if the key is missing.

# ErrorHandling.
When calling the translate methods an ErrorHandling argument can be provided.
If `ErrorHandling.ReturnErrorInfo` is passed in the method does not throw but returns information about the error in the string.
There is also a property `Translator.ErrorHandling` that sets default behaviour. If an explicit errorhandling is passed in to a method it overrides the global setting.

## Global setting
By setting `Translator.Errorhandling` the global default is changed.

## ErrorFormats
When `ReturnErrorInfo` or `ReturnErrorInfoPreserveNeutral` is used the following formats are used to encode errors.

| Error               |         Format          |
|---------------------|:-----------------------:|
| missing key         |        `!{key}!`        |
| missing culture     |        `~{key}~`        |
| missing translation |        `_{key}_`        |
| missing resources   |        `?{key}?`        |
| invalid format      |`{{"{format}" : {args}}}`|
| unknown error       |    `#{key}#`            |


# Validate.
Conveience API for unit testing localization.

## Translations.

Validate a `ResourceManager` like this:
```c#
TranslationErrors errors = Validate.Translations(Properties.Resources.ResourceManager);
Assert.IsTrue(errors.IsEmpty);
```

Checks:
- That all keys has a non null value for all cultures in `Translator.AllCultures`
- If the resource is a format string like `"First: {0}, second{1}"` it checks that.
  - The number of format items are the same for all cultures.
  - That all format strings has format items numbered 0..1..n

## EnumTranslations&lt;T&gt;.
Validate an `enum` like this:
```c#
TranslationErrors errors = Validate.EnumTranslations<DummyEnum>(Properties.Resources.ResourceManager);
Assert.IsTrue(errors.IsEmpty);
```
Checks:
- That all enum members has keys in the `ResourceManager`
- That all keys has non null value for all cultures in `Translator.AllCultures`

## TranslationErrors
`errors.ToString("  ", Environment.NewLine);`
Prints a formatted report with the errors found, sample:

```
Key: EnglishOnly
  Missing for: { de, sv }
Key: Value___0_
  Has format errors, the formats are:
    Value: {0}
    null
    Värde: {0} {1}
```
## Format
Validate a formatstring like this:
```c#
Validate.Format("Value: {0}", 1);
```

```c#
Debug.Assert(Validate.IsValidFormat("Value: {0}", 1), "Invalid format...");
```

# FormatString.
Conveience API for testing formatstrings.

## IsFormatString
Returns true if the string contains placeholders like `"Value: {0}"` and is a valid format string.

## IsValidFormatString
Returns true if the string contains placeholders like `"Value: {0}"` that matches the number of parameters and is a valid format string.

# LanguageSelector
A simple control for changing current language.
A few flags are included in the library, many are probably missing.

***Note: LanguageSelector might be depricated in the future***

## AutogenerateLanguages
Default is false.
If true it popolates itself with `Translator.Cultures` in the running application and picks the default flag or null.

```xaml
<l:LanguageSelector AutogenerateLanguages="True" />
```

## Explicit languages.

```xaml
<l:LanguageSelector>
    <l:Language Culture="de-DE"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/de.png" />
    <l:Language Culture="en-GB"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/gb.png" />
    <l:Language Culture="sv-SE"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/se.png" />
</l:LanguageSelector>
```

![screenie](http://i.imgur.com/DKfx8WB.png)

# Examples

## Simple ComboBox language select.
The below example binds the available cutures to a ComboBox.
```xaml
        <ComboBox ItemsSource="{Binding Path=(localization:Translator.Cultures)}" DockPanel.Dock="Top" HorizontalAlignment="right"
          SelectedItem="{Binding Path=(localization:Translator.CurrentCulture)}"/>
```

## ComboBox Language selector
```xaml
<Window ...
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:globalization="clr-namespace:System.Globalization;assembly=mscorlib"
        xmlns:l="http://gu.se/Localization"
        xmlns:localization="clr-namespace:Gu.Localization;assembly=Gu.Localization">
    <Grid>
        <ComboBox MinWidth="100"
                  HorizontalAlignment="Right"
                  VerticalAlignment="Top"
                  ItemsSource="{Binding Path=(localization:Translator.Cultures)}"
                  SelectedItem="{Binding Path=(localization:Translator.Culture)}">
            <ComboBox.ItemTemplate>
                <DataTemplate DataType="{x:Type globalization:CultureInfo}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>

                        <Image Grid.Column="0"
                               Height="12"
                               VerticalAlignment="Center"
                               Source="{Binding Converter={x:Static l:CultureToFlagPathConverter.Default}}"
                               Stretch="Fill" />

                        <TextBlock Grid.Column="1"
                                   Margin="10,0,0,0"
                                   HorizontalAlignment="Left"
                                   VerticalAlignment="Center"
                                   Text="{Binding NativeName}" />
                    </Grid>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>

        ...
    </Grid>
</Window>
```

## CultureToFlagPathConverter

For convenience a converter that converts from `CultureInfo` to a string with the pack uri of the flag resource is included.

# Embedded resource files (weaving)

_"Weaving refers to the process of injecting functionality into an existing program."_

You might want to publish your software as just one .exe file, without additional assemblies (dll files). Gu.Localization supports this, and a sample project is added [here](https://github.com/GuOrg/Gu.Localization/tree/master/Gu.Wpf.Localization.Demo.Fody). We advice you to use Fody (for it is tested).

## Weaving Setup

- Install https://www.nuget.org/packages/Fody/ (and add FodyWeavers.xml to your project, see [here](https://github.com/Fody/Fody#add-fodyweaversxml))
- Install https://www.nuget.org/packages/Costura.Fody/
- Install https://www.nuget.org/packages/Resource.Embedder/ to include the satelite assemblies _(in the folders /sv-SE/, /nl-NL/, etc)_

Your resource files are now embeded in your executable. Gu.Localization will use the embedded resource files.

# Analyzer
![animation](https://user-images.githubusercontent.com/1640096/39090329-8115bd4a-45dc-11e8-8cc5-a4af4a2f5812.gif)

Checks if keys exists and some code fixes for conveninence.
