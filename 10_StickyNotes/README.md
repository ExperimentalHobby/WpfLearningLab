# 付箋(Sticky Notes)アプリ

## 学習ポイント
複数ウィンドウ管理、WindowStyle=None

## 概要
付箋のように複数のメモウィンドウを自由に配置できるアプリ。

## 実装メモ
- 付箋データ(`StickyNoteData`)のJSONシリアライズ/デシリアライズを `StickyNoteSerializer` クラスに分離し、xUnitでTDD(Red→Green→Refactor)により実装した。実際のファイル読み書きは行わず、文字列の変換のみを担当するため、ファイルI/Oなしでテストできる
- `Deserialize` は空文字・不正なJSONでも例外を投げず空リストを返す仕様にし、保存ファイルが存在しない/壊れていてもアプリが起動時にクラッシュしないようにした
- 日本語を含むテキストがJSONで `\uXXXX` にエスケープされて可読性が落ちる問題があったため、`JavaScriptEncoder.Create(UnicodeRanges.All)` を指定してエスケープを回避した
- `MainWindow` はランチャー(「新規付箋」ボタンのみ)とし、起動時(`Loaded`)に `%AppData%\WpfLearningLab.StickyNotes\notes.json` から復元、終了時(`Closing`)に現在開いている付箋の状態を保存する。閉じた(削除した)付箋は追跡リストから除外されるため保存対象に含まれない
- `App.xaml` に `ShutdownMode="OnMainWindowClose"` を指定し、ランチャーを閉じるとアプリ全体が終了するようにした
- `StickyNoteWindow` は `WindowStyle=None` / `AllowsTransparency=True` のカスタムウィンドウ。独自タイトルバーの `MouseLeftButtonDown` で `DragMove()` を呼び出しウィンドウを移動する。背景色は色スウォッチボタンのClickで `SolidColorBrush` を差し替えて変更する
- UI Automationで実機を操作し、複数付箋の作成・独立した移動(ドラッグ)・色変更・削除、アプリ終了時の保存、再起動時の復元(削除した付箋は復元されないことも含む)を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
