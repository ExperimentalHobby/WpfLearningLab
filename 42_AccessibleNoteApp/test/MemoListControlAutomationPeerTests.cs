using System.Reflection;
using AccessibleNoteApp.Controls;
using AccessibleNoteApp.Models;
using System.Windows.Automation.Peers;

namespace AccessibleNoteApp.Tests;

/// <summary>
/// <see cref="MemoListControlAutomationPeer"/> の単体テスト。
/// </summary>
public class MemoListControlAutomationPeerTests
{
	private static readonly MethodInfo GetChildrenCoreMethod =
		typeof(MemoListControlAutomationPeer).GetMethod("GetChildrenCore", BindingFlags.Instance | BindingFlags.NonPublic)!;

	private static MemoListControlAutomationPeer CreatePeer(MemoListControl control) =>
		(MemoListControlAutomationPeer)UIElementAutomationPeer.CreatePeerForElement(control);

	/// <summary>
	/// <see cref="MemoListControlAutomationPeer.GetChildrenCore"/>(protected)を直接呼び出す。
	/// 基底<see cref="AutomationPeer.GetChildren"/>自体が独自にキャッシュを持つため、
	/// このテストが検証したい「GetChildrenCoreが呼ばれるたびに新規インスタンスを生成していないか」を
	/// 正確に検証するには、そのキャッシュを経由しない直接呼び出しが必要。
	/// </summary>
	private static List<AutomationPeer> InvokeGetChildrenCore(MemoListControlAutomationPeer peer) =>
		(List<AutomationPeer>)GetChildrenCoreMethod.Invoke(peer, null)!;

	/// <summary>
	/// パス条件: GetChildrenCoreを2回呼び出しても、同じIndexに対応するAutomationPeerは同一インスタンスであること
	/// (支援技術が要素を同一の項目として追跡できるようにするための回帰テスト)。
	/// </summary>
	[WpfFact]
	public void GetChildrenCore_2回呼び出しても同じIndexは同一インスタンスを返す()
	{
		var control = new MemoListControl
		{
			ItemsSource = [new Memo("id-1", "A", string.Empty, DateTime.Now), new Memo("id-2", "B", string.Empty, DateTime.Now)],
		};
		var peer = CreatePeer(control);

		var first = InvokeGetChildrenCore(peer);
		var second = InvokeGetChildrenCore(peer);

		Assert.Same(first[0], second[0]);
		Assert.Same(first[1], second[1]);
	}

	/// <summary>
	/// パス条件: 項目数が減った場合、範囲外になったIndexに対応するAutomationPeerのキャッシュは
	/// 以降のGetChildrenCoreの結果に含まれないこと。
	/// </summary>
	[WpfFact]
	public void GetChildrenCore_項目数が減ると範囲外のIndexは結果に含まれない()
	{
		var control = new MemoListControl
		{
			ItemsSource = [new Memo("id-1", "A", string.Empty, DateTime.Now), new Memo("id-2", "B", string.Empty, DateTime.Now)],
		};
		var peer = CreatePeer(control);
		InvokeGetChildrenCore(peer);

		control.ItemsSource = [new Memo("id-1", "A", string.Empty, DateTime.Now)];
		var children = InvokeGetChildrenCore(peer);

		Assert.Single(children);
	}

	/// <summary>
	/// パス条件: CreateItemPeerを同じIndexで2回呼び出すと、同一インスタンスを返すこと
	/// (選択変更のたびに<see cref="Controls.MemoListControl"/>から呼ばれるため)。
	/// </summary>
	[WpfFact]
	public void CreateItemPeer_同じIndexを2回呼び出すと同一インスタンスを返す()
	{
		var control = new MemoListControl
		{
			ItemsSource = [new Memo("id-1", "A", string.Empty, DateTime.Now)],
		};
		var peer = CreatePeer(control);

		var first = peer.CreateItemPeer(0);
		var second = peer.CreateItemPeer(0);

		Assert.Same(first, second);
	}
}
