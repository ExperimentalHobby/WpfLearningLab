# アクセシビリティ対応メモアプリ

## 学習ポイント
カスタムAutomationPeer、UI Automation、アクセシビリティ設計

## 概要
スクリーンリーダー等の支援技術に配慮した、シンプルなメモアプリ(作成・編集・一覧・削除)。メモ一覧は
既製の`ListBox`を使わず完全に自前描画のコントロールとして実装し、`AutomationPeer`を自作してUI
Automationツリーに正しく公開している。

## 実装メモ
- **自前描画の一覧コントロール**: `MemoListControl`は`ItemsControl`/`ListBox`を一切使わず、
  `OnRender(DrawingContext)`でメモタイトルの行を直接描画する。既製コントロールを使うと標準の
  AutomationPeerが無償で付いてしまい「カスタムAutomationPeerを自作する」という学習目的が成立しなく
  なるため、あえて子UIElementを持たない完全自前実装にした
- **カスタムAutomationPeerの構成**(WPF標準の`ListBox`の仮想化アイテムと同じ設計を踏襲):
  - `MemoListControlAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider`が
    `AutomationControlType.List`を返し、`GetChildrenCore()`でメモの件数分だけ
    `MemoListItemAutomationPeer`を動的に生成して返す
  - `MemoListItemAutomationPeer : AutomationPeer, ISelectionItemProvider`(対応する子UIElementを
    持たないため`FrameworkElementAutomationPeer`ではなく`AutomationPeer`を直接継承する。この場合
    `GetHelpTextCore`/`GetAccessKeyCore`等の多数の抽象メンバーを自前実装する必要があった)が
    `AutomationControlType.ListItem`を返し、`GetNameCore()`でメモタイトルを、
    `GetBoundingRectangleCore()`でオーナーコントロールの行レイアウトから計算した画面座標を返す
  - 選択が変わった際は`RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected)`を
    発火し、Narratorが選択メモの変更を読み上げられるようにした
- **キーボード操作**: `MemoListControl.OnKeyDown`で↑/↓/Home/End/Enter/Deleteを処理する。上下移動の
  ロジックは`MemoListNavigator`(現在Index・件数・押されたKeyから次のIndexを返す純粋関数)として切り出し
  単体テストした。Deleteキーは一覧コントロール自身が(専用の`DeleteRequested`ルーテッドイベントとして)
  処理することで、本文入力欄でのテキスト編集用Deleteキーと衝突しないようにした
- **永続化**: `JsonMemoRepository`が実ファイルI/O(1メモ1JSONファイル、`%AppData%\AccessibleNoteApp`)で
  保存・読込・削除を行う(`38_DragDropFileTagger`の`JsonTaggedFileRepository`と同じ、モックより実I/Oの
  方針)。単体テストは実一時フォルダに対して行う
- **AutomationProperties**: タイトル/本文入力欄に`AutomationProperties.LabeledBy`でラベルのTextBlockを
  関連付け、各ボタンには`AutomationProperties.HelpText`で操作内容を補足した
- **キーボードのみでの全操作**: 新規(Ctrl+N)・保存(Ctrl+S)は`Window.InputBindings`、削除は一覧コントロール
  フォーカス時のDeleteキーで行える。各ボタンにはアクセスキー(下線ニーモニック、Alt+新規/保存/削除)を
  設定した。メモ一覧でEnterを押すとタイトル入力欄にフォーカスが移動し、選択→編集の一連の操作を
  キーボードのみで完結できる
- **ハイコントラスト対応**: 色は`SystemColors.WindowBrush`/`ControlTextBrush`/`HighlightBrush`/
  `HighlightTextBrush`等のシステムカラーのみを使い、独自の固定色は使っていない。これによりWindowsの
  ハイコントラストテーマ設定に自動的に追従する。**実機のWindowsハイコントラストモードを本作業中に実際に
  ON/OFF切り替えることは、デスクトップ全体の見た目に影響する副作用が大きいため行っていない。**
  システムカラーのみを使用する実装であることをコードレビューで確認する形に留めた

## 動作確認(UI Automation)
Narrator自身が使うUI Automation APIで直接プロパティを取得することで、Narratorでの読み上げ内容を
裏付ける形で確認した(実際にNarratorを起動して音声出力を確認することは、実機のスクリーンリーダーが
常駐・発話を始めてしまう副作用があるため行っていない):
- `MemoListControl`の`AutomationElement`の`ControlType`が`ControlType.List`であること、`Name`が
  「メモ一覧」、`HelpText`が案内文になっていることを確認
- メモを2件登録すると、`MemoListControl`の子として`ControlType.ListItem`のAutomationElementが2件現れ、
  それぞれの`Name`が各メモのタイトルと一致することを確認
- ↓キーで選択が移動し、選択された項目の`SelectionItemPattern.IsSelected`がtrueになることを確認
- Enterキーでタイトル入力欄にフォーカスが移動し、選択したメモの内容が表示されることを確認
- 一覧にフォーカスがある状態でDeleteキーを押すと、選択中のメモが削除されること(一覧の子要素数が
  1件減ること)を確認
- **タイトル入力欄にフォーカスがある状態からCtrl+Sを押すと新規メモが保存され一覧に追加されること、
  続けてCtrl+Nを押すと入力欄がクリアされることを実機で確認**(ボタンクリックに頼らずキーボード
  ショートカットのみで新規作成・保存が行えることの実証)
- 検証時に作成したテスト用メモ・保存先フォルダ(`%AppData%\AccessibleNoteApp`)は、検証後にアプリ内の
  削除操作またはフォルダ削除により全て後始末済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
