# 色選択パレット

## 学習ポイント
Slider、ColorPicker的なUI、RGB⇔HEX変換

## 概要
R/G/Bをスライダーで調整し、プレビューとHEXコードを表示・入力できるカラーパレットツール。

## 実装メモ
- RGB⇔HEXの変換ロジックを `ColorPaletteEngine` クラスに分離し、xUnitでTDD(Red→Green→Refactor)により実装した。`TryParseHex` は `#` の有無・大文字小文字を許容し、桁数不正・16進数以外の文字を含む場合は例外を投げずfalseを返す
- Slider(R/G/B)とHEX入力欄は相互に値を反映するが、更新中フラグ(`_isUpdatingFromCode`)で無限ループを防いでいる(01/02の単位変換ツールで使った手法を踏襲)
- 不正なHEXコード入力時は赤字のエラーメッセージのみ表示し、Slider・プレビューは直前の値を保持したままにしてクラッシュを防いでいる
- UI Automationで実機を操作し、Slider操作→プレビュー・HEX連動、HEX入力→Slider・プレビュー連動、不正なHEX入力時のエラー表示とSlider値維持を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
