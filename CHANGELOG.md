# Changelog

All notable changes to this project will be documented in this file.

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
