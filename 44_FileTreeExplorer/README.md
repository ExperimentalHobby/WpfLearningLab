# フォルダツリー閲覧アプリ

## 学習ポイント
TreeView + HierarchicalDataTemplate、展開時の遅延読み込み(ダミーノードパターン)、選択フォルダ内ファイル一覧の表示

## 概要
TreeViewでローカルのドライブ/フォルダ構造を階層表示するアプリ。展開時に子フォルダを遅延読み込みし、選択フォルダ内のファイル一覧を右ペインに表示する。

## 実装メモ
- 実ファイルシステムへのアクセスを `IFileSystem` で抽象化し、アクセス拒否等の例外を吸収して `(成功可否, 値, エラーメッセージ)` を返す `FileSystemBrowserEngine` に分離した。xUnitでTDD(Red→Green→Refactor)で実装し、テストは `IFileSystem` のFake実装(`FakeFileSystem`)を使ってアクセス拒否例外のケースも検証している
- `Models/FolderNode` はダミーノードパターンで遅延読み込みを実現する。生成時に「読み込み中...」のダミー子ノードを1つ持たせておき、`TreeViewItem.Expanded` イベント発火時に `FileSystemBrowserEngine` で実際のサブフォルダに置き換える。この方式では展開してみるまで子の有無が分からないため、サブフォルダが実際には0件のフォルダも展開矢印が表示される(既知の制約として許容)
- `FolderNode` は `ToString()` をオーバーライドして表示名を返すようにしている。既定のままだと UI Automation 上のTreeViewItemの名前が型名(`FileTreeExplorer.Models.FolderNode`)になってしまい、自動化操作や検証がしづらいための対応
- アクセス拒否(`UnauthorizedAccessException`)等が発生した場合は例外を握りつぶし、子ノード0件+画面下部にエラーメッセージを表示するのみでアプリを継続動作させる。実機確認では `C:\System Volume Information` を展開してアクセス拒否時の挙動を確認した
- ファイル一覧は選択フォルダ変更時(`TreeView.SelectedItemChanged`)に読み込む。F5キーまたは「更新」ボタンで選択中フォルダの子フォルダ・ファイル一覧を再読み込みできる
- UI Automationで実機を操作し、ドライブ一覧の表示、フォルダ展開による子フォルダの遅延読み込み、ファイル一覧表示、アクセス拒否フォルダでのエラー表示継続、F5更新を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
