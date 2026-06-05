# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

`Crude.Logging.Console` is a single-project NuGet library: a minimal colorized `ConsoleFormatter` for `Microsoft.Extensions.Logging` that emits one-line, color-coded output and highlights text wrapped in `'single quotes'`. Source lives in `src/Crude.Logging.Console/`.

## Build & pack

- Targets **net10.0** — requires the .NET 10 SDK. The solution is `CrudeLogFormatter.slnx` at the repo root (`.slnx` format, not `.sln`).
- `dotnet build -c Release` also produces the `.nupkg` — `GeneratePackageOnBuild` is on, so a Release build packs automatically (no separate `dotnet pack` needed).
- The package version comes from `<VersionPrefix>` in the `.csproj`; bump it there for releases.

## Gotchas

- **`TreatWarningsAsErrors` is true** — any compiler/analyzer warning fails the build. Code must be warning-clean, including for `Nullable` (enabled).
- **Trimming/AOT-aware** — the formatter is registered via `[DynamicDependency]` (see `Extensions/LoggingBuilderExtensions.cs`). Don't introduce reflection that would break trimming, and preserve those attributes.
- The `Microsoft.Extensions.Logging.Console` dependency is pinned to `[10.0.0, 11)`; keep it in that major range.
- **No test project** exists — there are no automated tests to run. Verify changes by building and, if needed, exercising the formatter from a sample host.
