# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [0.2.0] - Unreleased

### Changed
- Add nullable annotations
- Target language version is C# 8.0
- `Maybe<T>` implements `IEquatable<Maybe<T>>` now
    - It already had non-interface `Equals` method before that
- Remove assembly signing
- Run tests and build under .NET Core 3.1
