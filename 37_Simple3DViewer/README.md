# 簡易3Dモデルビューア

## 学習ポイント
Viewport3D、System.Windows.Media.Media3D、3Dオブジェクトの操作

## 概要
`Viewport3D`を使い、立方体・球をコード上で生成して表示するビューア。マウスドラッグでカメラを軌道回転(オービット)、ホイールでズーム、オブジェクト種別・マテリアル色を切り替えられ、自動回転のON/OFFも可能。

## 実装メモ
- `MeshFactory`で立方体(6面×4頂点=24頂点、12三角形)と球(UV球、分割数(slices/stacks)から頂点数・三角形数が決定)の`MeshGeometry3D`をコード上で生成した。立方体は面ごとに独立した頂点を持たせ、面ごとの法線(平坦シェーディング)が正しく出るようにした
- カメラは球面座標(方位角Azimuth・仰角Elevation・距離Distance)で管理し、`ViewModel`のプロパティ変更をフックして`PerspectiveCamera.Position`/`LookDirection`を再計算する設計にした(`x = distance・cos(仰角)・sin(方位角)`等)。マウスドラッグ量→角度変化、ホイール量→距離変化の計算は`CameraOrbitCalculator`という純粋な静的クラスに切り出し、WPFの3D APIやDispatcherに依存せず決定的に単体テストした(仰角の-89〜89度クランプ、距離の2〜20クランプを含む)
- 自動回転は`24_MusicPlayer`/`27_NetworkMonitor`と同じ「View側がタイマーを保持する」方針で、`DispatcherTimer`(50ms間隔)がON時のみ`Azimuth`を加算する
- オブジェクト種別・マテリアル色の切り替えは、`RadioButton`のIsChecked双方向バインディングではなく`Button`+`ICommand`(`CommandParameter`で色名を渡す`RelayCommand<string>`)にした。理由は、CLAUDE.mdに記載の「`TogglePattern.Toggle()`はWPFの`Click`ルーテッドイベントを発火させない」という既知の落とし穴を避け、UI Automationでの動作確認を`InvokePattern`(確実に`Click`相当のロジックを実行する)で行えるようにするため
- ダミーの立体は`MeshGeometry3D`の頂点位置さえ正しければ表示できるため、UI Automationでのマウスドラッグ・ホイール操作の検証はWin32 `mouse_event`によるシミュレーションで行った。ドラッグ量とAzimuth変化量、ホイール量とDistance変化量が`CameraOrbitCalculator`の計算式通りになることを実機で確認した

## 動作確認(UI Automation)
- 「球」「赤」ボタンでオブジェクト・マテリアルが例外無く切り替わることを確認
- Viewport3D中央でマウスを左ボタン押下したまま水平に80px×10ステップドラッグすると、方位角が`0.0`→`-24.0`(ドラッグ量80px×感度0.3)に変化することを確認
- マウスホイールを回すと、距離が最小値(2.00)にクランプされることを確認
- 自動回転チェックボックスをONにして1秒待つと、方位角が継続的に変化し続けることを確認

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
