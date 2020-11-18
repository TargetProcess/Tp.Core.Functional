# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [NEXT] - Unreleased

### Breaking changes
- `Maybe<T>` now has `T : notnull` constraint
- It's not possible to have `Maybe<T>` with `null` value anymore
    - `Maybe.Just(null)` and other constructors throw `ArgumentNullException` in this case
- `Maybe<T>` implements `IEquatable<Maybe<T>>` now
    - It already had non-interface `Equals` method before that
- Remove assembly signing
- Some utility methods are moved from `Try` instance to `TryExtensions` for better interop with nullable types

### Changed
- Add nullable annotations
- It's recommended to use `Maybe.GetOrNull()` instead of `Maybe.GetOrDefault()` for nullable reference types now
    - `Maybe.GetOrDefault()` called for nullable reference type produces a compiler warning, you must use explicit `Maybe.GetOrDefault(null!)` instead, it's a known compiler issue https://github.com/dotnet/roslyn/issues/40110
- Target language version is C# 8.0
- Run tests and build under .NET Core 3.1
