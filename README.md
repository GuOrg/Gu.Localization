# Gu.Localization
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) 
[![Build status](https://ci.appveyor.com/api/projects/status/ili1qk8amyjmd71t?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-localization)


## Usage in xaml:

The library has a `StaticExtension` markupextension that is used when translating.
The reason for naming it `StaticExtension` and not `TranslateExtension` is that Resharper provides intellisense when named `StaticExtension`
Binding the text like below updates the text when `Translator.CurrentCulture`changes enabling runtime selection of language.

```
<Window ...
        xmlns:p="clr-namespace:Gu.Wpf.Localization.Demo.WithResources.Properties"
        xmlns:l="http://gu.se/Localization">
    ...
    <TextBlock Text="{l:Static p:Resources.SomeResource}" />
    ...
```

## Usage in code:
```
string translated = Translator.Translate(() => Resources.SomeResource);
```

## Misc.
Does not use CurrentUICulture, set the culture explicitly.
