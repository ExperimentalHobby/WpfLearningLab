# RSSリーダー

## 学習ポイント
XML解析、非同期通信、ListView表示

## 概要
指定したRSSフィードURLから記事一覧を取得して表示するリーダーアプリ。

## 実装メモ
- `RssFeedClient`は`HttpClient`をコンストラクタ注入可能にし、`System.Xml.Linq`(`XDocument`)でRSS 2.0の`channel/item`要素(title/description/link/pubDate)をパースする。テストでは疑似`HttpMessageHandler`でXML応答を差し替え、実ネットワーク通信なしに確定的に検証した
- `pubDate`は`DateTimeOffset.TryParse`で吸収し、欠如や形式のばらつきがあっても例外にせず`null`として扱う
- MVVM基盤(`ObservableObject`)は他アプリと同様のパターンを自前実装。コマンドは`AsyncRelayCommand`(フィード取得、多重実行防止)と`RelayCommand<T>`(記事のリンクを開く、型付きパラメータ)の2種類を使い分けた
- 「ブラウザで開く」の実際の起動処理(`Process.Start`)は`IBrowserLauncher`という抽象を介して呼び出す設計にした。ViewModelのテストでは実際にブラウザを起動せず、渡されたURLだけを検証できる
- エラーハンドリング: 不正なURL/HTTP通信失敗/RSS(XML)パース失敗のいずれも`ErrorMessage`に表示。再取得時は前回のエラー表示をクリアする
- ListViewは`GridView`でタイトル・日時の列を表示し、記事選択(`SelectedArticle`)時に右側の詳細ペインで概要を表示する
- UI Automationで実機を操作し、実際にBBC NewsのRSSフィード(`https://feeds.bbci.co.uk/news/rss.xml`)を取得して以下を確認済み:
  - 記事一覧(21件)が正しく表示されること
  - 記事選択で詳細(タイトル・概要)が表示され、「ブラウザで開く」ボタンが有効化されること
  - 存在しないドメインを指定するとエラーメッセージが表示されること
  - URL欄が空欄の場合、取得ボタンが無効化されること
- 検証時の気づき: WPFの`ListView`(`GridView`)の行は、UI AutomationのControlTypeが`ListItem`ではなく`DataItem`として公開される。要素検索条件を`ListItem`にしたところ0件になり、`DataItem`に修正して正しく検出できた

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
