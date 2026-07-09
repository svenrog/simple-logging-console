# Simple log formatter

A way to simplify console logging from the default C# fancy 2 line logging that provides way too much information for simple applications.

- Has some simple coloring related to log levels.
- Has some highlight formatting when placing text between single quotes ''.

## Usage

`dotnet add package Simple.Logging.Console`

In your startup code

```
var builder = Host.CreateApplicationBuilder(arguments);
builder.Logging.AddConsoleLogging();
```

> Formerly published as `Crude.Logging.Console`. That package is deprecated in favor of this one — see the [changelog](CHANGELOG.md).

## Icon

The package icon is a cropped version of ["Rainbow Emoji"](https://commons.wikimedia.org/wiki/File:Rainbow_Emoji.png) by [EmmanuelCordoliani](https://commons.wikimedia.org/wiki/User:EmmanuelCordoliani), used under [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/). It is licensed separately from the source code above (see [LICENSE.txt](LICENSE.txt)); redistributing `icon.png` itself, modified or not, must keep it under CC BY-SA 4.0 (or a compatible license) with the same attribution.

## Package maintainer

https://github.com/svenrog

## Changelog

[Changelog](CHANGELOG.md)
