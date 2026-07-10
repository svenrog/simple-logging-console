# Changelog

All notable changes to this project will be documented in this file.

## [2.0.0] (Simple.Logging.Console)

Performance-focused rewrite: a warm `Write` now allocates **0 bytes** (previously 48 B/line for ANSI and up to 544 B/line for RGB), and RGB formatting is ~37% faster. See `benchmarks/Simple.Logging.Console.Benchmarks`.

**Breaking changes:**

- Replaced the `ILogColor` marker interface with `IConsoleColor`, which now carries `WriteForeground`/`WriteBackground`. `AnsiColor` and `RgbColor` implement it and emit their own escape codes; `RgbColor` writes digits directly into a stack buffer instead of interpolating a string, so it no longer allocates per line.
- `LogPalette` and `LogLevelColors` are now generic (`LogPalette<TColor>`, `LogLevelColors<TColor>`) and homogeneous — a single palette is entirely `AnsiColor` or entirely `RgbColor`. This is what lets the formatter stay generic and boxing-free.
- Replaced `SimpleLogFormatter` with an abstract `AbstractLogFormatter<TColor>` base and two sealed formatters: `ConsoleLogFormatter` (ANSI, name `"simple-color"`) and `RgbLogFormatter` (24-bit truecolor, name `"simple-rgb"`). The generic pipeline uses constrained calls, so color writes never box or dispatch through an interface.
- Added `AddRgbConsoleLogging` for the truecolor formatter; `AddConsoleLogging` now registers the ANSI formatter and its `configurePalette` delegate takes a `LogPalette<AnsiColor>`.
- The static true-color heuristic remains `LogPalette.LikelySupportsTrueColor`, now on a non-generic `LogPalette` type alongside the generic `LogPalette<TColor>` configuration holder.
- Build populated defaults via `DefaultPalettes.Ansi()` / `DefaultPalettes.Rgb()` (both derived from one shared spec, so they stay in visual parity).

**Other:**

- Added a `colorize` option (tri-state `bool?`) to both formatters and to `AddConsoleLogging`/`AddRgbConsoleLogging`. `null` (default) honors the [NO_COLOR](https://no-color.org) standard — any non-empty `NO_COLOR` disables color; `true`/`false` force it. When color is off the formatter emits plain text at the source (no escape codes) while still consuming the highlight/accent delimiters, so lines stay readable. Exposed as `LogPalette.ShouldColorizeByDefault`. Output redirection is deliberately not auto-treated as no-color; pass `colorize: false` (or set `NO_COLOR`) for that.

## [1.1.0] (Simple.Logging.Console)

- Relicensed from `GPL-2.0-only` to `MIT`, so referencing this package no longer imposes copyleft obligations on the consuming project. Prior published versions (1.0.0, 1.0.1) remain under `GPL-2.0-only`.
- Added a configurable `LogPalette`: pass `configurePalette` to `AddConsoleLogging`, or a `LogPalette` instance to `SimpleLogFormatter`'s constructor, to override badge/message/highlight/accent/timestamp/exception colors per log level.
- Added the `ILogColor` interface, implemented by `AnsiColor` (a `ConsoleColor`, implicitly convertible, unchanged default behavior) and `RgbColor` (a 24-bit truecolor value, via its constructor or `RgbColor.FromHex`).
- Added a second highlight tier: text wrapped in `` `backticks` `` now renders in a level's accent color, alongside the existing `'single quote'` highlighting. The two delimiters don't nest inside each other.
- Added `LogPalette.HighlightDelimiter` / `AccentDelimiter` so the `'` and `` ` `` delimiter characters are configurable, defaulting to their existing values.
- Added `LogPalette.LikelySupportsTrueColor`, a static best-effort heuristic (`COLORTERM`, `WT_SESSION`, `NO_COLOR`, output redirection) for deciding whether to opt into an `RgbColor`-based palette.
- Fixed `Trace`/`Debug`/`Information`/`Warning` badges rendering with an explicit black background, which stood out against non-black terminal backgrounds. Their backgrounds are now transparent by default; `Error`/`Critical` keep their filled alert block.

## [1.0.1] (Simple.Logging.Console)

- Fixed the formatter name colliding with the built-in console formatter reserved name `"simple"` (`Microsoft.Extensions.Logging.Console.ConsoleFormatterNames.Simple`), which silently caused `AddConsoleLogging()` to fall back to the default 2-line formatter instead of `SimpleLogFormatter`. Renamed to `"simple-color"`.
- Removed the `[RequiresUnreferencedCode]`/`[RequiresDynamicCode]` warnings on `AddConsoleLogging` (added in 1.0.0) by registering `SimpleLogFormatter` directly instead of via `AddConsoleFormatter<TFormatter, TOptions>()`, which also wires up reflection-based configuration binding for `TOptions` that this formatter never reads. Consumers publishing with trimming/AOT no longer see these warnings at all.

## [1.0.0] (Simple.Logging.Console)

- Renamed package from `Crude.Logging.Console` to `Simple.Logging.Console` (namespace, formatter name, and repository all follow). The old package is deprecated; see its own changelog entry below.
- Renamed `CrudeLogFormatter` to `SimpleLogFormatter` and the formatter name from `"crude"` to `"simple"`.
- Marked the package `IsAotCompatible`, and fixed the resulting IL2026/IL3050 trim/AOT analyzer warnings on `AddConsoleLogging` by propagating `RequiresUnreferencedCode`/`RequiresDynamicCode` from `AddConsoleFormatter`.

---

Prior history, published as `Crude.Logging.Console`:

## [1.0.4] (Crude.Logging.Console, deprecated)

- Deprecated in favor of `Simple.Logging.Console`.

## [1.0.3]

- Fixed `'single quote'` highlighting treating word apostrophes (e.g. `couldn't`, `server's`) as delimiters
- Fixed an unmatched trailing quote highlighting the rest of the line
- Exceptions are now written on their own line
- `LogLevel.None` no longer throws
- Added test project

## [1.0.2]

- Fixed issue with logging builder

## [1.0.1]

- Fixed project reference issue


## [1.0.0]

- Added initial implementation
