# Claude APIチャットクライアント

## 学習ポイント
Claude API(Messages API)のストリーミング応答、`IAsyncEnumerable`による非同期ストリーム処理、
APIキーの安全な保管([17_PasswordManager](../17_PasswordManager/README.md)の暗号化保管の再利用)

## 概要
Claude APIのストリーミング応答を`IAsyncEnumerable`で受信し、逐次描画するチャットクライアント。

## 実装メモ
- **ストリーミング受信**: `ClaudeApiClient.StreamMessageAsync`が`HttpClient`で
  `POST https://api.anthropic.com/v1/messages`(`stream: true`)を送信し、`HttpCompletionOption.ResponseHeadersRead`
  でヘッダー受信時点から読み取りを開始する。応答本文(SSE)は`SseStreamParser`に渡す
- **SSE解析**: `SseStreamParser`はClaude Messages APIの実際のイベント形式(`event: content_block_delta` /
  `data: {...}`のペアが空行区切りで続く)を解析し、`content_block_delta`の`text_delta`のみを`yield return`する。
  `message_stop`イベントで列挙を終了し、`event: error`ではメッセージを持つ`ClaudeApiException`をスローする。
  ストリーム終端(EOF)直前のイベントは末尾に空行がないままEOFに達することがあるため、EOF時にも未処理データを
  処理してから終了するようにしている(この考慮漏れがテストで発覚し、修正した)
- **エラーハンドリング**: HTTPステータス401は「APIキーが無効です」、429は「レート制限を超えました」という
  メッセージの`ClaudeApiException`に変換する。`SendAsync`側で`OperationCanceledException`/
  `ClaudeApiException`をキャッチしてエラーメッセージを表示する
- **APIキー保管**: `AesApiKeyCryptoService`(AES-GCM + PBKDF2、[17_PasswordManager](../17_PasswordManager/README.md)の
  `AesPasswordCryptoService`と同等ロジックをコピーして再利用)でAPIキーを暗号化し、`FileApiKeyStore`で
  `%APPDATA%\ClaudeChatClient\apikey.json`に保存する。マスターパスワードの正誤判定は、既知の固定文字列を
  暗号化した検証用値を復号し、一致するかで行う([17_PasswordManager](../17_PasswordManager/README.md)と同じ方式)
- **逐次描画**: `MainViewModel.SendAsync`はUserメッセージ追加後、空のAssistantメッセージを追加し、
  `StreamMessageAsync`から届くテキスト差分を`StringBuilder`に蓄積しながら、そのつど
  `Messages[assistantIndex] = new ChatMessage(...)`で置き換える(`ObservableCollection`の`Replace`通知により
  バインディング先のUIも逐次更新される)
- **キャンセル**: `CancelCommand`は送信中の`CancellationTokenSource`をキャンセルする。ユニットテストでは、
  フェイクのAPIクライアントに`await Task.Yield()`を挟ませることで、送信開始直後にテストコードから
  確実にキャンセルを割り込ませ、決定的に検証している
- **テスト方針**: ユニットテストは全て`HttpMessageHandler`をフェイクしてHTTP通信をモックし、実際にAnthropicの
  サーバーへは接続しない。実機動作確認のみ、ダミーのAPIキーで実際にエンドポイントへ接続し、401エラーハンドリング
  (実運用と同じコードパスでの疎通確認)を行った

## 動作確認(UI Automation)
- 初回起動でマスターパスワード・APIキー入力欄が表示されることを確認
- マスターパスワード・ダミーAPIキーを入力して「開始」を押すと、ロック画面からチャット画面に遷移することを確認
- メッセージ送信すると、Userメッセージが即座に履歴に追加され、その後実際にAPIへの接続(ダミーキーのため401)が
  発生し、「APIキーが無効です(認証エラー)。設定を確認してください。」がエラー表示されることを確認
  (スクリーンショット取得済み。ユニットテストはHTTPをモックしているが、この実機確認のみ実際にAnthropicの
  APIエンドポイントへ接続し、認証エラー時の実際の応答コードでハンドリングが動作することを確認した)
- 実際のAPIキーでの正常な会話・ストリーミング表示の疎通確認は、ユーザー自身の手元のAPIキーで行う想定

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
