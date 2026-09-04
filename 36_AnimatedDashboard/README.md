# アニメーションダッシュボード

## 学習ポイント
Storyboard、DoubleAnimation、イージング関数

## 概要
複数のKPI風の指標(売上・ユーザー数・コンバージョン率・エラー件数)を、`Storyboard`+`DoubleAnimation`で滑らかにカウントアップ表示するダッシュボードアプリ。更新ボタンでダミーデータを再生成し、イージング関数の種類を切り替えられる。

## 実装メモ
- `KpiCard`(UserControl)は`TargetValue`(実際の値)と`DisplayValue`(アニメーション中の表示値)を分離したDependencyPropertyを持つ。`TargetValue`変更時に実際に`Storyboard`オブジェクトを組み立て(`34_CustomGaugeControl`では`BeginAnimation`を直接呼んだのに対し、本アプリでは学習ポイント通り`Storyboard`/`Storyboard.SetTarget`/`SetTargetProperty`を明示的に使用)、`DisplayValue`をカウントアップさせる
- イージング関数の選択(`EasingType`列挙型 → `IEasingFunction`)は`EasingFunctionFactory`という純粋な静的クラスに切り出し、Dispatcherに依存せず単体テストした。`Linear`は`DoubleAnimation`既定の線形補間を使うため`null`を返す設計にした
- ダミーデータ生成(`DummyMetricGenerator`)は同一シードの`Random`で決定的に検証できるようにした
- `KpiCard`自体のテスト(Storyboard開始が例外を起こさないこと)には`34_CustomGaugeControl`と同じく`Xunit.StaFact`の`[WpfFact]`を使用した
- **ハマった点(UserControlも既定でUI Automationのツリーに現れない)**: `34_CustomGaugeControl`の`GaugeControl`と同様、`UserControl`である`KpiCard`も`OnCreateAutomationPeer`をオーバーライドしないと`AutomationProperties.AutomationId`を設定してもUI Automationから見えなかった。`OnCreateAutomationPeer`をオーバーライドして`FrameworkElementAutomationPeer`を返すことで解決した
- **ハマった点(UI Automationスクリプトの日本語文字列比較が失敗する)**: 検証用PowerShellスクリプト(Windows PowerShell 5.1、`powershell -File`)内でBOM無しUTF-8ファイルに書いた日本語のAutomationId("売上"等)を`-eq`比較すると、実行時エラーは出ないが常に不一致になった(文字コード解釈のずれによるものと考えられる)。AutomationIdでの検索はASCII文字("RefreshButton"等)に限定し、日本語を含む値の検証はテキスト内容の直接読み取り(`ControlType.Text`要素の列挙)で行うことで回避した

## 動作確認(UI Automation)
- 起動直後、4枚のKPIカードにそれぞれ指標名・単位付きの値(例: `売上:394.2万円`)が表示されていることを確認
- 更新ボタンを押して2秒待つと、4枚すべての値がダミーデータの再生成+カウントアップアニメーションの完了により新しい値に変わっていることを確認
- イージング選択用ComboBoxが存在し操作可能であることを確認

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
