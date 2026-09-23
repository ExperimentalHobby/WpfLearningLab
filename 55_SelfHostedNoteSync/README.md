# 自作HTTPサーバー メモ同期アプリ

## 学習ポイント
HttpListenerで自作HTTPサーバー、HttpClientで外部API呼び出し、async/await、クライアント/サーバー構成

## 概要
`System.Net.HttpListener` で実装した自作の最小HTTPサーバーがメモをJSON APIとして公開し、WPFクライアントが `HttpClient` でCRUD操作を行うメモ同期アプリ。`55_SelfHostedNoteSync/Server` がサーバー(コンソールアプリ)、`55_SelfHostedNoteSync/Client` がWPFクライアント。

## 実装メモ
- サーバー(`NoteHttpServer`)は`HttpListener`のリクエスト受付ループと、ルーティング・JSONハンドリングを行う`NoteApiHandler`を分離した。`NoteApiHandler`は`HttpListener`に依存しない`ApiRequest`/`ApiResponse`という単純なレコード型でやり取りするため、実際にHTTP通信を起動せずルーティングロジックをユニットテストできる
- サーバーの永続化は`NoteStore`(ロックで排他制御した`Dictionary`)によるプロセス内メモリのみ。学習用のため永続化(DB/ファイル)は対象外
- **見つけて直したバグ1: 文字化け** — `HttpListenerRequest.ContentEncoding`はContent-Typeヘッダーにcharset指定が無い場合、環境依存のエンコーディング(既定コードページ)にフォールバックする。クライアント(`HttpClient`)はcharset無しでUTF-8ボディを送るため、このままだと日本語が文字化けした。`Encoding.UTF8`を明示指定して読み取るよう修正し、実際にHTTPListenerを起動してHttpClientと通信する結合テスト(`NoteHttpServerTests`)で日本語の往復を検証した
- **見つけて直したバグ2: 更新後に削除ボタンが反応しない** — `MainViewModel.UpdateAsync`で`Notes[index] = updated`と新しい参照に置き換えると、ListBoxの`SelectedItem`バインディングが古い参照を見失い選択解除される問題があったため、更新後に`SelectedNote = updated`と明示的に選択を維持するよう修正。さらに、この修正の過程で`IsBusy`の変更が`AddCommand`/`UpdateCommand`/`DeleteCommand`/`LoadCommand`の`CanExecuteChanged`に通知されておらず、通信中(`IsBusy=true`)のタイミングでたまたま発火した`CanExecuteChanged`の判定結果のままボタンの有効/無効が固まってしまう問題も見つかったため、`IsBusy`のsetterで全コマンドに通知するよう修正した。どちらもユニットテストだけでは検出できず、実機でのUI操作(追加→選択→更新→削除)で発見した
- クライアント(`NoteApiClient`)は`HttpClient`をコンストラクタ注入可能にし、テストでは実ネットワーク通信を行わず疑似`HttpMessageHandler`でJSON応答を差し替えて検証した。加えて送信されたリクエストのメソッド・URL・ボディも検証し、GET/POST/PUT/DELETEが正しいエンドポイントに送られることを確認している
- エラーハンドリング: `HttpRequestException`/`TaskCanceledException`/`JsonException`(作成・更新時は`InvalidOperationException`も)を捕捉し、`ErrorMessage`に日本語メッセージを設定する。サーバー未起動時は接続確立に数秒かかることがある(Windowsの`localhost`名前解決でIPv6→IPv4のフォールバックが発生するため)が、最終的にエラーメッセージが表示されることを実機で確認した
- UI Automationで実機を操作し、以下を確認済み:
  - サーバー・クライアントを起動し、メモの追加→一覧反映→選択→タイトル/本文編集→更新→一覧反映→削除→一覧から消えることを一連の操作で確認
  - サーバーを停止した状態で「再読み込み」を押すとエラーメッセージが表示されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
