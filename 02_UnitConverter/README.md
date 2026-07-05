# 単位変換ツール

## 学習ポイント
温度/長さ/重さなどの変換、ComboBox+TextBox連携

## 概要
温度・長さ・重さなど複数カテゴリの単位変換を行うツール。カテゴリ選択に応じて変換元/変換先の単位を切り替える。

## 実装メモ
- カテゴリ(温度/長さ/重さ)ごとに `IUnitConverter` を実装する `TemperatureConverter` / `LengthConverter` / `WeightConverter` を用意し、`UnitConverterEngine` がカテゴリ名から対応するコンバータを選んで委譲するファサード構成にした
- 長さ・重さは基準単位(m, kg)への換算係数を使った乗除算、温度は摂氏を仲介した変換式で実装。数値計算は電卓アプリ(#1)の反省を踏まえ `decimal` を使用
- カテゴリ切り替え時は `FromUnitComboBox` / `ToUnitComboBox` の `ItemsSource` を張り替え、既定で1番目・2番目の単位を選択する。コードから選択肢を更新している間は `_isUpdatingUnits` フラグでイベントの誤発火(変換の二重実行)を防いでいる
- 変換元TextBoxの `TextChanged` で都度変換を実行。数値以外・空入力の場合は例外を出さず変換先を空表示にする
- UI Automationで実機を操作し、温度・長さ・重さの変換、カテゴリ切り替え、異常系(数値以外・空入力)を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
