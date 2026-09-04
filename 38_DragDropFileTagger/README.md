# ドラッグ&ドロップ ファイルタグ付けツール

## 学習ポイント
DragDrop.DoDragDrop、ドロップターゲットの実装、OSとのファイルドラッグ連携

## 概要
エクスプローラーからファイルをドラッグ&ドロップして取り込み、タグ付け・フィルタ表示・アプリ内での並び替えができる管理ツール。タグと並び順はJSONファイルへ永続化する。

## 実装メモ
- **OSとのファイルドラッグ連携**: `Window.AllowDrop=True`+`DragOver`/`Drop`イベントで、`e.Data.GetDataPresent(DataFormats.FileDrop)`によりエクスプローラーからのファイルドロップを検出し、`(string[])e.Data.GetData(DataFormats.FileDrop)`でパス一覧を取得する
- **アプリ内でのドラッグ並び替え(`DragDrop.DoDragDrop`)**: 一覧内のアイテムを`PreviewMouseLeftButtonDown`でドラッグ開始位置を記録し、`PreviewMouseMove`で`SystemParameters.MinimumHorizontalDragDistance`等の閾値を超えたら`DragDrop.DoDragDrop(item, file, DragDropEffects.Move)`を呼び出す。ドロップ先の`ListBoxItem.Drop`イベントで並び替えを実行する
- 並び替えの実際のロジック(リスト内移動+`SortOrder`再採番)は`TaggedFileReorderer`という純粋な静的クラスに切り出し、WPFのドラッグ&ドロップAPIに依存せず決定的にテストした
- タグの絞り込み(`TaggedFileFilter`)・ファイルサイズの表示整形(`FileSizeFormatter`)も同様に純粋なロジックとして分離しテストした
- タグ・並び順の永続化は`JsonTaggedFileRepository`(`System.Text.Json`)で行い、実際の一時ファイルへのSave→Loadでテストした
- `TaggedFile.Tags`は`List<string>`(非通知)のため、タグ追加後に`NotifyTagsChanged()`を明示的に呼び出して`TagsDisplay`(表示用の結合済み文字列)の変更をUIへ通知する設計にした
- **既知の制限(エクスプローラーからの実ドラッグのUI Automation検証)**: この環境のUI Automationスクリプトからは、別プロセス(エクスプローラー)を起点とする実際のOSレベルのファイルドラッグを再現できなかった。検証時のみ環境変数でファイルパスを指定し`MainViewModel.AddFiles`相当の取り込みを行う一時的なテストフックを追加し、動作確認後に削除した(`21_PaintTool`等の`SaveFileDialog`非描画問題と同様の環境制約)。ドロップイベントハンドラ自体は標準的なWPFの`DataFormats.FileDrop`パターンであり、実際の取り込みロジック(`MainViewModel.AddFiles`)は単体テストで検証済み
- **ハマった点(UI Automationスクリプトの日本語文字列比較)**: `36_AnimatedDashboard`と同様、検証用PowerShellスクリプト内の日本語タグ文字列を`ValuePattern.SetValue`で設定すると、`-eq`比較が正しく機能しないことがあった。検証用の文字列はASCII("TagA"等)に統一することで回避した

## 動作確認(UI Automation)
- (テストフック経由で)3件のファイルを取り込み、一覧に表示されることを確認
- 1件を選択してタグを追加すると、一覧にタグが表示されることを確認
- タグで絞り込むと該当ファイルのみ表示され、絞り込み解除で全件に戻ることを確認
- 一覧内でマウスドラッグ(Win32 `mouse_event`でシミュレーション)して並び替えると、`invoice.pdf,photo.jpg,report.txt` → `photo.jpg,report.txt,invoice.pdf`のように順序が入れ替わることを確認
- ファイルを削除すると一覧から消えることを確認

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
