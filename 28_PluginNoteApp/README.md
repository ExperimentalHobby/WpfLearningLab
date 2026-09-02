# プラグイン対応メモアプリ

## 学習ポイント
MEF(Managed Extensibility Framework)、拡張性のあるアーキテクチャ

## 概要
プラグイン(拡張機能)DLLを実行時に動的に読み込んで、メモ本文に対する処理(文字数カウント等)を追加できるメモアプリ。MEF(`System.Composition`、軽量版MEF)を使用する。

## 構成
このアプリはプラグイン機構を成立させるため、1フォルダの中に複数の`.csproj`を持つ(本リポジトリの他アプリでは通常1本体+1テストの2プロジェクトだが、このアプリのみ4プロジェクト構成)。
- `PluginNoteApp.csproj` … ホストアプリ本体(WPF)
- `Contracts/PluginNoteApp.Contracts.csproj` … `IMemoPlugin`インターフェースのみを持つ、WPFに依存しない小さいクラスライブラリ。ホストアプリ・プラグインの両方がこれだけを参照することで、プラグインDLLがホストの全依存関係を引きずらないようにする
- `Plugins/CharacterCountPlugin/CharacterCountPlugin.csproj` … サンプルプラグイン(`Contracts`のみ参照)
- `test/PluginNoteApp.Tests.csproj` … `PluginNoteApp.csproj`と`CharacterCountPlugin.csproj`の両方を`ProjectReference`し、ビルド順序を保証した上で実際にビルドされたプラグインDLLを使った統合テストを書けるようにしている

## 実装メモ
- `IPluginLoader`/`MefPluginLoader`が指定フォルダ内の`*.dll`を`Assembly.LoadFrom`で読み込み、`System.Composition.Hosting.ContainerConfiguration`で`[Export(typeof(IMemoPlugin))]`を探す。1DLLごとに成功/失敗を`PluginLoadResult`として記録し、失敗しても他のDLLの読込・ホストアプリの起動を止めない
- `MefPluginLoaderTests`は、`test/PluginNoteApp.Tests.csproj`が`ProjectReference`している`CharacterCountPlugin.csproj`の実際のビルド出力DLLを一時フォルダにコピーして読み込ませることで、モックではなく実際のMEF合成が正しく動作することを検証している。同様に、壊れたバイト列を書き込んだ`.dll`ファイルを読み込ませ、例外を投げず失敗として記録されることも実際のファイルI/Oで検証した
- 当初`container.GetExport<IMemoPlugin>()`(1件必須)を使っていたが、これだと`IMemoPlugin`をエクスポートしない正常なDLL(例: `Contracts.dll`自体)が紛れ込んだ場合に例外扱いになってしまう。`container.GetExports<IMemoPlugin>()`(複数可・0件可)に変更し、「エクスポート無し」と「読込失敗」を区別できるようにした
- ホストアプリは実行ファイルと同じフォルダの`Plugins`サブフォルダをプラグイン置き場として走査する。本リポジトリの`Directory.Build.props`によりビルド成果物は`bin/PluginNoteApp/<Configuration>/<TargetFramework>/`に出力されるため、`CharacterCountPlugin.dll`を実際に読み込ませて動作確認する場合は`bin/CharacterCountPlugin/<Configuration>/net10.0/`からこの`Plugins`フォルダへ手動でコピーする必要がある(自動コピーの仕組みは持たせていない。実際のプラグイン配布シナリオに近い形にした)
- UI Automationで実機を操作し、以下を確認済み:
  - 正常なプラグイン(`CharacterCountPlugin.dll`)と意図的に壊したDLL(`Broken.dll`)を`Plugins`フォルダに混在させた状態で起動しても、ホストアプリはクラッシュせず起動し、壊れたDLLのエラーメッセージ(`Bad IL format...`)が画面に表示されること
  - メモ本文を入力し、ComboBoxからプラグインを選択して「実行」を押すと、正しい実行結果(「文字数: 11文字」)が表示されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
