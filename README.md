# Gu.Localization
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) 
[![Build status](https://ci.appveyor.com/api/projects/status/ili1qk8amyjmd71t?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-localization)

## Table of Contents
- [1. Usage in XAML.](#1-usage-in-xaml)
- [2. Usage in code](#2-usage-in-code)
- [3. Error formats](#3-error-formats)
- [4. LanguageSelector](#4-languageselector)

## 1. Usage in XAML.

The library has a `StaticExtension` markupextension that is used when translating.
The reason for naming it `StaticExtension` and not `TranslateExtension` is that Resharper provides intellisense when named `StaticExtension`
Binding the text like below updates the text when `Translator.CurrentCulture`changes enabling runtime selection of language.

```
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
```
string translated = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.SomeResource));
string translated = TranslatorTranslate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeResource));
Translation translation = Translation.GetOrCreate(Properties.Resources.ResourceManager, nameof(Properties.Resources.SomeResource))
```

## 3. Error formats
| Error               |  Format      |
|---------------------|:------------:|
| missing key         |    `!{0}!`   |
| missing culture     |    `~{0}~`   |
| missing translation |    `_{0}_`   |
| missing resources   |    `?{0}?`   |
| unknown error       |    `#{0}#`   |

## 4. LanguageSelector
A simple control for changing current language.

`AutogenerateLanguages="True"` displays all cultures found in the running application and picks the default flag.
A few flags are included in the library, many are probably missing.

```
<l:LanguageSelector AutogenerateLanguages="True" />
```

Or

```
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
