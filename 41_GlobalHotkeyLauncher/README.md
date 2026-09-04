# グローバルホットキーランチャー

## 学習ポイント
RegisterHotKey(Win32 API)、メッセージフック、非アクティブ時のキー捕捉

## 概要
アプリが非アクティブ(バックグラウンド)な状態でも、登録したホットキー(修飾キー+キー)を押すと、対応する
コマンド(アプリ起動・URLを開く等)を実行するランチャー。

## 実装メモ
- **P/Invoke層**: `Win32HotKeyRegistrar`が`user32.dll`の`RegisterHotKey`/`UnregisterHotKey`を実際にラップする。
  登録には対象ウィンドウの`HWND`が必要なため、`MainWindow.OnSourceInitialized`(ウィンドウのハンドルが確定した
  タイミング)で生成する
- **テスト容易性**: `IHotKeyRegistrar`(`TryRegister(id, combination)`/`Unregister(id)`)で抽象化し、
  `MainViewModel`はこれに依存する。単体テストでは`FakeHotKeyRegistrar`を使い、実際のOSレベルの登録は行わない
- **ホットキーの組み合わせ**: `HotKeyCombination`(`record struct` Modifiers+Key)がUI非依存の値として存在し、
  `ToDisplayString()`(例: "Ctrl+Alt+L")と`Validate()`(修飾キー無し・キー未選択はエラー)を持つ。WPFの
  `ModifierKeys`のビット値はWin32の`MOD_ALT`/`MOD_CONTROL`/`MOD_SHIFT`/`MOD_WIN`と一致するため、そのまま
  キャストして使い、`KeyInterop.VirtualKeyFromKey`で仮想キーコードに変換する
- **重複登録・登録失敗の防止**: 追加前に一覧内の`HotKeyCombination`と値等価性で重複チェックする。重複時・
  OS側の登録失敗時(`TryRegister`がfalse。既に他アプリが同じホットキーを使用している場合など)はどちらも
  エラーメッセージを表示し、一覧には追加しない
- **コマンド実行**: `ProcessCommandLauncher`が`Process.Start(new ProcessStartInfo(target) { UseShellExecute = true })`
  で実行対象を起動する。`UseShellExecute=true`により、実行ファイルパス・URLのどちらも同じ方法で起動できる
- **メッセージフック**: `MainWindow.OnSourceInitialized`で`HwndSource.AddHook`により`WM_HOTKEY`(0x0312)を受信し、
  `wParam`のホットキーIDから`MainViewModel.HandleHotKeyTriggered(id)`を呼び出す。`RegisterHotKey`で登録した
  ホットキーはOSレベルでグローバルに捕捉されるため、ウィンドウがアクティブかどうかに関わらず動作する
  (`MOD_NOREPEAT`フラグにより、キーを押しっぱなしにしても連続発火しない)
- **編集**: 一覧の各行の「編集」ボタンで対象を`Unregister`・一覧から削除した上で入力欄に値を復元する
  (ユーザーが値を変更して再度「登録」を押すことで更新となる、削除+再登録に近いシンプルな設計)

## 動作確認(UI Automation)
- ホットキー(Ctrl+Alt+L → notepad.exe)を登録すると一覧に追加されることを確認
- **登録後、Calculatorを起動してフォーカスを奪った状態("GlobalHotkeyLauncherがアクティブでない"状態)で、
  `keybd_event`(Win32 API)により実際にCtrl+Alt+Lキー入力をシステムに送信したところ、実行ログに
  「実行: OpenNotepad (notepad.exe)」が記録され、実際にNotepadが起動することを確認**(アプリ非アクティブ時
  でもグローバルホットキーとして機能していることの実証)
- 同一の組み合わせを再度登録しようとするとエラーメッセージが表示され追加されないことを確認(単体テスト)
- 「編集」ボタンで対象ホットキーが解除・一覧から削除され、入力欄に元の値(修飾キー・キー・説明・実行対象)が
  復元されることを実機で確認
- 「削除」ボタンでホットキーが解除され一覧から削除されることを実機で確認

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
