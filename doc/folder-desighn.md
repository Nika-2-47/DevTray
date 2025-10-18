✅ Shared に入れるべきもの

特徴

複数プロジェクト・ツールで共通利用できる

他の層に依存していない（独立して動く）

ビジネスルールを持たないユーティリティやインフラ補助

具体例
種類	例	備考
共通ユーティリティ	StringHelper, DateTimeHelper, EnumerableExtensions	汎用関数
共通定数	AppConstants, ErrorCodes	固定値・識別子
共通例外	DomainException, ApplicationException	例外クラス
共通ロギング	ILogger, ConsoleLogger	インターフェース＋簡易実装
設定管理ラッパー	ConfigLoader, AppSettings	JSONや環境変数の読み込み
外部ライブラリラッパー	JsonSerializerWrapper, HttpClientFactoryWrapper	外部API・ライブラリ依存の吸収

⚡ ポイント

Domain / Application / Infrastructure どの層からも参照可能

Shared 内では他の層に依存しない

❌ Shared に入れてはいけないもの

特徴

特定ツール・プロジェクトに依存する

ビジネスルールやユースケースを持つ

具体例
種類	例	理由
ドメインエンティティ	Task, Email	ビジネスルールがあるため、Domain層に置く
ユースケース / Applicationサービス	SendEmailUseCase	Application層に置くべき
Infrastructure 実装	SmtpEmailSender, SqlUserRepository	Applicationのインターフェースに依存するため Shared ではない
UI コード	ConsoleApp / WebApp	Presentation層専用

⚡ ポイント

Shared は「共通補助」

「ツール固有・ビジネスルール・DB/API依存」は絶対に入れない

🧩 まとめ
Shared: 全プロジェクトで共通して使える “純粋な補助/ライブラリ”
Domain: ビジネスルール
Application: ユースケース
Infrastructure: 外部アクセス
Presentation: UI


目安：「これを削除しても業務の意味は変