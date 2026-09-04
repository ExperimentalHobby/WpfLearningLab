# 単一インスタンス起動ランチャー

## 学習ポイント
Mutexによる多重起動防止、Named Pipesによるプロセス間通信(IPC)、起動引数の受け渡し

## 概要
`Mutex` で多重起動を防止し、2個目以降の起動引数を `Named Pipes` 経由で既存インスタンスへ送信、受信時にウィンドウを前面表示するランチャーアプリ。

## 実装メモ
- 起動引数のメッセージ(`LaunchMessage`)のJSON変換ロジックを `LaunchMessageSerializer` に分離し、xUnitでTDD(Red→Green→Refactor)で実装した
- `SingleInstanceGuard` は名前付き `Mutex`(`Local\WpfLearningLab.SingleInstanceLauncher.Mutex`)を使い、`Mutex` のコンストラクタが返す `createdNew` で自分が最初のインスタンスかどうかを判定する
- `PipeMessenger` は `NamedPipeServerStream`/`NamedPipeClientStream` をラップし、メッセージを1行のJSONとして送受信する(`StreamReader.ReadLine`で行単位に区切るため、シリアライズしたJSONは改行を含まない1行として扱う)
- `App.xaml.cs` の `OnStartup` で多重起動を判定する。2個目以降の場合はウィンドウを生成せず、`PipeMessenger.SendMessage` で起動引数を送信してすぐに `Shutdown()` する。最初のインスタンスの場合は通常通り起動し、`PipeMessenger.StartServerAsync` をバックグラウンドで開始して以後の起動要求を待ち受ける
- メッセージ受信時は `Dispatcher.Invoke` でUIスレッドに戻してから履歴一覧に追加し、`Topmost` を一度trueにしてfalseに戻す一般的な手法でウィンドウを前面表示している
- UI Automationで実機を操作し、(1) 引数付きで2個目のプロセスを起動しても多重起動せず5秒以内に正常終了すること(生存プロセスが1つのみ)、(2) 最初のインスタンスの受信履歴に2個目の起動引数が反映されること、(3) 受信時にウィンドウが前面表示されることを確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
