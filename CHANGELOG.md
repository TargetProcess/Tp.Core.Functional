# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [1.0.0] - Unreleased

### Breaking changes
- `Maybe<T>` implements `IEquatable<Maybe<T>>` now
    - It already had non-interface `Equals` method before that
- Remove assembly signing
- Some utility methods are moved from `Try` instance to `TryExtensions` for better interop with nullable types

### Changed
- Add nullable annotations
- Target language version is C# 8.0
- Target platform is netstandard2.0
- Run tests and build under .NET Core 3.1
