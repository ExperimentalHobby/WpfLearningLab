using AccessibleNoteApp.Controls;
using AccessibleNoteApp.Models;

namespace AccessibleNoteApp.Tests;

/// <summary>
/// <see cref="MemoListControl"/> の単体テスト。実際にインスタンスを生成して検証する。
/// </summary>
public class MemoListControlTests
{
	/// <summary>
	/// パス条件: 初期状態でSelectedIndexが-1(未選択)であること
	/// </summary>
	[WpfFact]
	public void SelectedIndex_初期状態は未選択()
	{
		var control = new MemoListControl();

		Assert.Equal(-1, control.SelectedIndex);
	}

	/// <summary>
	/// パス条件: ItemsSourceを設定すると、ItemCountに反映されること
	/// </summary>
	[WpfFact]
	public void ItemsSource_設定するとItemCountに反映される()
	{
		var control = new MemoListControl
		{
			ItemsSource = [new Memo("id-1", "A", string.Empty, DateTime.Now), new Memo("id-2", "B", string.Empty, DateTime.Now)],
		};

		Assert.Equal(2, control.ItemCount);
	}

	/// <summary>
	/// パス条件: GetItemTitleで指定したIndexのメモタイトルが取得できること
	/// </summary>
	[WpfFact]
	public void GetItemTitle_指定したIndexのタイトルを返す()
	{
		var control = new MemoListControl
		{
			ItemsSource = [new Memo("id-1", "最初のメモ", string.Empty, DateTime.Now)],
		};

		var title = control.GetItemTitle(0);

		Assert.Equal("最初のメモ", title);
	}

	/// <summary>
	/// パス条件: 範囲外のIndexをGetItemTitleに渡した場合nullを返すこと
	/// </summary>
	[WpfFact]
	public void GetItemTitle_範囲外のIndexの場合nullを返す()
	{
		var control = new MemoListControl();

		var title = control.GetItemTitle(5);

		Assert.Null(title);
	}

	/// <summary>
	/// パス条件: OnCreateAutomationPeerに相当する自動化ピア生成が、MemoListControlAutomationPeerを返すこと
	/// </summary>
	[WpfFact]
	public void CreateAutomationPeer_MemoListControlAutomationPeerを返す()
	{
		var control = new MemoListControl();

		var peer = System.Windows.Automation.Peers.UIElementAutomationPeer.CreatePeerForElement(control);

		Assert.IsType<MemoListControlAutomationPeer>(peer);
	}
}
