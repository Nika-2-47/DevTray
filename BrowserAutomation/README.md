# Browser Automation Tool

汎用的なブラウザ自動化ツールです。YAMLファイルで手順を定義し、Playwrightを使用してブラウザ操作を自動化します。

## 必要要件

- Python 3.8+
- Playwright

## セットアップ

1. 依存関係のインストール:
   ```bash
   pip install -r requirements.txt
   playwright install
   ```

2. 環境変数の設定:
   `.env.example` をコピーして `.env` を作成し、ログイン情報などを設定してください。
   ```bash
   cp .env.example .env
   ```

## 使い方

1. `config/sites.yaml` に自動化したいサイトの手順を記述します。

2. スクリプトを実行します:
   ```bash
   python automator.py <site_name> [config_file]
   ```

   例:
   ```bash
   python automator.py attendance
   ```

## 設定ファイルの書き方 (YAML)

`config/sites.yaml` の例:

```yaml
sites:
  mysite:
    browser: chromium # chromium, firefox, webkit
    headless: false   # true にするとブラウザを表示せずに実行
    steps:
      - description: "ページを開く"
        action: goto
        target: "https://example.com"
      
      - description: "入力"
        action: fill
        target: "#username"
        value: "$MY_USER" # $で始めると環境変数を参照
      
      - description: "クリック"
        action: click
        target: "button.submit"
```

### サポートしているアクション

- `goto`: URLを開く (target: URL)
- `fill`: テキストを入力 (target: セレクタ, value: 入力値)
- `click`: クリック (target: セレクタ)
- `press`: キー入力 (target: セレクタ, value: キー名 e.g., "Enter")
- `wait`: 待機 (value: 秒数 または セレクタ)
- `screenshot`: スクリーンショット (value: ファイルパス)
- `select_option`: プルダウン選択 (target: セレクタ, value: 選択値)
- `check` / `uncheck`: チェックボックス操作 (target: セレクタ)
