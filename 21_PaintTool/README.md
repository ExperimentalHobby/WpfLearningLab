# お絵かきツール(ペイント風)

## 学習ポイント
Canvas描画、InkCanvas、Undo/Redo実装

## 概要
マウス/ペンでの自由描画ができるペイントツール。色・太さの変更、Undo/Redo、消しゴム、全消去、PNG保存に対応する。

## 実装メモ
- 描画には`InkCanvas`(WPF標準コントロール)を使用。`EditingMode`を`Ink`/`EraseByStroke`で切り替え、消しゴムはストローク単位で消去する
- Undo/Redoは汎用の`UndoRedoStack<T>`(Push/Undo/Redo/Clear、Push時にRedoスタックをクリアする一般的な仕様)として純粋なロジックを切り出し、単体テストした。実際のInkCanvas固有の適用処理(`Strokes.StrokesChanged`の購読、`StrokeCollection`の追加/削除、履歴適用中の再入防止フラグ)は`IInkCanvasController`インターフェース越しにView層(`InkCanvasController`)へ委譲し、ViewModelはこのインターフェースのフェイクでテストした
- PNG保存(`RenderTargetBitmap`+`PngBitmapEncoder`)も`IInkCanvasController.SaveAsPng`に含め、保存先パスの選択は`ISaveFileDialogService`(Win32の`SaveFileDialog`ラップ)経由にすることでViewModelを実ダイアログ非依存でテスト可能にした
- UI Automationで実機を操作し、以下を確認済み:
  - 実際のマウスドラッグでInkCanvasに描画でき、色選択ボタンで指定した色のストロークが描かれること
  - 「元に戻す」ボタンでストロークが消え、「やり直す」ボタンで再描画されること(共にボタンの有効/無効状態が正しく切り替わることも確認)
  - 消しゴムモードに切り替えてストローク上をドラッグすると、そのストロークが消去されること
  - 「全消去」でキャンバスが空になること
  - **既知の制限**: 「保存」ボタン押下で表示されるWindows標準の名前を付けて保存ダイアログは、本開発環境(サンドボックス)ではダイアログ自体が表示されず(WPF/WinFormsを問わず、素の`SaveFileDialog`呼び出しでも同様に発生することを別途確認済み)、実機での自動操作検証ができなかった。`15_ImageViewer`で確認された`OpenFolderDialog`の自動操作の難しさと同種の環境制約と考えられる。保存処理自体は`MainViewModel`の単体テストで`ISaveFileDialogService`呼び出し・`IInkCanvasController.SaveAsPng`呼び出しの配線を検証済みであり、実際のレンダリング処理は標準的な`RenderTargetBitmap`/`PngBitmapEncoder`の組み合わせであるため、コードレビューで妥当性を確認した

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
