# テーマ・多言語切替アプリ

## 学習ポイント
MergedDictionariesの動的差し替え、DynamicResourceとStaticResourceの違い、.resx + CultureInfoによる多言語切替

## 概要
ライト/ダークテーマと日本語/英語表示をアプリ実行中に切り替えられるサンプルアプリ。選択したテーマ・言語は再起動後も保持される。

## 実装メモ
- テーマ切替: `Themes/LightTheme.xaml` / `Themes/DarkTheme.xaml` の2種類の `ResourceDictionary` を用意し、切替時に `Application.Current.Resources.MergedDictionaries` を `Clear()` して差し替える。起動時は `App.xaml` で `LightTheme.xaml` を静的にマージしておくことで、`StaticResource` が最初から解決できる状態にしている
- `DynamicResourceDemoLabel` / `StaticResourceDemoLabel` の2行はどちらも `AccentBrush` を使うが、前者は `DynamicResource`、後者は `StaticResource` で束縛している。テーマ切替後、前者だけ新しい色(`#60A5FA`)に変わり、後者は読み込み時に解決された古い色(`#2563EB`)のまま変化しない。この挙動差はピクセル値でも確認済み(目視だとどちらも「青系」に見えて分かりづらい)
- 多言語切替: `Resources/Strings.resx`(既定=日本語)と `Resources/Strings.en.resx`(英語)を用意し、.NET SDKの標準機能でサテライトアセンブリ(`en/ThemeAndLocaleApp.resources.dll`)としてビルドされる。`LocalizedStrings` シングルトンがインデクサ `this[string key]` で `ResourceManager.GetString(key, CultureInfo.CurrentUICulture)` を返し、XAML側は `Binding Path=[キー名]` で束縛する
- 言語切替時は `CultureInfo.CurrentUICulture` を変更した上で `LocalizedStrings.Refresh()` を呼び、`PropertyChanged("Item[]")` を発火させて全てのインデクサバインディングを再評価させている(`ObservableCollection` のインデクサ変更通知と同じ規約)
- 設定の保存/読込ロジックを `SettingsStore` に分離し、xUnitでTDD(Red→Green→Refactor)で実装した。設定は `%APPDATA%\ThemeAndLocaleApp\settings.json` にJSON保存する。実装当初、保存先ディレクトリが未作成の場合に `File.WriteAllText` が `DirectoryNotFoundException` を投げてアプリがクラッシュする不具合をUI Automationでの実機確認中に発見し、`Save()`内でディレクトリを事前作成するよう修正した(このケースもテストに追加している)
- UI Automationで実機を操作し、テーマ切替(ウィンドウ背景色の変化)、言語切替(タイトル・ボタン・文言の英日切替)、設定の保存・再起動後の復元を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
