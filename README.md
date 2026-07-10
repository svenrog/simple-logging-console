# Simple log formatter 🌈

[![Platform](https://img.shields.io/badge/Platform-.NET%2010-blue.svg?style=flat)](https://docs.microsoft.com/en-us/dotnet/)
[![NuGet](https://img.shields.io/nuget/v/Simple.Logging.Console)](https://www.nuget.org/packages/Simple.Logging.Console)
[![License: MIT](https://img.shields.io/github/license/svenrog/simple-logging-console)](LICENSE.txt)

A way to simplify console logging from the default C# fancy 2 line logging that provides way too much information for simple applications.

- Has some simple coloring related to log levels.
- Has some highlight formatting when placing text between `'single quotes'` or `` `backticks` ``.
- Colors are configurable per log level via a `LogPalette`.

## Usage

`dotnet add package Simple.Logging.Console`

In your startup code

```
var builder = Host.CreateApplicationBuilder(arguments);
builder.Logging.AddConsoleLogging();
```

`AddConsoleLogging` uses the ANSI 16-color formatter, which renders correctly on any terminal that understands SGR escape codes. For 24-bit truecolor, use `AddRgbConsoleLogging` instead (see [Custom palette](#custom-palette)).

## Highlighting

Text wrapped in `'single quotes'` is drawn in the level's highlight color; text wrapped in `` `backticks` `` is drawn in its accent color. The two delimiters don't nest inside each other — a backtick inside a quoted span (or vice versa) is treated as literal text.

## Custom palette

Pass a delegate to override any of the per-level colors, the timestamp color, the exception color, or the highlight/accent delimiter characters. Palettes are **homogeneous**: `AddConsoleLogging` configures a `LogPalette<AnsiColor>` (standard `ConsoleColor` values, implicitly convertible), and `AddRgbConsoleLogging` configures a `LogPalette<RgbColor>` (24-bit truecolor). Each color slot is an `IConsoleColor`; a single palette can't mix the two, which is what keeps the formatter allocation-free (no boxing, no per-line escape-string allocation).

ANSI (works everywhere):

```
builder.Logging.AddConsoleLogging(configurePalette: palette =>
{
    palette.Warning = palette.Warning with { AccentColor = ConsoleColor.DarkYellow };
    palette.HighlightDelimiter = '*';
    palette.AccentDelimiter = '~';
});
```

Truecolor:

```
builder.Logging.AddRgbConsoleLogging(configurePalette: palette =>
{
    palette.Warning = palette.Warning with { AccentColor = RgbColor.FromHex(0xFF8800) };
    palette.Information = palette.Information with { MessageColor = new RgbColor(0, 200, 255) };
});
```

Truecolor renders correctly on modern ANSI-capable terminals; if output is redirected to a file or viewed through a legacy console, `Microsoft.Extensions.Logging.Console`'s own fallback only recognizes the standard 16 colors, so truecolor escape sequences may appear as raw text there — use `AddConsoleLogging` if that scenario matters to you.

`LogPalette.LikelySupportsTrueColor` is a best-effort, static heuristic (`COLORTERM`, `WT_SESSION`, `NO_COLOR`, and whether output is redirected) you can check before choosing which formatter to register:

```
if (LogPalette.LikelySupportsTrueColor)
    builder.Logging.AddRgbConsoleLogging();
else
    builder.Logging.AddConsoleLogging();
```

There's no way to reliably query a terminal's actual color depth without risking a hang on redirected output, so treat this as a hint, not a guarantee.

Each level's badge background defaults to transparent so it doesn't clash with the terminal's own background; `Error` and `Critical` are the exception, keeping a filled block to stand out.

## Previewing a palette

The `samples/Simple.Logging.Console.Preview` project renders every log level and color slot (badge, message, highlight, accent, timestamp, exception) for both the ANSI and RGB defaults — plus a no-color fallback pass (SGR codes stripped) that emulates how the output degrades on a terminal without color support or under `NO_COLOR` — so you can eyeball a palette before shipping it:

```
dotnet run --project samples/Simple.Logging.Console.Preview
```

Point it at your own palette by passing it to `new RgbLogFormatter(palette)` (or `new ConsoleLogFormatter(palette)`) in `Program.cs`.

> Formerly published as `Crude.Logging.Console`. That package is deprecated in favor of this one — see the [changelog](CHANGELOG.md).

## Icon

The package icon is a cropped version of ["Rainbow Emoji"](https://commons.wikimedia.org/wiki/File:Rainbow_Emoji.png) by [EmmanuelCordoliani](https://commons.wikimedia.org/wiki/User:EmmanuelCordoliani), used under [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/). It is licensed separately from the source code above (see [LICENSE.txt](LICENSE.txt)); redistributing `icon.png` itself, modified or not, must keep it under CC BY-SA 4.0 (or a compatible license) with the same attribution.

## Package maintainer

https://github.com/svenrog

## Change log

[Changes are documented in](CHANGELOG.md)
