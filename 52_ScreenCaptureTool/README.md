# スクリーンキャプチャツール

## 学習ポイント
Win32 APIによる画面キャプチャ、Per-Monitor DPI対応、マルチモニタの座標系

## 概要
範囲選択でスクリーンショットを撮影し、マルチモニタ・DPI差異に対応するキャプチャツール。

## 実装メモ
- **DPI対応**: `app.manifest`に`<dpiAwareness>PerMonitorV2</dpiAwareness>`を宣言し、モニタごとにDPIが
  異なる環境でもプロセス側でDPI仮想化されずに実座標を扱えるようにしている
- **キャプチャ本体**: `GdiScreenCaptureService`が`System.Drawing.Graphics.CopyFromScreen`(Win32 `BitBlt`の
  ラッパー)でビットマップを取得し、`CreateBitmapSourceFromHBitmap`でWPFの`BitmapSource`に変換する
- **モニタ情報取得**: `Win32MonitorInfoProvider`が`EnumDisplayMonitors`でモニタごとの物理範囲(`RECT`)を、
  `GetDpiForMonitor`(Shcore.dll)でモニタごとの実効DPIを取得する。モニタ単体のDPIスケールで物理→論理変換した
  範囲を、そのモニタの論理座標系として扱う簡易実装とした(モニタ間の論理座標オフセットの厳密な累積計算は
  Windowsの内部仕様に依存するため、本アプリでは座標変換ロジックの検証を優先している)
- **座標変換ロジック**: `MonitorDpiCoordinateConverter`(UI非依存の純粋ロジック)が、
  - `Normalize(start, end)`: ドラッグの開始・終了点(逆方向ドラッグ含む)から正規化された論理矩形を求める
  - `ToPhysicalRegion(logicalRect, monitor)`: 論理矩形をモニタのDPIスケール・物理オフセットを使って
    物理ピクセル座標に変換する
  - `ToPhysicalRegions(logicalRect, monitors)`: モニタ境界をまたぐ矩形を、重なりのある各モニタの
    担当領域(交差部分)ごとに分割して物理座標に変換する
- **範囲選択UI**: `SelectionOverlayWindow`が仮想スクリーン全体(`SystemParameters.VirtualScreen*`)を覆う
  半透明ウィンドウを表示し、マウスドラッグで選択矩形を描画する。`Esc`キーでキャンセルできる。
  `WpfRegionSelector`が選択結果(論理矩形)を`MonitorDpiCoordinateConverter`で物理座標に変換し、
  モニタ境界をまたぐ場合は各モニタの担当領域をバウンディングボックスとして結合する
- **出力**: `WpfClipboardImageService`(`Clipboard.SetImage`)でのコピー、`PngFileSaveService`
  (`PngBitmapEncoder`)でのファイル保存、保存先は`WpfSaveFileDialogService`(`SaveFileDialog`)で選択する
- **範囲選択中の自ウィンドウ非表示**: `MainWindow`は範囲選択開始時に自身を`Hide()`し、選択完了後に`Show()`
  することで、自ウィンドウがキャプチャ結果に映り込まないようにしている

## 動作確認(UI Automation)
- 「全画面キャプチャ」でプレビューに画像が表示されることを確認
- 「範囲選択キャプチャ」でオーバーレイ表示中にマウスドラッグ操作(`mouse_event`)を行い、選択範囲がプレビューに
  表示されることを確認(スクリーンショット取得済み)
- 「クリップボードにコピー」で`Clipboard.ContainsImage()`が`true`になることを確認
- 「保存」で`SaveFileDialog`にパスを入力してファイルが実際に作成されることを確認
- マルチモニタ環境での座標ズレ有無は`MonitorDpiCoordinateConverterTests`(DPI100%/150%/200%混在、
  モニタ境界をまたぐ矩形の分割)でユニットテストとして検証した。動作確認を行った実機は単一モニタ環境のため、
  実機でのマルチモニタ検証は未実施

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
