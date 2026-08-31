# システムトレイ常駐ユーティリティ

## 学習ポイント
NotifyIcon、コンテキストメニュー、バルーン通知、常駐アプリのライフサイクル管理

## 概要
タスクトレイに常駐し、右クリックメニューやバルーン通知から簡易機能を提供するユーティリティアプリ。ウィンドウを閉じてもアプリは終了せずトレイに常駐し続け、常駐中の簡易機能として「定期リマインダー」(指定間隔ごとにバルーン通知)を実装した。

## 実装メモ
- **NotifyIcon**: WPFにはタスクトレイアイコンの標準APIが無いため、`System.Windows.Forms.NotifyIcon`をWinForms相互運用(`<UseWindowsForms>true</UseWindowsForms>`を追加)で使用した。WPFとWinFormsを同一プロジェクトで有効にすると`Application`型が`System.Windows.Application`/`System.Windows.Forms.Application`の両方で曖昧になるため、`using Application = System.Windows.Application;`で明示的にエイリアスした
- **常駐アプリのライフサイクル管理**: `App.xaml`に`ShutdownMode="OnExplicitShutdown"`を指定し、`MainWindow.Closing`イベントで`e.Cancel = true; Hide();`することでウィンドウを閉じてもアプリ自体は終了しないようにした。実際の終了はトレイメニューの「終了」からのみ行え、`_notifyIcon.Dispose()`でトレイアイコンを消してから`Application.Current.Shutdown()`を呼ぶ
- **コンテキストメニュー**: `NotifyIcon.ContextMenuStrip`に`System.Windows.Forms.ContextMenuStrip`(「開く」「今すぐ通知」「終了」)を設定するだけで、右クリック時に自動的に表示される
- **バルーン通知**: `NotifyIcon.ShowBalloonTip(timeout)`(事前に`BalloonTipTitle`/`BalloonTipText`を設定)を使用。定期リマインダーは`DispatcherTimer`(View側で保持、`24_MusicPlayer`等と同じ方針)で一定間隔ごとに呼び出す
- **スタートアップ登録**: `IStartupRegistrar`で抽象化し、実装(`RegistryStartupRegistrar`)は`HKCU\Software\Microsoft\Windows\CurrentVersion\Run`への値の設定/削除で行う。単体テストは実際のHKCUレジストリキーに対して行うが、テスト専用の一意な値名を使い、テスト後に必ず削除することで実機の環境を汚さないようにした(`SQLite`の実ファイルI/Oテスト等、本リポジトリで一貫している「モックより実I/O」の方針をレジストリにも適用した)
- リマインダー間隔の入力検証(`ReminderIntervalParser`)はWPF/タイマーに依存しない純粋なロジックとして分離しテストした
- **UI Automationでの検証範囲**: `NotifyIcon`/`ContextMenuStrip`はタスクトレイ上のWinForms要素であり、WPFのUI Automationツリーからは検証できない。そのため、メイン画面に「テスト通知を表示」ボタンを設け、バルーン通知の呼び出しが例外なく実行できることを確認する形で検証した。また、UI Automationでの動作確認中に実機のスタートアップ登録(実レジストリ)を汚さないよう、検証時のみインメモリのフェイクレジストラ(`InMemoryStartupRegistrar`)に一時的に差し替えて確認し、検証後に本来の`RegistryStartupRegistrar`に戻した(実際のレジストリを使う構成のままウィンドウを閉じても常駐が継続することは別途確認済み)

## 動作確認(UI Automation)
- ウィンドウを閉じても、プロセスが終了せずタスクトレイに常駐し続けることを実機で確認(`Process.HasExited`がfalseのまま)
- 「テスト通知を表示」ボタンでバルーン通知の呼び出しが例外なく実行できることを確認
- 定期リマインダーの間隔・メッセージ設定、スタートアップ登録チェックボックスの切り替えでステータステキストが正しく更新されることを確認(フェイクレジストラ使用時)

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
