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

	/// <summary>
	/// Index単位で生成済みの<see cref="MemoListItemAutomationPeer"/>を保持するキャッシュ。
	/// 呼び出しのたびに新規インスタンスを生成すると、支援技術側が同一項目として要素を
	/// 追跡できなくなるため、同じIndexに対しては同一インスタンスを返す。
	/// </summary>
	private readonly Dictionary<int, MemoListItemAutomationPeer> _itemPeersByIndex = [];

	/// <inheritdoc/>
	protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;

	/// <inheritdoc/>
	protected override string GetClassNameCore() => nameof(MemoListControl);

	/// <inheritdoc/>
	protected override List<AutomationPeer>? GetChildrenCore()
	{
		PruneStaleItemPeers();

		var children = new List<AutomationPeer>(_owner.ItemCount);
		for (var i = 0; i < _owner.ItemCount; i++)
		{
			children.Add(GetOrCreateItemPeer(i));
		}
		return children;
	}

	/// <inheritdoc/>
	public override object? GetPattern(PatternInterface patternInterface) =>
		patternInterface == PatternInterface.Selection ? this : base.GetPattern(patternInterface);

	/// <summary>
	/// 現在選択中の項目に対応する<see cref="MemoListItemAutomationPeer"/>を作成する。
	/// 選択が変化した際に<see cref="AutomationEvents.SelectionItemPatternOnElementSelected"/>を
	/// 発火させるために使う。同じIndexに対しては、キャッシュ済みの同一インスタンスを返す。
	/// </summary>
	/// <param name="index">選択された項目のIndex。</param>
	public AutomationPeer CreateItemPeer(int index) => GetOrCreateItemPeer(index);

	private MemoListItemAutomationPeer GetOrCreateItemPeer(int index)
	{
		if (!_itemPeersByIndex.TryGetValue(index, out var peer))
		{
			peer = new MemoListItemAutomationPeer(_owner, this, index);
			_itemPeersByIndex[index] = peer;
		}
		return peer;
	}

	/// <summary>
	/// 項目数が減少した場合に、範囲外になったIndexのキャッシュを破棄する。
	/// </summary>
	private void PruneStaleItemPeers()
	{
		var staleIndexes = _itemPeersByIndex.Keys.Where(i => i >= _owner.ItemCount).ToList();
		foreach (var index in staleIndexes)
		{
			_itemPeersByIndex.Remove(index);
		}
	}

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
