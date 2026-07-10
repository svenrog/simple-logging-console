# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

`Simple.Logging.Console` is a single-project NuGet library: a minimal colorized `ConsoleFormatter` for `Microsoft.Extensions.Logging` that emits one-line, color-coded output and highlights text wrapped in `'single quotes'`. Source lives in `src/Simple.Logging.Console/`.

**Architecture (2.0+):** the formatter is generic over the color struct — `AbstractLogFormatter<TColor> where TColor : struct, IConsoleColor` — with two sealed closes: `ConsoleLogFormatter` (`AnsiColor`, name `"simple-color"`) and `RgbLogFormatter` (`RgbColor`, name `"simple-rgb"`). This is a deliberate performance design: colors emit their own escape codes via `IConsoleColor.WriteForeground/WriteBackground`, and because those are called through the constrained generic parameter (not an `ILogColor` interface slot), the JIT specializes per struct with **no boxing and no interface dispatch**. A warm `Write` allocates 0 bytes; `benchmarks/Simple.Logging.Console.Benchmarks` guards this and `tests/…/AllocationTests.cs` asserts it. Consequences: palettes are homogeneous (`LogPalette<TColor>`, all-`AnsiColor` or all-`RgbColor`); `RgbColor` writes digits into a stackalloc buffer rather than interpolating a string; default palettes come from `DefaultPalettes.Ansi()/Rgb()` (one shared spec). Don't reintroduce a non-generic `ILogColor`-typed color slot on the hot path — that brings the boxing back.

Formerly published as `Crude.Logging.Console` (namespace `Crude.Logging.Console`, class `CrudeLogFormatter`, formatter name `"crude"`); that package is deprecated in favor of this one.

## Build & pack

- Targets **net10.0** — requires the .NET 10 SDK. The solution is `SimpleLogFormatter.slnx` at the repo root (`.slnx` format, not `.sln`).
- `dotnet build -c Release` also produces the `.nupkg` — `GeneratePackageOnBuild` is on, so a Release build packs automatically (no separate `dotnet pack` needed).
- The package version comes from `<VersionPrefix>` in the `.csproj`; bump it there for releases.

## Gotchas

- **`TreatWarningsAsErrors` is true** — any compiler/analyzer warning fails the build. Code must be warning-clean, including for `Nullable` (enabled).
- **`IsAotCompatible` is true** — trim/AOT analyzers run on every build, not just publish. The private `AddFormatter<TColor>` helper (see `Extensions/LoggingBuilderExtensions.cs`) registers a **prebuilt formatter instance** via `TryAddEnumerable(ServiceDescriptor.Singleton<ConsoleFormatter>(formatter))` rather than the generic `AddConsoleFormatter<TFormatter, TOptions>()` helper — that helper wires up reflection-based configuration binding for `TOptions`, which requires `[RequiresUnreferencedCode]`/`[RequiresDynamicCode]` and which this formatter never needs since it takes no options. Registering an instance (not activating by type) also sidesteps the `[DynamicallyAccessedMembers(PublicConstructors)]` annotation that `Singleton<TService, TImplementation>()` would otherwise demand (IL2091), and lets the extension pass the caller's configured palette + `colorize` into the constructor. If you ever switch back to type activation or need config-bound options, those attributes come back.
- **NO_COLOR / `RespectNoColor`** — `LogPalette<TColor>.RespectNoColor` (default true) controls it: `AbstractLogFormatter` resolves `_colorize = LogPalette.ShouldColorize(palette.RespectNoColor, Environment.GetEnvironmentVariable("NO_COLOR"))` once at construction. Honors the NO_COLOR standard only (any non-empty value), **not** output redirection. When off, the color writes in `AbstractLogFormatter`/`TextWriterExtensions` are gated but the delimiter-stripping still runs, so plain-text output stays readable and allocation-free. Tests that depend on the env var live in `NoColorTests` (a `DisableParallelization` collection, since `NO_COLOR` is process-global); `AllocationTests` forces the color path with `RespectNoColor = false` palettes so it's env-independent.
- **Formatter names must not collide with a built-in name** — `ConsoleFormatterNames` reserves `"simple"`, `"json"`, and `"systemd"`. `ConsoleLogFormatter.FormatterName` is `"simple-color"` and `RgbLogFormatter.FormatterName` is `"simple-rgb"`, both chosen to avoid the built-in `"simple"`; a collision silently falls back to the built-in formatter instead of throwing, so it won't be caught by tests that call `Write()` directly — only by exercising `AddConsoleLogging()`/`AddRgbConsoleLogging()` through actual DI.
- The `Microsoft.Extensions.Logging.Console` dependency is pinned to `[10.0.0, 11)`; keep it in that major range.
- A test project exists at `tests/Simple.Logging.Console.Tests` (xUnit). Run `dotnet test SimpleLogFormatter.slnx`. Benchmarks live in `benchmarks/Simple.Logging.Console.Benchmarks` (BenchmarkDotNet); run with `dotnet run -c Release --project benchmarks/… -- --filter '*'`. A palette preview app is at `samples/Simple.Logging.Console.Preview` (drives the formatter directly so it renders every level including `None`, which `ILogger` can't log); `dotnet run --project samples/…` to eyeball a palette.
- **`icon.png` is CC BY-SA 4.0, not MIT** — it's a cropped version of a third-party Wikimedia Commons image, licensed separately from the code (see the Icon section in `README.md`). Don't replace or crop it further without preserving attribution, and don't assume `PackageLicenseExpression` covers it.

## Legacy `Crude.Logging.Console` package

`legacy/Crude.Logging.Console/` is a standalone (not part of `SimpleLogFormatter.slnx`) meta-package: it has no source of its own, just a `ProjectReference` to `src/Simple.Logging.Console`. Packing it produces a `Crude.Logging.Console` nupkg whose nuspec forwards via a package dependency onto `Simple.Logging.Console`, so existing consumers who don't change their reference still pull the current implementation. `dotnet build -c Release` there packs it the same way. This exists to deprecate the old package id without deleting it outright; it should not receive further feature changes.
