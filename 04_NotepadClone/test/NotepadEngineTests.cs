namespace NotepadClone.Tests;

/// <summary>
/// <see cref="NotepadEngine"/> のファイルパス・未保存状態管理・ウィンドウタイトル生成に関するテスト。
/// </summary>
public class NotepadEngineTests
{
	/// <summary>
	/// パス条件: 何も編集していない初期状態で FilePath が null であること。
	/// </summary>
	[Fact]
	public void InitialFilePath_IsNull()
	{
		var engine = new NotepadEngine();

		Assert.Null(engine.FilePath);
	}

	/// <summary>
	/// パス条件: 何も編集していない初期状態で IsDirty が false であること。
	/// </summary>
	[Fact]
	public void InitialIsDirty_IsFalse()
	{
		var engine = new NotepadEngine();

		Assert.False(engine.IsDirty);
	}

	/// <summary>
	/// パス条件: 何も編集していない初期状態で GetWindowTitle が「無題 - メモ帳」を返すこと。
	/// </summary>
	[Fact]
	public void InitialTitle_IsUntitledWithoutMark()
	{
		var engine = new NotepadEngine();

		Assert.Equal("無題 - メモ帳", engine.GetWindowTitle());
	}

	/// <summary>
	/// パス条件: MarkDirty を呼ぶと IsDirty が true になること。
	/// </summary>
	[Fact]
	public void MarkDirty_SetsIsDirtyTrue()
	{
		var engine = new NotepadEngine();

		engine.MarkDirty();

		Assert.True(engine.IsDirty);
	}

	/// <summary>
	/// パス条件: MarkDirty を呼んだ後の GetWindowTitle が未保存マーク付きで「無題」を返すこと。
	/// </summary>
	[Fact]
	public void MarkDirty_TitleShowsDirtyMark()
	{
		var engine = new NotepadEngine();

		engine.MarkDirty();

		Assert.Equal("*無題 - メモ帳", engine.GetWindowTitle());
	}

	/// <summary>
	/// パス条件: Load を呼ぶと FilePath が読み込んだファイルのパスに更新され、IsDirty が false になること。
	/// </summary>
	[Fact]
	public void Load_SetsFilePathAndClearsDirty()
	{
		var engine = new NotepadEngine();
		engine.MarkDirty();

		engine.Load(@"C:\memo\日記.txt");

		Assert.Equal(@"C:\memo\日記.txt", engine.FilePath);
		Assert.False(engine.IsDirty);
	}

	/// <summary>
	/// パス条件: Load を呼んだ後の GetWindowTitle が、パスから抽出したファイル名を未保存マークなしで表示すること。
	/// </summary>
	[Fact]
	public void Load_TitleShowsFileNameWithoutMark()
	{
		var engine = new NotepadEngine();

		engine.Load(@"C:\memo\日記.txt");

		Assert.Equal("日記.txt - メモ帳", engine.GetWindowTitle());
	}

	/// <summary>
	/// パス条件: MarkSaved を呼ぶと FilePath が保存先パスに更新され、IsDirty が false になること。
	/// </summary>
	[Fact]
	public void MarkSaved_SetsFilePathAndClearsDirty()
	{
		var engine = new NotepadEngine();
		engine.MarkDirty();

		engine.MarkSaved(@"C:\memo\新規.txt");

		Assert.Equal(@"C:\memo\新規.txt", engine.FilePath);
		Assert.False(engine.IsDirty);
	}

	/// <summary>
	/// パス条件: MarkSaved を呼んだ後の GetWindowTitle が未保存マークなしでファイル名を表示すること。
	/// </summary>
	[Fact]
	public void MarkSaved_TitleShowsFileNameWithoutMark()
	{
		var engine = new NotepadEngine();
		engine.MarkDirty();

		engine.MarkSaved(@"C:\memo\新規.txt");

		Assert.Equal("新規.txt - メモ帳", engine.GetWindowTitle());
	}

	/// <summary>
	/// パス条件: New を呼ぶと FilePath/IsDirty が初期状態にリセットされること。
	/// </summary>
	[Fact]
	public void New_ResetsFilePathAndDirty()
	{
		var engine = new NotepadEngine();
		engine.Load(@"C:\memo\日記.txt");
		engine.MarkDirty();

		engine.New();

		Assert.Null(engine.FilePath);
		Assert.False(engine.IsDirty);
	}

	/// <summary>
	/// パス条件: 未保存の変更がある状態で New を呼んだ後の GetWindowTitle が、
	/// 未保存マークなしで「無題 - メモ帳」を返すこと。
	/// </summary>
	[Fact]
	public void New_TitleReturnsUntitledWithoutMark()
	{
		var engine = new NotepadEngine();
		engine.MarkDirty();

		engine.New();

		Assert.Equal("無題 - メモ帳", engine.GetWindowTitle());
	}

	/// <summary>
	/// パス条件: Load でファイルを開いた後に MarkDirty で編集すると、
	/// GetWindowTitle がファイル名付きで未保存マークを表示すること。
	/// </summary>
	[Fact]
	public void MarkDirty_AfterLoad_TitleShowsFileNameWithMark()
	{
		var engine = new NotepadEngine();
		engine.Load(@"C:\memo\日記.txt");

		engine.MarkDirty();

		Assert.Equal("*日記.txt - メモ帳", engine.GetWindowTitle());
	}
}
