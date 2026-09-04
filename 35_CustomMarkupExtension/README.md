# カスタムMarkupExtensionサンプル集

## 学習ポイント
MarkupExtensionの自作、XAML拡張構文

## 概要
独自のXAML `MarkupExtension`を3種類実装し、その活用例を1画面のデモアプリで示す。

1. **`EnumValuesExtension`**: 指定した列挙型の全値を配列で返す(`{me:EnumValues {x:Type models:Priority}}`)。`ComboBox.ItemsSource`に直接指定でき、列挙型の値一覧を表示するためだけにViewModelへプロパティを追加する必要がなくなる
2. **`UnitConversionExtension`**: 値と単位(px/cm/inch)を指定し、変換済みの数値をXAML解析時に返す(`Width="{me:UnitConversion Value=5, From=Centimeter, To=Pixel}"`)。変換ロジックは`UnitConverter`という純粋な静的クラスに切り出した
3. **`BoolToVisibilityExtension`**: 「boolプロパティにバインドしVisibilityへ変換する」という頻出パターンをまとめたもの。内部で実際に`Binding`を組み立て(`Converter`に独自の`InvertibleBoolToVisibilityConverter`を設定)、`ProvideValue`はそのBindingの`ProvideValue`に委譲する(バインディングと組み合わせたユースケース)

## 実装メモ
- `MarkupExtension`は`ProvideValue(IServiceProvider)`を実装するだけでよく、コンストラクタ引数(`EnumValuesExtension(Type enumType)`)を使うとXAML上で`{me:EnumValues {x:Type models:Priority}}`のように位置引数として渡せる。プロパティ(`UnitConversionExtension.Value`/`From`/`To`)は名前付き引数として`Value=5, From=Centimeter`のように渡せる
- `BoolToVisibilityExtension`はBinding構築ロジック(`BuildBinding()`)を`ProvideValue`から分離した。`ProvideValue`は実際のXAML解析時にしか呼べない(`IServiceProvider`が必要)が、`BuildBinding()`はどこからでも呼べるため、Path/Converter/ConverterParameterが正しく設定されることを実際のXAML無しで単体テストできた
- **標準のMarkupExtension(`StaticResource`等)との違い**: `StaticResource`/`Binding`は「値の参照方法」を表す組み込みの拡張だが、自作の`MarkupExtension`は任意のC#コードを`ProvideValue`内に書けるため、「XAML解析時に計算した値」(`UnitConversionExtension`)や「複数のXAML機能を組み合わせた省略記法」(`BoolToVisibilityExtension`が内部で`Binding`を組み立てるように)を自由に作れる。`{}`の中に置けるという構文だけが共通で、実体は普通のC#クラスである点が大きな違い
- **ハマった点(UI Automationでの検証範囲)**: `ComboBox`のドロップダウン項目一覧や`Rectangle`(Shape)は、実際のツリーダンプ(`AutomationElement.FindAll(TreeScope.Children, ...)`による手動再帰)では`ConvertedRectangle`/`PriorityComboBox`ともに正しいAutomationIdで存在を確認できたが、`FindFirst(TreeScope.Descendants, AutomationIdCondition)`による一括検索では安定して発見できなかった(UI Automation COM相互運用の既知の制約と考えられる)。ビルド成功(XAML解析エラー無し)・単体テスト(`EnumValuesExtensionTests`/`UnitConverterTests`)・実際にウィンドウが例外無く起動することの3点で、両拡張の実際の変換結果が正しいことを確認した

## 動作確認(UI Automation)
- ウィンドウが例外無く起動し、3つのGroupBoxとその内容(ComboBox/Rectangle/CheckBox+Border)が正しいAutomationIdでツリーに存在することを確認(手動ツリーダンプで検証)
- チェックボックスのチェック/解除で、`BoolToVisibilityExtension`によりパネル(`TogglePanel`)がツリーから消える/現れることを確認(Collapsed時はツリーから完全に消えることも確認)

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
