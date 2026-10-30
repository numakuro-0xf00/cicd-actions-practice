# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## リポジトリの目的

CI/CD と GitHub Actions を学習するためのリポジトリ。ユーザーは C# 開発に慣れているため、題材となるサンプルプロジェクトは以下で構成する想定:

- .NET MAUI Blazor Hybrid アプリ（クライアント。開発機の macOS では Mac Catalyst で実行確認する）
- ASP.NET Core Web API

開発環境は macOS (arm64) で、.NET 10 SDK を使用する。

現時点ではリポジトリは空（コミットなし）で、ソリューション・プロジェクト・ワークフローはまだ存在しない。

## 作業ルール（重要）

- **学習が主目的のため、ユーザーから明示的に指示がない限りファイルの作成・編集・削除は行わない。** 説明、コード例の提示、手順の解説に留め、実際の変更はユーザーの指示を待つこと。
- 説明は CI/CD・GitHub Actions の概念（トリガー、ジョブ、ランナー、アーティファクト、シークレット、環境、マトリクスなど）と結びつけて行い、C# / .NET の知識を前提にしてよい。

## CI 上の注意点

- MAUI Blazor Hybrid はターゲットごとにランナーが異なる: Mac Catalyst (`net10.0-maccatalyst`) は `macos-latest`、Windows (`net10.0-windows10.0.xxxxx`) は `windows-latest`。いずれも `dotnet workload install maui` が必要。ASP.NET Core Web API は `ubuntu-latest` で動かせる。
- Mac Catalyst のビルドは Xcode のバージョンと MAUI ワークロードの対応関係に依存する（ローカル・CI とも）。
- ワークフローは `.github/workflows/*.yml` に配置する。

## コマンド

```sh
dotnet build MemoPad.slnx                     # 全体ビルド（macOS では MemoPad.App は Mac Catalyst 向けにビルドされる）
dotnet test MemoPad.slnx                      # 全テスト
dotnet test tests/MemoPad.Api.Tests --filter "FullyQualifiedName~WeatherForecastControllerTests.Get_ReturnsOk"   # 単一テスト
dotnet run --project src/MemoPad.Api          # API 起動
dotnet build src/MemoPad.App -t:Run -f net10.0-maccatalyst   # MAUI アプリを Mac で起動
```

## 構成

ソリューション `MemoPad.slnx`（題材はメモ帳アプリ）。SDK は `global.json` で 10.0.403（latestFeature）に固定。

- `src/MemoPad.Api` — ASP.NET Core Web API（コントローラー方式）。メモの保存は当面インメモリ。
- `src/MemoPad.Shared` — DTO・API 契約。Api と App.UI の両方が参照する。
- `src/MemoPad.App.UI` — Razor Class Library。画面（Pages/Layout/Routes.razor）と API クライアントはここに置く。
- `src/MemoPad.App` — MAUI Blazor Hybrid のホスト。`MainPage.xaml` の BlazorWebView が `MemoPad.App.UI.Routes` を表示するだけの薄い層。ターゲットは macOS では `net10.0-maccatalyst`、Windows では `net10.0-windows10.0.19041.0` のみ（Android/iOS は除外済み）。
- `tests/MemoPad.Api.Tests` — xUnit + `WebApplicationFactory<Program>` による結合テスト。
- `tests/MemoPad.App.UI.Tests` — xUnit + bUnit（v2 API: `BunitContext` / `Render<T>()`）。

MAUI 以外のプロジェクトとテストはすべて `net10.0` なので Linux でビルド・テストできる。一方 `MemoPad.App` は Linux ではターゲットフレームワークが空になるため、`ubuntu-latest` で `MemoPad.slnx` 全体をビルドすると失敗する。Linux のジョブではプロジェクトを個別指定するなどして MemoPad.App を除外すること。

`Directory.Build.props` で Mac Catalyst ビルド時のみ `ValidateXcodeVersion=false` を設定している（ローカルの Xcode 27.2 が MAUI ワークロードの想定する 27.1 より新しいため）。
