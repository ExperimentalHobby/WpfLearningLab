using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace AccessibleNoteApp.Controls;

/// <summary>
/// <see cref="MemoListControl"/>用の自作AutomationPeer。<see cref="MemoListControl"/>は
/// <see cref="System.Windows.Controls.ItemsControl"/>を使わず自前描画するため、標準では得られない
/// "リスト+リスト項目"としてのUI Automationツリーを自前で構築して公開する。
/// </summary>
/// <param name="owner">このPeerが対応する<see cref="MemoListControl"/>。</param>
public sealed class MemoListControlAutomationPeer(MemoListControl owner)
	: FrameworkElementAutomationPeer(owner), ISelectionProvider
{
	private readonly MemoListControl _owner = owner;

	/// <inheritdoc/>
	protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;

	/// <inheritdoc/>
	protected override string GetClassNameCore() => nameof(MemoListControl);

	/// <inheritdoc/>
	protected override List<AutomationPeer>? GetChildrenCore()
	{
		var children = new List<AutomationPeer>(_owner.ItemCount);
		for (var i = 0; i < _owner.ItemCount; i++)
		{
			children.Add(new MemoListItemAutomationPeer(_owner, this, i));
		}
		return children;
	}

	/// <inheritdoc/>
	public override object? GetPattern(PatternInterface patternInterface) =>
		patternInterface == PatternInterface.Selection ? this : base.GetPattern(patternInterface);

	/// <summary>
	/// 現在選択中の項目に対応する<see cref="MemoListItemAutomationPeer"/>を作成する。
	/// 選択が変化した際に<see cref="AutomationEvents.SelectionItemPatternOnElementSelected"/>を
	/// 発火させるために使う。
	/// </summary>
	/// <param name="index">選択された項目のIndex。</param>
	public AutomationPeer CreateItemPeer(int index) => new MemoListItemAutomationPeer(_owner, this, index);

	/// <inheritdoc/>
	public bool CanSelectMultiple => false;

	/// <inheritdoc/>
	public bool IsSelectionRequired => false;

	/// <inheritdoc/>
	public IRawElementProviderSimple[] GetSelection()
	{
		if (_owner.SelectedIndex < 0)
		{
			return [];
		}

		return [ProviderFromPeer(CreateItemPeer(_owner.SelectedIndex))];
	}
}
