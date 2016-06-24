# Gu.Localization.

[![Join the chat at https://gitter.im/JohanLarsson/Gu.Localization](https://badges.gitter.im/JohanLarsson/Gu.Localization.svg)](https://gitter.im/JohanLarsson/Gu.Localization?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![Build status](https://ci.appveyor.com/api/projects/status/ili1qk8amyjmd71t?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-localization)
[![NuGet](https://img.shields.io/nuget/v/Gu.Localization.svg)](https://www.nuget.org/packages/Gu.Localization/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Wpf.Localization.svg)](https://www.nuget.org/packages/Gu.Wpf.Localization/)


# Contents.
- [1. Usage in XAML.](#1-usage-in-xaml)
  - [1.1. Bind a localized string.](#11-bind-a-localized-string)
  - [1.2. Errorhandling.](#12-errorhandling)
  - [1.3. CurrentCulture.](#13-currentculture)
  - [1.4. Binding to CurrentCulture and CurrentCulture in XAML.](#14-binding-to-currentculture-and-currentculture-in-xaml)
- [2. Usage in code.](#2-usage-in-code)
  - [2.1. Translator.](#21-translator)
    - [2.1.1. Culture.](#211-culture)
    - [2.1.2. Culture.](#212-culture)
    - [2.1.3. CurrentCulture.](#213-currentculture)
    - [2.1.4. Cultures.](#214-cultures)
    - [2.1.5. ErrorHandling.](#215-errorhandling)
    - [2.1.6. Translate.](#216-translate)
      - [2.1.6.1. Translate to neutral culture:](#2161-translate-to-neutral-culture)
      - [2.1.6.2. Translate to explicit culture:](#2162-translate-to-explicit-culture)
      - [2.1.6.3. Override global error handling (throw on error):](#2163-override-global-error-handling--throw-on-error)
      - [2.1.6.4. Override global error handling (return info about error):](#2164-override-global-error-handling--return-info-about-error)
      - [2.1.6.5. Translate with parameter:](#2165-translate-with-parameter)
  - [2.2. Translator&lt;T&gt;.](#22-translatort)
  - [2.3. Translation.](#23-translation)
  - [2.3.1 GetOrCreate.](#231-getorcreate)
  - [2.4. StaticTranslation.](#24-statictranslation)
- [3. ErrorHandling.](#3-errorhandling)
  - [3.1. Global setting](#31-global-setting)
  - [3.2. ErrorFormats](#32-errorformats)
- [4. Validate.](#4-validate)
  - [4.1. Translations.](#41-translations)
  - [4.2. EnumTranslations&lt;T&gt;.](#42-enumtranslationst)
  - [4.3. TranslationErrors](#43-translationerrors)
  - [4.4. Format](#44-format)
- [5. FormatString.](#5-formatstring)
  - [5.1. IsFormatString](#51-isformatstring)
  - [5.2. IsValidFormatString](#52-isvalidformatstring)
- [6. LanguageSelector](#6-languageselector)
  - [6.1. AutogenerateLanguages](#61-autogeneratelanguages)
  - [6.2. Explicit languages.](#62-explicit-languages)

# 1. Usage in XAML.

The library has a `StaticExtension` markupextension that is used when translating.
The reason for naming it `StaticExtension` and not `TranslateExtension` is that Resharper provides intellisense when named `StaticExtension`
Binding the text like below updates the text when `Translator.CurrentCulture`changes enabling runtime selection of language.

The markupextension has ErrorHandling = ErrorHandling.ReturnErrorInfoPreserveNeutral as default, it encodes errors in the result, see [ErrorFormats](#3-errorhandling))
## 1.1. Bind a localized string.

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

## 1.2. Errorhandling.
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

## 1.3. CurrentCulture.
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

## 1.4. Binding to CurrentCulture and CurrentCulture in XAML.
The static properties support binding. Use this XAML for a twoway binding:
```xaml
<TextBox Text="{Binding Path=(localization:Translator.CurrentCulture)}" />
```

# 2. Usage in code.

The API is not super clean, introducing a helper like this can clean things up a bit.

Creating it like the above is pretty verbose. Introducing a helper like below can help some.

```c#
namespace YourNamespace.Properties
{
    using Gu.Localization;

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

## 2.1. Translator.

### 2.1.1. Culture.
Get or set the current culture. The default is `null`
Changing culture updates all translations. Setting culture to a culture for which there is no translation throws. Check ContainsCulture() first.

### 2.1.2. Culture.
Get or set the current culture. The default is `null`
Changing culture updates all translations. Setting culture to a culture for which there is no translation throws. Check ContainsCulture() first.

### 2.1.3. CurrentCulture.
Get the culture used in translations. By the following mechanism:
  1) CurrentCulture if not null.
  2) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
  3) Any Culture in <see cref="Cultures"/> matching <see cref="CultureInfo.CurrentCulture"/> by name.
  4) CultureInfo.InvariantCulture
When this value changes CurrentCultureChanged is raised and all translatins updates and notifies.

### 2.1.4. Cultures.
Get a list with the available cultures. Cultures are found by looking in current directory and scanning for satellite assemblies.

### 2.1.5. ErrorHandling.
Get or set how errors are handled. The default value is `ReturnErrorInfoPreserveNeutral`.

### 2.1.6. Translate.
Translate a key in a ResourceManager.

Use global culture & error handling:
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en"); // no need to set this every time, just for illustration purposes here.
string inEnglish = Translator.Translate(Properties.Resources.ResourceManager,
                                        nameof(Properties.Resources.SomeResource));
```

#### 2.1.6.1. Translate to neutral culture:
```c#
string neutral = Translator.Translate(Properties.Resources.ResourceManager, 
                                      nameof(Properties.Resources.SomeResource), 
                                      CultureInfo.InvariantCulture);
```

#### 2.1.6.2. Translate to explicit culture:
```c#
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource), 
                                        CultureInfo.GetCultureInfo("sv"));
```

#### 2.1.6.3. Override global error handling (throw on error):
```c#
Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo; // no need to set this every time, just for illustration purposes here.
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource), 
                                        ErrorHandling.Throw);
```

#### 2.1.6.4. Override global error handling (return info about error):
```c#
Translator.ErrorHandling = ErrorHandling.Throw; // no need to set this every time, just for illustration purposes here.
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource), 
                                        ErrorHandling.ReturnErrorInfo);
```

#### 2.1.6.5. Translate with parameter:
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource__0__), 
                                        foo);
```

## 2.2. Translator&lt;T&gt;.

Same as translator but used like `Translator<Properties.Resources>.Translate(...)`

## 2.3. Translation.
An object with a Translated property that is a string with the value in `Translator.CurrentCulture` 
Implements ÌNotifyPropertyChanged` and notifies when for the property `Translated` if a change in `Translator.CurrentCulture` updates the translation.

## 2.3.1 GetOrCreate.
Returns an `ITranslation` from cache or creates and caches a new instance.
If ErrorHandling is Throw it throws if the key is missing. If other than throw a `StaticTranslation` is returned.

```c#
Translation translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeResource))
```

## 2.4. StaticTranslation.
An implementation of `ITranslation` that never updates the `Translated`property and returns the value of `Translated` when calling `Translate()`on it with any paramaters.
This is returned from `Translation.GetOrCreate(...)` if the key is missing.

# 3. ErrorHandling.
When calling the translate methods an ErrorHandling argument can be provided.
If `ErrorHandling.ReturnErrorInfo` is passed in the method does not throw but returns information about the error in the string.
There is also a property `Translator.ErrorHandling` that sets default behaviour. If an explicit errorhandling is passed in to a method it overrides the global setting.

## 3.1. Global setting
By setting `Translator.Errorhandling` the global default is changed.

## 3.2. ErrorFormats
When `ReturnErrorInfo` or `ReturnErrorInfoPreserveNeutral` is used the following formats are used to encode errors.

| Error               |         Format          |
|---------------------|:-----------------------:|
| missing key         |        `!{key}!`        |
| missing culture     |        `~{key}~`        |
| missing translation |        `_{key}_`        |
| missing resources   |        `?{key}?`        |
| invalid format      |`{{"{format}" : {args}}}`|
| unknown error       |    `#{key}#`            |


# 4. Validate.
Conveience API for unit testing localization. 

## 4.1. Translations.

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

## 4.2. EnumTranslations&lt;T&gt;.
Validate an `enum` like this:
```c#
TranslationErrors errors = Validate.EnumTranslations<DummyEnum>(Properties.Resources.ResourceManager);
Assert.IsTrue(errors.IsEmpty);
```
Checks:
- That all enum members has keys in the `ResourceManager`
- That all keys has non null value for all cultures in `Translator.AllCultures`

## 4.3. TranslationErrors
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
## 4.4. Format
Validate a formatstring like this:
```c#
Validate.Format("Value: {0}", 1);
```

```c#
Debug.Assert(Validate.IsValidFormat("Value: {0}", 1), "Invalid format...");
```

# 5. FormatString.
Conveience API for testing formatstrings.

## 5.1. IsFormatString
Returns true if the string contains placeholders like `"Value: {0}"` and is a valid format string.

## 5.2. IsValidFormatString
Returns true if the string contains placeholders like `"Value: {0}"` that matches the number of parameters and is a valid format string.


# 6. LanguageSelector
A simple control for changing current language.
A few flags are included in the library, many are probably missing.

## 6.1. AutogenerateLanguages
Default is false. 
If true it popolates itself with `Translator.Cultures` in the running application and picks the default flag or null.

```xaml
<l:LanguageSelector AutogenerateLanguages="True" />
```

## 6.2. Explicit languages.

```xaml
<l:LanguageSelector>
    <l:Language Culture="de-DE"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/de.png" />
    <l:Language Culture="en-GB"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/en.png" />                
    <l:Language Culture="sv-SE"
                FlagSource="pack://application:,,,/Gu.Wpf.Localization;component/Flags/sv.png" />
</l:LanguageSelector>
```

![screenie](http://i.imgur.com/DKfx8WB.png)
