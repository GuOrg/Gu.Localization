# Gu.Localization.
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![Build status](https://ci.appveyor.com/api/projects/status/ili1qk8amyjmd71t?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-localization)
[![NuGet](https://img.shields.io/nuget/v/Gu.Localization.svg)](https://www.nuget.org/packages/Gu.Localization/)
[![NuGet](https://img.shields.io/nuget/v/Gu.Wpf.Localization.svg)](https://www.nuget.org/packages/Gu.Wpf.Localization/)


## Contents.
  - [1. Usage in XAML.](#1-usage-in-xaml)
  - [2. Usage in code.](#2-usage-in-code)
    - [2.1. Translator.](#21-translator)
      - [2.1.1. Culture.](#211-culture)
      - [2.1.2. Cultures.](#212-cultures)
      - [2.1.3. ErrorHandling.](#213-errorhandling)
      - [2.1.4. Translate.](#214-translate)
    - [2.2. Translator&lt;T&gt;.](#22-translatort)
    - [2.3 Translation.](#23-translation)
  - [3. ErrorHandling.](#3-errorhandling)
  - [4. Validation.](#4-validation)
    - [4.1. Translations.](#41-translations)
    - [4.2. EnumTranslations&lt;T&gt;.](#42-enumtranslationst)
    - [4.3. TranslationErrors](#43-translationerrors)
    - [4.4. Format](#44-format)
  - [5. ErrorFormats](#5-errorformats)
  - [7. LanguageSelector](#7-languageselector)

## 1. Usage in XAML.

The library has a `StaticExtension` markupextension that is used when translating.
The reason for naming it `StaticExtension` and not `TranslateExtension` is that Resharper provides intellisense when named `StaticExtension`
Binding the text like below updates the text when `Translator.CurrentCulture`changes enabling runtime selection of language.

The markupextension never throws, it encodes errors in the result, see [ErrorFormats](#41-errorformats)

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

## 2. Usage in code.
### 2.1. Translator.

#### 2.1.1. Culture.
Get or set the current culture. The default is `Thread.CurrentThread.CurrentUICulture`
Changing culture updates all translations.

#### 2.1.2. Cultures.
Get a list with the available cultures. Cultures are found by looking in current directory and scanning for satellite assemblies.

#### 2.1.3. ErrorHandling.
Get or set how errors are handled. The default behaviour is throw on errors.

#### 2.1.4. Translate.
Translate a key in a ResourceManager.

Use global culture & error handling:
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
string inEnglish = Translator.Translate(Properties.Resources.ResourceManager,
                                        nameof(Properties.Resources.SomeResource));
```

Translate to neutral culture:
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
string neutral = Translator.Translate(Properties.Resources.ResourceManager, 
                                      nameof(Properties.Resources.SomeResource), 
                                      null);
```

Translate to explicit culture:
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource), 
                                        CultureInfo.GetCultureInfo("sv"));
```

Override global error handling (throw on error):
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource), 
                                        ErrorHandling.Throw);
```

Override global error handling (return info about error):
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
Translator.ErrorHandling = ErrorHandling.Throw;
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource), 
                                        ErrorHandling.ReturnErrorInfo);
```

Translate with parameter:
```c#
Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
string inSwedish = Translator.Translate(Properties.Resources.ResourceManager, 
                                        nameof(Properties.Resources.SomeResource__0__), 
                                        foo);
```

### 2.2. Translator&lt;T&gt;.

Same as translator but used like `Translator<Properties.Resources>.Translate(...)`

### 2.3 Translation.
An object with a Translated property that is a string with the value in `Translator.CurrentCulture` 
Implements ÌNotifyPropertyChanged` and notifies when `Translator.CurrentCulture` changes.

```c#
Translation translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeResource))
```

## 3. ErrorHandling.
When calling the translate methods an ErrorHandling argument can be provided.
If `ErrorHandling.ReturnErrorInfo` is passed in the method does not throw but returns information about the error in the string.
There is also a property `Translator.ErrorHandling` that sets default behaviour. If an explicit errorhandling is passed in to a method it overrides the global setting.

## 4. Validation.
Conveience API for unit testing localization. 

### 4.1. Translations.
Not optimized for performance and should probably not.

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

### 4.2. EnumTranslations&lt;T&gt;.
Validate an `enum` like this:
```c#
TranslationErrors errors = Validate.EnumTranslations<DummyEnum>(Properties.Resources.ResourceManager);
Assert.IsTrue(errors.IsEmpty);
```
Checks:
- That all enum members has keys in the `ResourceManager`
- That all keys has non null value for all cultures in `Translator.AllCultures`

### 4.3. TranslationErrors
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
### 4.4. Format
Validate a formatstring like this:
```c#
Validate.Format("Value: {0}", 1);
```

```c#
Debug.Assert(Validate.IsValidFormat("Value: {0}", 1), "Invalid format...");
```

## 5. ErrorFormats
When `ReturnErrorInfo` is used the following formats are used to encode errors.

| Error               |         Format          |
|---------------------|:-----------------------:|
| missing key         |        `!{key}!`        |
| missing culture     |        `~{key}~`        |
| missing translation |        `_{key}_`        |
| missing resources   |        `?{key}?`        |
| invalid format      |`{{"{format}" : {args}}}`|
| unknown error       |    `#{key}#`            |

## 7. LanguageSelector
A simple control for changing current language.

`AutogenerateLanguages="True"` displays all cultures found in the running application and picks the default flag.
A few flags are included in the library, many are probably missing.

```xaml
<l:LanguageSelector AutogenerateLanguages="True" />
```

Or

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
