# 簡易IDE/コードエディタ

## 学習ポイント
AvalonEdit連携、シンタックスハイライト

## 概要
`AvalonEdit`(`ICSharpCode.AvalonEdit`)を使ったシンタックスハイライト付きの簡易コードエディタ。ファイルの拡張子に応じて自動でシンタックスハイライトが切り替わり、行番号表示・折り返し設定にも対応する。

## 実装メモ
- `21_PaintTool`の`IInkCanvasController`、`24_MusicPlayer`の`IMediaPlayerController`と同じ方針で、実の`AvalonEdit.TextEditor`を`IEditorController`/`AvalonEditController`でラップした。AvalonEditの`Text`プロパティは(`WordWrap`や`ShowLineNumbers`と異なり)`DependencyProperty`ではない単純なCLRプロパティのため、XAMLで直接`Text="{Binding ...}"`のような双方向バインディングができない。そのため`IEditorController`経由でOpen/Save/New時のみ明示的にget/setする設計にした(エディタ上での通常の編集はAvalonEdit自身が管理し、ViewModel側に逐次同期する必要は無い)
- シンタックスハイライトは`HighlightingManager.Instance.GetDefinitionByExtension(拡張子)`でファイル拡張子から自動判定する
- `IFileService`/`FileService`が実の`File.ReadAllText`/`WriteAllText`をラップし、実の一時フォルダに対してテストした
- ファイル選択ダイアログは`IFileDialogService`/`Win32FileDialogService`(`OpenFileDialog`/`SaveFileDialog`)抽象を使う。**この環境ではUI Automationからのクリックで`OpenFileDialog`/`SaveFileDialog`が描画されない**(`21_PaintTool`の`SaveFileDialog`、`24_MusicPlayer`の`OpenFolderDialog`、`25_FileOrganizer`の`OpenFolderDialog`と同じ既知の制限)ため、動作確認時のみ環境変数で切り替わる一時的なフェイク`IFileDialogService`を注入するデバッグフックを使い、確認後にコードを削除した
- UI Automationで実機を操作し、以下を確認済み:
  - C#ファイルを開くと、`using`/`namespace`/`class`等のキーワードや文字列リテラルに拡張子(`.cs`)に応じたシンタックスハイライトが適用されること、行番号が表示されること
  - 「折り返し」チェックボックスをクリックするとAvalonEditの`WordWrap`が実際に切り替わること
  - 「名前を付けて保存」で、開いていた内容がそのまま指定パスへ保存されること
  - 「新規」でエディタの内容・現在のファイルパスがクリアされること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
