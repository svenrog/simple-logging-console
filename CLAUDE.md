# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

`Simple.Logging.Console` is a single-project NuGet library: a minimal colorized `ConsoleFormatter` for `Microsoft.Extensions.Logging` that emits one-line, color-coded output and highlights text wrapped in `'single quotes'`. Source lives in `src/Simple.Logging.Console/`.

Formerly published as `Crude.Logging.Console` (namespace `Crude.Logging.Console`, class `CrudeLogFormatter`, formatter name `"crude"`); that package is deprecated in favor of this one.

## Build & pack

- Targets **net10.0** — requires the .NET 10 SDK. The solution is `SimpleLogFormatter.slnx` at the repo root (`.slnx` format, not `.sln`).
- `dotnet build -c Release` also produces the `.nupkg` — `GeneratePackageOnBuild` is on, so a Release build packs automatically (no separate `dotnet pack` needed).
- The package version comes from `<VersionPrefix>` in the `.csproj`; bump it there for releases.

## Gotchas

- **`TreatWarningsAsErrors` is true** — any compiler/analyzer warning fails the build. Code must be warning-clean, including for `Nullable` (enabled).
- **`IsAotCompatible` is true** — trim/AOT analyzers run on every build, not just publish. `AddConsoleLogging` (see `Extensions/LoggingBuilderExtensions.cs`) registers `SimpleLogFormatter` directly via `TryAddEnumerable(ServiceDescriptor.Singleton<ConsoleFormatter, SimpleLogFormatter>())` rather than the generic `AddConsoleFormatter<TFormatter, TOptions>()` helper — that helper also wires up reflection-based configuration binding for `TOptions`, which requires `[RequiresUnreferencedCode]`/`[RequiresDynamicCode]` and which this formatter never needs since it takes no options. If you ever need config-bound options here, you'll have to reintroduce those attributes (and they'll propagate to every caller).
- **Formatter name must not collide with a built-in name** — `ConsoleFormatterNames` reserves `"simple"`, `"json"`, and `"systemd"`. `SimpleLogFormatter.FormatterName` is `"simple-color"` specifically to avoid colliding with the built-in `"simple"` formatter; a collision silently falls back to the built-in formatter instead of throwing, so it won't be caught by tests that call `Write()` directly — only by exercising `AddConsoleLogging()` through actual DI.
- The `Microsoft.Extensions.Logging.Console` dependency is pinned to `[10.0.0, 11)`; keep it in that major range.
- A test project exists at `tests/Simple.Logging.Console.Tests` (xUnit). Run `dotnet test SimpleLogFormatter.slnx`.
- **`icon.png` is CC BY-SA 4.0, not GPL-2.0-only** — it's a cropped version of a third-party Wikimedia Commons image, licensed separately from the code (see the Icon section in `README.md`). Don't replace or crop it further without preserving attribution, and don't assume `PackageLicenseExpression` covers it.

## Legacy `Crude.Logging.Console` package

`legacy/Crude.Logging.Console/` is a standalone (not part of `SimpleLogFormatter.slnx`) meta-package: it has no source of its own, just a `ProjectReference` to `src/Simple.Logging.Console`. Packing it produces a `Crude.Logging.Console` nupkg whose nuspec forwards via a package dependency onto `Simple.Logging.Console`, so existing consumers who don't change their reference still pull the current implementation. `dotnet build -c Release` there packs it the same way. This exists to deprecate the old package id without deleting it outright; it should not receive further feature changes.
