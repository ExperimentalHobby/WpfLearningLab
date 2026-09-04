# 自作ゲージコントロール

## 学習ポイント
Controlの継承、DependencyProperty、ControlTemplate、ルーテッドイベント

## 概要
値を円弧状のゲージ(メーター)で表示する再利用可能なカスタムコントロール(`GaugeControlLib`)と、それを使ったデモアプリ(`CustomGaugeControl`)。1つのフォルダに「コントロールライブラリ」と「デモアプリ」の2プロジェクト構成にした(`28_PluginNoteApp`のホスト+プラグイン構成と同様のパターン)。

## 実装メモ
- `GaugeControl`は`Control`を継承し、`Value`/`Minimum`/`Maximum`/`Threshold`のDependencyPropertyと、ControlTemplate専用の内部状態`AnimatedAngle`を持つ。外観は`Themes/Generic.xaml`のControlTemplateで定義し、円弧を`Path`、針を`Line`+`RotateTransform`で表現した
- `Value`変更時は`DoubleAnimation`で`AnimatedAngle`をアニメーションさせ、針が滑らかに動くようにした。`Value`自体(外部から見える値)とアニメーション対象の`AnimatedAngle`(内部の表示状態)を分離したことで、`Value`のgetterは常に最新の実際の値を返しつつ、見た目だけアニメーションで追従する設計にできた
- 角度計算(`GaugeMath.ValueToAngle`)としきい値超過判定(`GaugeMath.HasCrossedThresholdUpward`)はWPFのDispatcher/アニメーションに依存しない純粋な静的クラスに切り出し、決定的に単体テストした
- カスタムルーテッドイベント`ThresholdExceeded`(Bubble)を`EventManager.RegisterRoutedEvent`で登録し、`Value`が下から上へ`Threshold`を超えたときに発火する。同じ`GaugeControl`を異なる`Minimum`/`Maximum`/`Threshold`で複数配置しても、それぞれ独立して正しく発火することをUI Automationで確認した
- **ハマった点1(GaugeControlのユニットテストにはSTAスレッドが必要)**: `Control`(`FrameworkElement`)のインスタンス生成は`InputManager`の初期化を伴うため、既定のMTAスレッドで実行されるxUnitテストでは`呼び出しスレッドは...STAである必要があります`という`InvalidOperationException`で失敗した。`Xunit.StaFact`パッケージ(xunit 2.x系と互換性のある`1.2.69`を使用。最新の`4.x`はxunit.v3向けで本リポジトリのxunit 2.9.3とは組み合わせられなかった)の`[WpfFact]`属性でSTAスレッド上にテストを実行することで解決した
- **ハマった点2(カスタムControlは既定でUI Automationのツリーに現れない)**: `AutomationProperties.AutomationId`をXAMLで設定しても、`Control`は既定で`OnCreateAutomationPeer`をオーバーライドしないため`AutomationPeer`が生成されず、UI Automationのツリーから完全に見えなくなっていた(`FindFirst`で発見できない)。`OnCreateAutomationPeer`をオーバーライドし最小限の`FrameworkElementAutomationPeer`を返すことで解決した。なお、ControlTemplate内の`TextBlock`(値表示)まではUI Automationの子要素として辿れなかった(Panelにも既定でAutomationPeerが無いため)。テンプレートパーツ単位の詳細なUI Automation対応は本アプリの学習スコープ外としたが、`GaugeControl`自体の存在確認・`ThresholdExceeded`イベント発火の確認には支障がなかった
- **既知の制限(スクリーンショットによる目視確認)**: `31_ParallelImageProcessor`と同様、この環境では`Graphics.CopyFromScreen`によるスクリーンショット取得がアプリのウィンドウ領域とは無関係な別プロセスの画面を捉えてしまい、目視確認には使えなかった。針の回転・円弧の描画・数値表示の正しさは、ビルド時のXAMLエラー無し・実行時例外無し、および`ThresholdExceeded`イベントが`Value`の実際の変化に応じて正しく発火すること(値が正しく処理されている間接証拠)で確認した

## 動作確認(UI Automation)
- スライダーで「温度」ゲージ(Minimum=0, Maximum=100, Threshold=80)の値を85まで動かすと、`ThresholdExceeded`が発火しログに1件追加されることを確認
- 別スライダーで「CPU使用コア数」ゲージ(Minimum=0, Maximum=10, Threshold=8、同じ`GaugeControl`を異なる範囲で再利用)の値を9まで動かすと、独立してログに2件目が追加されることを確認

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
