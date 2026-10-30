# CI/CD・GitHub Actions 学習ロードマップ

題材: MemoPad（ASP.NET Core Web API + .NET MAUI Blazor Hybrid）

## 進め方

- 各ステップの「ゴール」を満たしたらチェックを入れ、末尾の「進捗ログ」に日付とメモを残す。
- 次回の学習再開時は「現在地」を見て続きから始める。
- 各ステップで学ぶ GitHub Actions の概念を【】で示す。

## 現在地

- **ステップ: 1（進行中）**
- 最終更新: 2026-10-30

---

## フェーズ 1: GitHub Actions の基本

### ステップ 1: 最初のワークフロー
【ワークフロー / トリガー / ジョブ / ステップ / ランナー】
- [ ] `.github/workflows/` に `echo` するだけのワークフローを置き、`push` で起動させる
- [ ] Actions タブで実行ログを読み、ジョブ・ステップの構造を理解する
- [ ] `workflow_dispatch` で手動実行できるようにする

### ステップ 2: Web API のビルドとテスト（CI の基本形）
【`actions/checkout` / `actions/setup-dotnet` / `global.json` / 終了コードによる失敗判定】
- [ ] `ubuntu-latest` で `MemoPad.Api` と `MemoPad.Shared` をビルド
- [ ] `tests/MemoPad.Api.Tests` を `dotnet test` で実行
- [ ] わざとテストを失敗させ、ワークフローが赤になることを確認する
- [ ] `MemoPad.slnx` 全体ビルドが Linux で失敗する理由（MemoPad.App の TFM が空）を説明できる

### ステップ 3: Pull Request と連携する
【`pull_request` トリガー / パスフィルタ / ブランチ保護 / ステータスチェック】
- [ ] `pull_request` で CI が走るようにする
- [ ] `paths` フィルタで、関係するファイル変更時だけ動かす
- [ ] ブランチ保護ルールで「CI 成功を必須」にし、PR からマージする流れを体験する

## フェーズ 2: CI を育てる

### ステップ 4: 複数ジョブと依存関係
【`jobs.<id>.needs` / 並列実行 / ジョブ間の独立性】
- [ ] Api テストと App.UI（bUnit）テストを別ジョブに分ける
- [ ] `needs` で後続ジョブを依存させる

### ステップ 5: キャッシュと高速化
【`actions/cache` / `setup-dotnet` のキャッシュ / キャッシュキー設計】
- [ ] NuGet パッケージをキャッシュし、2 回目以降の実行時間を比較する
- [ ] `packages.lock.json` とキャッシュキーの関係を理解する

### ステップ 6: アーティファクト
【`actions/upload-artifact` / `download-artifact` / ジョブ間の受け渡し】
- [ ] テスト結果（TRX）とカバレッジをアーティファクトとして保存する
- [ ] `dotnet publish` した API をアーティファクト化し、別ジョブで取り出す

### ステップ 7: マトリクスビルド
【`strategy.matrix` / `fail-fast` / `include` / `exclude` / OS ごとの差異】
- [ ] ubuntu / windows / macos でテストをマトリクス実行する
- [ ] OS ごとにステップを `if:` で出し分ける

## フェーズ 3: MAUI（クライアント）の CI

### ステップ 8: Mac Catalyst ビルド
【`macos-latest` ランナー / `dotnet workload install maui` / Xcode バージョン選択】
- [ ] `macos-latest` で `MemoPad.App` を `net10.0-maccatalyst` 向けにビルドする
- [ ] ランナーの Xcode バージョンと MAUI ワークロードの対応を確認・固定する
- [ ] macOS ランナーの課金倍率（分数消費）を把握する

### ステップ 9: Windows ビルド
【`windows-latest` ランナー / ランナーごとの shell の違い】
- [ ] `windows-latest` で `net10.0-windows10.0.19041.0` 向けにビルドする
- [ ] Mac Catalyst と Windows をマトリクスまたは別ジョブで構成する

## フェーズ 4: 再利用と品質

### ステップ 10: 再利用可能な構成
【Reusable workflow（`workflow_call`）/ Composite action】
- [ ] .NET セットアップ処理を Composite action にまとめる
- [ ] ビルド・テストを Reusable workflow に切り出す

### ステップ 11: 品質ゲート
【静的解析 / フォーマットチェック / `concurrency` / Dependabot】
- [ ] `dotnet format --verify-no-changes` を CI に組み込む
- [ ] `concurrency` で古い実行をキャンセルする
- [ ] Dependabot で NuGet と Actions のバージョン更新 PR を受け取る

## フェーズ 5: CD（デリバリー・デプロイ）

### ステップ 12: シークレットと環境
【Secrets / Variables / Environments / 承認ルール / `GITHUB_TOKEN` の権限】
- [ ] リポジトリシークレットを使うワークフローを書く（ログでマスクされることを確認）
- [ ] `staging` / `production` の Environment を作り、承認を必須にする
- [ ] `permissions:` で `GITHUB_TOKEN` を最小権限にする

### ステップ 13: コンテナイメージの公開
【Docker ビルド / GitHub Container Registry (ghcr.io) / タグ戦略】
- [ ] Web API の Docker イメージをビルドして ghcr.io に push する
- [ ] コミット SHA とバージョンタグでイメージにタグ付けする

### ステップ 14: クラウドへのデプロイ
【OIDC によるクラウド認証 / デプロイジョブ / Environment 連携】
- [ ] デプロイ先を決める（例: Azure App Service / Azure Container Apps）
- [ ] OIDC（シークレットなし）でクラウドにログインする
- [ ] main マージで staging、承認後に production へデプロイする

### ステップ 15: リリース自動化
【タグトリガー / GitHub Releases / バージョニング】
- [ ] `v*` タグの push でリリースワークフローを起動する
- [ ] MAUI アプリ（Mac Catalyst / Windows）の成果物を GitHub Release に添付する
- [ ] （発展）コード署名・公証の仕組みを調べる

---

## 進捗ログ

| 日付 | ステップ | メモ |
| --- | --- | --- |
| 2026-10-30 | 0 | ロードマップ作成 |
