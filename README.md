# CounterStrikeSharp Plugin

[![Discord](https://img.shields.io/discord/1296858234853789826?style=for-the-badge&label=Join%20the%20Discord%20Server&link=https%3A%2F%2Fdiscord.gg%2FZaEwHfDD5N)](https://discord.gg/ZaEwHfDD5N)

CounterStrikeSharp plugin for LIGA Esports Manager.

## APIs and Technologies

- [CounterStrikeSharp](https://docs.cssharp.dev/) `v1.0.320`
- [Metamod Source](https://www.sourcemm.net/) `v2.0.0-git1359`
- [Visual Studio Redistributables](https://aka.ms/vs/17/release/vc_redist.x64.exe)
- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Prerequisites

1. Create a `generated` folder in the root of the project.
2. Download [CounterStrikeSharp](https://docs.cssharp.dev/) and [Metamod Source](https://www.sourcemm.net/) and extract their contents into the `generated` folder.
3. Ensure [Visual Studio Redistributables](https://aka.ms/vs/17/release/vc_redist.x64.exe) and [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) are installed on your system.

## Getting Started

```bash
dotnet restore
dotnet build -c release
```
