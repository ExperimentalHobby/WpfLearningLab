# ファイル整理ツール

## 学習ポイント
ディレクトリ監視(FileSystemWatcher)、非同期I/O

## 概要
指定した監視フォルダ直下を`FileSystemWatcher`で監視し、拡張子ごとの振り分けルール(例: `.jpg` → `Images`)に従ってファイルを自動的にサブフォルダへ移動するツール。既存ファイルの一括整理と、処理結果のログ表示にも対応する。

## 実装メモ
- `FileSortingRuleMatcher`(拡張子→移動先フォルダ名を判定する純粋ロジック、大文字小文字を区別しない)、`IFileOrganizerService`/`FileOrganizerService`(実際の`File.Move`によるファイル移動)、`IDirectoryWatcher`/`FileSystemDirectoryWatcher`(実際の`FileSystemWatcher`のラップ)の3層に分離した
- `FileOrganizerService`・`FileSystemDirectoryWatcher`はいずれも高速・決定的なファイルシステム操作のため、モックを使わず実の一時フォルダ(`Path.GetTempPath()`配下)に対してテストした。`FileSystemDirectoryWatcher`のテストのみ`TaskCompletionSource`+タイムアウト付き`Task.WhenAny`で実際のイベント発火を待つ非同期テストとした
- 移動先に同名ファイルが既に存在する場合、`File.Move`は`IOException`を投げるが、これをcatchして`OrganizeResult.ErrorMessage`に記録し、アプリ全体をクラッシュさせないようにした
- `FileSystemWatcher`のイベントはバックグラウンドスレッドで発火するため、`16_LocalChatApp`と同じ`IUiDispatcher`抽象を再利用し、`ObservableCollection<OrganizeResult> Logs`への追加のみUIスレッドにマーシャリングした
- `MainViewModel.OnFileCreated`(watcherイベントハンドラ)・`OrganizeExistingAsync`(一括整理)はともに、振り分け結果が「移動できた」または「エラーが発生した」場合のみ`Logs`に記録し、「一致するルールが無く何もしなかった」場合は記録しない(`ShouldLog`ヘルパーで共通化)。無関係なファイル作成のたびにログが埋まらないようにする意図
- フォルダ選択は既存アプリと同じ`IFolderPicker`(`OpenFolderDialog`)抽象を使う。**この環境ではUI Automationからのクリックで`OpenFolderDialog`が描画されない**(`21_PaintTool`の`SaveFileDialog`、`24_MusicPlayer`の`OpenFolderDialog`と同じ既知の制限)ため、動作確認時のみ環境変数で切り替わる一時的なフェイク`IFolderPicker`を注入するデバッグフックを使い、確認後にコードを削除した
- UI Automationで実機を操作し、以下を確認済み:
  - フォルダ選択後、振り分けルール(`.jpg → Images`、`.pdf → Documents`)を2件追加できること
  - 「既存ファイルを一括整理」実行で、監視フォルダ直下の既存ファイルが正しいサブフォルダへ移動し、処理ログに2件記録されること
  - 監視開始後に新規ファイルを作成すると、自動的に振り分けられ処理ログに追加されること(`FileSystemWatcher`のイベントが実際に発火し、`IUiDispatcher`経由でUIに反映されることを確認)
  - 監視停止・ルール削除が正しく反映されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
