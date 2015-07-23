# Gu.Localization

## Usage in code:
```
string translated = Translator.Translate(() => Resources.SomeResource);
```

## Usage in xaml:
```
<TextBlock FontWeight="Bold"
           Text="{l:Static p:Resources.SomeResource}"
           />
<!--
using the name l:static fools R# to give intellisense 
(xmlns:localization="clr-namespace:Gu.Wpf.Localization;assembly=Gu.Wpf.Localization")
xmlns:p="clr-namespace:YourProject.Properties"
--> 
```
