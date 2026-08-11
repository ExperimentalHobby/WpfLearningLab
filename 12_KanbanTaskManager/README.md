# タスク管理(カンバン風)

## 学習ポイント
MVVM+ICommand、Drag&Drop

## 概要
未着手/対応中/完了の3カラムでタスクをドラッグ&ドロップ管理できるカンバンボード。

## 実装メモ
- MVVM基盤(`ObservableObject`/`RelayCommand`/`RelayCommand<T>`)は外部パッケージに依存せず自前実装。`RelayCommand<T>`は削除コマンド(対象タスク)・ドロップコマンド(移動要求)など、実行時にパラメータを渡す必要があるコマンド用に用意した
- `TaskColumnViewModel`が1カラム分の状態(タスク一覧・新規タスク入力・追加/削除コマンド)を保持し、`MainViewModel`が`TodoColumn`/`InProgressColumn`/`DoneColumn`の3インスタンスと、カラム間移動を行う`MoveTask`/`MoveTaskCommand`を公開する構成にした
- ドラッグ&ドロップは`DragDropBehavior`という添付ビヘイビアに閉じ込め、View(XAML)・コードビハインドにロジックを一切書かせない設計にした。`IsDragSource`(ドラッグ元の`ItemsControl`に設定)でマウス操作からの`DragDrop.DoDragDrop`呼び出しを行い、`DropCommand`+`DropTargetStatus`(ドロップ先の`Border`に設定)でドロップ結果を`MoveTaskRequest`として`MainViewModel.MoveTaskCommand`に橋渡しする
- **重要な落とし穴**: WPFの`Drop`イベントは、`AllowDrop=true`を設定しただけでは発火しない。`DragEnter`/`DragOver`で明示的に`DragEventArgs.Effects`を設定しない限り、既定ではドロップが拒否扱いとなり`Drop`イベント自体が呼ばれない。`DragEnterOrOver`ハンドラで`e.Effects = DragDropEffects.Move`を明示することで解消した
- カラム名(`Todo`/`InProgress`/`Done`)は`System.Threading.Tasks.TaskStatus`との名前衝突を避けるため独自の`KanbanStatus`enumとして定義した
- 削除ボタンの`Command`は、タスクカードの`DataTemplate`が全カラムで共有されているため、`ItemsControl.Tag`に各カラムの`DeleteCommand`をバインドし、`RelativeSource AncestorType=ItemsControl`経由で参照する方式にした
- 当初は対応中・完了列にも入力欄・追加ボタンを配置していたが、実機確認時にユーザーから指摘を受け、一般的なカンバンボードの慣例(新規タスクは未着手列からのみ投入し、対応中・完了へはドラッグでのみ到達する)に合わせて、**未着手列のみ**入力欄・追加ボタンを表示する仕様に変更した(`TaskColumnViewModel`自体は引き続き全カラムでAddCommandを持つ汎用実装のままとし、View側で導線を絞った)
- **UI Automationでのドラッグ&ドロップ自動検証の限界**: `mouse_event`/`SendInput`の両API、ウィンドウのフォアグラウンド化、待機時間の延長など複数パターンを試したが、`DragEnter`/`DragOver`は正しく発火し受理(`Effects=Move`)される一方、`Drop`イベントが一度も発火しなかった。これはWPFのネイティブOLEドラッグ&ドロップの完了判定(`DoDragDrop`内部の`QueryContinueDrag`ループ)が、本環境での合成マウス入力を認識できないことによる自動化ツール側の制約と判断し、ドラッグ&ドロップの実機動作確認はユーザーによる手動操作で行った(正常動作を確認済み)。カラム間移動のロジック自体(`MainViewModel.MoveTask`/`MoveTaskCommand`)はユニットテストで検証済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
