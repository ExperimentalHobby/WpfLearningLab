# メモ帳クローン

## 学習ポイント
TextBox、ファイル読み書き(保存/開く)

## 概要
シンプルなテキストエディタ。ファイルを開いて編集し、保存できる。

## 実装メモ
- テキスト内容・ファイルパス・未保存フラグの管理とウィンドウタイトル生成を `NotepadEngine` クラスに分離し、xUnitでTDD(Red→Green→Refactor)により実装した。実ファイルI/O(`File.ReadAllText`/`WriteAllText`)とダイアログ(`OpenFileDialog`/`SaveFileDialog`/`MessageBox`)は `MainWindow.xaml.cs` が担当する薄いコードビハインドにしている
- 未保存確認は「新規作成」「開く」「ウィンドウを閉じる」の3箇所で共通化した(`ConfirmProceedDespiteUnsavedChanges`)。Issueの完了条件は「閉じる時」のみ明記だが、データ消失防止のためユーザーと合意の上、新規作成・開くにも適用した
- 本文TextBoxのTextChangedで毎回IsDirtyを立てると、プログラムからの読込・新規作成時にも誤って未保存扱いになってしまうため、`_suppressTextChanged` フラグで抑止している
- UI Automationでの検証では、Windows標準の「名前を付けて保存」ダイアログのファイル名入力欄の `AutomationId` がダイアログの種類によって異なった(保存ダイアログは `1001`、開くダイアログは `1148`)。`AutomationId` に依存せず `Name` に「ファイル名」を含む `Edit` 要素を検索する方式に切り替えて解決した
- 同じくUI Automationでのメニュー操作は、`InvokePattern`/`ExpandCollapsePattern` や座標クリックでは不安定だったため、`Alt+F` などのアクセスキー(ニーモニック)を `SendKeys` で送る方式に切り替えたところ安定した
- UI Automationで実機を操作し、新規作成・開く・保存・名前を付けて保存・未保存確認ダイアログ(新規作成/開く/閉じるそれぞれで確認→キャンセルで継続、いいえで破棄)を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
