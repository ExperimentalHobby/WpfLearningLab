using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace AccessibleNoteApp.Controls;

/// <summary>
/// <see cref="MemoListControl"/>の各行に対応する、UIElementを持たない仮想的なAutomationPeer。
/// WPF標準の<c>ListBoxItemAutomationPeer</c>(仮想化アイテム用)と同じ設計で、対応するビジュアル要素の
/// 有無に関わらずUI Automationツリーに"ListItem"として存在を公開する。
/// </summary>
/// <param name="owner">この項目を保持する<see cref="MemoListControl"/>。</param>
/// <param name="ownerPeer">オーナーの<see cref="AutomationPeer"/>(<see cref="SelectionContainer"/>用)。</param>
/// <param name="index">この項目の<see cref="MemoListControl.ItemsSource"/>内でのIndex。</param>
public sealed class MemoListItemAutomationPeer(MemoListControl owner, AutomationPeer ownerPeer, int index)
	: AutomationPeer, ISelectionItemProvider
{
	/// <inheritdoc/>
	protected override string GetNameCore() => owner.GetItemTitle(index) ?? string.Empty;

	/// <inheritdoc/>
	protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ListItem;

	/// <inheritdoc/>
	protected override string GetClassNameCore() => nameof(MemoListItemAutomationPeer);

	/// <inheritdoc/>
	protected override bool IsContentElementCore() => true;

	/// <inheritdoc/>
	protected override bool IsControlElementCore() => true;

	/// <inheritdoc/>
	protected override bool IsEnabledCore() => true;

	/// <inheritdoc/>
	protected override bool IsOffscreenCore() => false;

	/// <inheritdoc/>
	protected override bool IsKeyboardFocusableCore() => true;

	/// <inheritdoc/>
	protected override bool HasKeyboardFocusCore() => owner.IsKeyboardFocused && IsSelected;

	/// <inheritdoc/>
	protected override Rect GetBoundingRectangleCore() => owner.GetItemScreenBounds(index);

	/// <inheritdoc/>
	protected override Point GetClickablePointCore()
	{
		var bounds = GetBoundingRectangleCore();
		return new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
	}

	/// <inheritdoc/>
	protected override List<AutomationPeer>? GetChildrenCore() => null;

	/// <inheritdoc/>
	public override object? GetPattern(PatternInterface patternInterface) =>
		patternInterface == PatternInterface.SelectionItem ? this : null;

	/// <inheritdoc/>
	protected override string GetHelpTextCore() => string.Empty;

	/// <inheritdoc/>
	protected override string GetItemStatusCore() => string.Empty;

	/// <inheritdoc/>
	protected override string GetItemTypeCore() => "メモ";

	/// <inheritdoc/>
	protected override string GetAcceleratorKeyCore() => string.Empty;

	/// <inheritdoc/>
	protected override string GetAccessKeyCore() => string.Empty;

	/// <inheritdoc/>
	protected override string GetAutomationIdCore() => string.Empty;

	/// <inheritdoc/>
	protected override AutomationPeer? GetLabeledByCore() => null;

	/// <inheritdoc/>
	protected override AutomationOrientation GetOrientationCore() => AutomationOrientation.None;

	/// <inheritdoc/>
	protected override bool IsPasswordCore() => false;

	/// <inheritdoc/>
	protected override bool IsRequiredForFormCore() => false;

	/// <inheritdoc/>
	protected override void SetFocusCore() => Select();

	/// <inheritdoc/>
	public bool IsSelected => owner.SelectedIndex == index;

	/// <inheritdoc/>
	public IRawElementProviderSimple SelectionContainer => ProviderFromPeer(ownerPeer);

	/// <inheritdoc/>
	public void Select() => owner.SelectedIndex = index;

	/// <inheritdoc/>
	public void AddToSelection() => Select();

	/// <inheritdoc/>
	public void RemoveFromSelection()
	{
		if (IsSelected)
		{
			owner.SelectedIndex = -1;
		}
	}
}
