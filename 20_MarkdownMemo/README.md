# 簡易CMS風メモアプリ

## 学習ポイント
Markdown表示、RichTextBoxまたはWebView2連携

## 概要
Markdown形式でメモを記述し、右ペインでプレビュー表示できるメモアプリ。複数メモの保存・一覧・読み込み・削除に対応する。Phase2(11〜20)の最後のアプリ。

## 実装メモ
- Markdown→HTML変換には`Markdig`(`UseAdvancedExtensions()`パイプライン)を使用。テーブル等のGFM拡張記法にも対応する
- プレビュー表示は`WebView2`を採用した。RichTextBoxへのFlowDocument変換よりMarkdownの見た目再現度が高く、Issueの学習ポイントにも明記されているため選定。WebView2は非同期初期化(`EnsureCoreWebView2Async`)が必要で、かつバインディングに対応せず`NavigateToString`呼び出しでの描画が必要という特性上、ViewModelには持たせずコードビハインドで`MainViewModel.PreviewHtml`の`PropertyChanged`を購読して描画する(既存アプリの`PasswordBox`と同様、View技術固有の関心事はコードビハインドに閉じ込める方針)
- メモの永続化はSQLite/EF Coreではなく、シンプルなファイルベース保存(1メモ=1個の`.md`ファイル)を採用した。タイトルをファイル名(兼識別子)とし、`FileMemoRepository`がフォルダ内`*.md`の列挙・読み書き・削除を行う
- `MarkdigMarkdownToHtmlConverter`は入力Markdown文字列→出力HTML文字列の純粋な変換なので、ViewModelのテストにも実物をそのまま使い、フェイクは用意していない(既存アプリの`AesPasswordCryptoService`と同様の方針)
- 一覧から選択した内容が編集欄に反映され、保存すると一覧の選択状態が保存したメモに追従する
- UI Automationで実機を操作し、以下を確認済み:
  - タイトル・Markdown本文を入力すると、右ペインのプレビューがリアルタイムに更新されること(見出し・太字・箇条書きが正しくHTML表示される)
  - 保存すると一覧に反映されること
  - 「新規」で編集欄・選択状態がクリアされること
  - 一覧からメモを選択すると編集欄に内容が読み込まれること
  - 削除が一覧に反映されること
  - アプリを再起動しても、ファイルとして保存したメモが正しく一覧に復元されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
