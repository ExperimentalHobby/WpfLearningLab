# 添付ビヘイビアギャラリー

## 学習ポイント
添付プロパティ(Attached Property)の自作、Microsoft.Xaml.Behaviors(Behavior<T>)の自作、XAMLでの振る舞いの再利用

## 概要
添付プロパティと`Microsoft.Xaml.Behaviors`を使った自作ビヘイビアのサンプルを1画面にギャラリー形式で並べたアプリ。

## 実装メモ
- **添付プロパティ**: `AttachedProperties/PlaceholderService` で `TextBox` にプレースホルダー文言を表示する。テキストが空の間だけ `VisualBrush` で描画した文言を `Background` に設定し、入力されると `ClearValue` で元に戻す。「表示すべきかどうか」の判定は `PlaceholderVisibility.ShouldShow(string?)` という純粋関数に切り出し、xUnitでTDD(Red→Green→Refactor)で実装した
- **Behavior(コマンド実行)**: `Behaviors/EnterKeyCommandBehavior` は `Behavior<UIElement>` を継承し、`PreviewKeyDown` でEnterキーを検知して `Command` を実行する。デモ用に最小限の `ICommand` 実装 `Commands/DelegateCommand` を用意した
- **Behavior(ドラッグ移動)**: `Behaviors/DragMoveBehavior` は `Behavior<FrameworkElement>` を継承し、`Canvas` 内の要素をマウスドラッグで移動できるようにする。移動量から新しい座標を計算する部分は `DragMoveCalculator.CalculateNewPosition` という純粋関数に切り出し、TDDで実装した。イベント配線自体(マウスキャプチャ、`MouseMove`ハンドリング)はUI寄りのためTDD対象とせず、UI Automationでの実機確認で検証した
- **実装時の学び(UI Automation)**: `Border`や`Canvas`など多くのレイアウト要素はデフォルトでUI Automationのピア(自動化ツリー上の代表)を持たない。実機確認でBorderの座標取得が常に空になる問題が発生したため、内部の`TextBlock`(`IsHitTestVisible="False"`で配置)の座標を代わりに取得し、そこをクリック/ドラッグする方式で確認した(クリックは`IsHitTestVisible=False`のTextBlockを素通りして下のBorderに届く)
- UI Automationで実機を操作し、(1)プレースホルダーが未入力時のみ表示され入力すると消えること、(2)Enterキー押下でコマンドが実行され結果が表示されること、(3)矩形をドラッグすると実際に座標が移動すること、をそれぞれ確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
