# BMI計算機

## 学習ポイント
入力検証(Validation)、スタイル(Style/Trigger)の練習

## 概要
身長・体重を入力しBMIを計算、判定結果を表示するアプリ。

## 実装メモ
- BMI計算・判定区分(低体重/普通体重/肥満)のロジックを `BmiEngine` クラスに分離し、xUnitでTDD(Red→Green→Refactor)により実装した
- 入力検証は `IDataErrorInfo` ではなくWPF標準の `ValidationRule` を継承した汎用 `NumericRangeValidationRule` で実装。`Min`/`Max`/`FieldName` をXAML側で指定して身長・体重の両方で再利用している。`ValidationRule.Validate()` はUIなしでxUnitから直接呼び出しテスト可能
- MVVMは導入せず、コードビハインド自身をDataContextとした簡易プロパティ(`HeightInput`/`WeightInput`)にTextBox.Textをバインドし、`Binding.ValidationRules` で検証をかけている
- 入力エラー時は `Style` の `Trigger`(`Validation.HasError`)で赤枠+ツールチップのエラーメッセージを表示。判定結果の文字色も `TextBlock.Tag` の値に応じた `Style.Triggers` で切り替えている(DataTrigger不要でシンプルに実現)
- **ハマった点**: 検証エラーの有無をコードビハインドで監視して計算ボタンの有効/無効を切り替えるため `Validation.Error` ルーティングイベントをGridで購読したが、最初は入力しても一切発火しなかった。原因は `Binding.NotifyOnValidationError` の既定値が `false` であること。`ValidationRule` 自体は正しく呼ばれ `Validation.HasError`/赤枠表示は機能していたが、ルーティングイベントの発火にはBinding側で明示的に `NotifyOnValidationError="True"` を指定する必要があった
- UI Automationで実機を操作し、数値以外・範囲外の入力でボタンが無効化されること、正常な入力(普通体重/肥満/低体重の3パターン)でBMI値・判定区分・文字色が正しく表示されることを確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
