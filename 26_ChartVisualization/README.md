# 簡易グラフ描画(データ可視化)

## 学習ポイント
OxyPlot/LiveChartsライブラリ活用

## 概要
ラベル+数値のデータ点を入力し、棒グラフ・折れ線グラフ・円グラフの3種類を切り替えて可視化するアプリ。グラフ描画には`OxyPlot.Wpf`を使用する。

## 実装メモ
- グラフ生成ロジックを`ChartModelBuilder`(データ点一覧+グラフ種類から`OxyPlot.PlotModel`を組み立てる純粋ロジック)として切り出した。`PlotModel`はUIに依存しないプレーンオブジェクトのため、実際に生成された`Series`の型・件数・値をUIなしで単体テストできる(`19_HabitTracker`の`AchievementRateCalculator`と同じ方針)
- 棒グラフは`BarSeries`+`BarItem`、折れ線グラフは`LineSeries`+`OxyPlot.DataPoint`、円グラフは`PieSeries`+`PieSlice`をそれぞれ1系列生成する。自前の`Models.DataPoint`(ラベル+値)と`OxyPlot.DataPoint`(X/Y座標)は型名が衝突するため、`using DataPoint = ChartVisualization.Models.DataPoint;`のエイリアスで明示的に区別した
- **ハマった点(OxyPlotの軸配置)**: `BarSeries`はOxyPlot 2.x では横棒グラフとして描画され、`CategoryAxis`をY軸(`Position=Left`)に置く必要がある。当初X軸(`Position=Bottom`)に置いていたところ、`Series`/`Axes`の型・件数だけを見る単体テストは全て通過したにもかかわらず、実際に`PlotView`で描画すると「`BarSeries requires a CategoryAxis on the Y Axis.`」という例外が発生し画面にエラーメッセージが表示される不具合になった。`IPlotModel.Update(true)`を直接呼び出すテストを書いても再現せず(この検証はOxyPlotの実描画パス内でのみ行われるため)、UI Automationで実際に`PlotView`を表示して初めて発見できた。修正として`CategoryAxis`の位置をテストで直接検証する回帰テストを追加した
- `MainViewModel`はデータ点の追加/削除・グラフ種類変更のたびに`ChartModelBuilder.Build`を呼び直し`PlotModel`プロパティを更新する。グラフ種類の選択UIは`RadioButton`3つ+`EnumToBooleanConverter`(列挙値↔`IsChecked`の標準的な相互変換パターン)で実装した
- UI Automationで実機を操作し、以下を確認済み:
  - データ点を3件追加すると一覧に反映されること
  - 棒グラフ・折れ線グラフ・円グラフのいずれも例外なく正しく描画されること(上記のバグ修正後)
  - データ点を削除すると一覧・グラフの両方から反映されること
  - ラベルが空欄、または値が数値でない場合に追加ボタンが無効化されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
