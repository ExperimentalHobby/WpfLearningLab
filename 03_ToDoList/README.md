# ToDoリスト

## 学習ポイント
ListBox/ListView、CRUD操作の基礎

## 概要
タスクを追加・編集・削除・完了管理できるToDoリストアプリ。

## 実装メモ
- タスクのCRUDロジックを `ToDoListEngine` クラスに分離し、xUnitでTDD(Red→Green→Refactor)により実装した。`MainWindow.xaml.cs` はボタン・チェックボックスのイベントを`ToDoListEngine`に委譲するだけの薄いコードビハインドにしている
- タスクは `ToDoTask`(Id/Title/IsDone)のシンプルなモデル。Idは追加順の連番で、空白のみのタイトルは追加・編集ともに無視する(前後の空白はトリム)
- MVVMはまだ導入していない(Phase1のため)。一覧の再描画は `ListBox.ItemsSource` を都度張り替える方式(01/02と同様)
- 一覧はListBoxのDataTemplateで各行にCheckBox(完了切り替え)+タイトルを表示。選択中タスクのタイトルは編集用TextBoxに反映され、「更新」ボタンで反映する
- UI Automationで実機を操作し、追加(空白拒否含む)・完了チェック・編集・削除を確認済み。検証時に `TogglePattern.Toggle()` で自動化すると WPF の `Click` イベントを経由せず状態が正しく検証できないことが分かったため、実際のマウスクリックをシミュレートして確認した

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
