using System.ComponentModel;
using DragDropFileTagger.Models;

namespace DragDropFileTagger.Tests;

/// <summary>
/// <see cref="TaggedFile"/>のテスト。
/// </summary>
public class TaggedFileTests
{
	/// <summary>
	/// パス条件: Tags.Addだけで(呼び出し側が明示的な通知メソッドを呼ばなくても)、
	/// TagsDisplayのPropertyChangedが発火すること。
	/// </summary>
	[Fact]
	public void TagsAdd_だけでTagsDisplayのPropertyChangedが発火する()
	{
		var file = new TaggedFile();
		var raisedProperties = new List<string?>();
		file.PropertyChanged += (_, e) => raisedProperties.Add(e.PropertyName);

		file.Tags.Add("重要");

		Assert.Contains(nameof(TaggedFile.TagsDisplay), raisedProperties);
		Assert.Equal("重要", file.TagsDisplay);
	}

	/// <summary>
	/// パス条件: Tagsプロパティ自体を別のコレクションに差し替えた場合でも、
	/// 差し替え後のコレクションへのAddでTagsDisplayの変更通知が引き続き発火すること
	/// (JSON復元時にTagsが新しいインスタンスに置き換わるケースを想定)。
	/// </summary>
	[Fact]
	public void Tags差し替え後もAddでTagsDisplayの変更通知が発火する()
	{
		var file = new TaggedFile { Tags = ["既存"] };
		var raised = false;
		file.PropertyChanged += (_, e) =>
		{
			if (e.PropertyName == nameof(TaggedFile.TagsDisplay))
			{
				raised = true;
			}
		};

		file.Tags.Add("追加");

		Assert.True(raised);
	}
}
