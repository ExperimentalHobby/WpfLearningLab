namespace WpfLearningLab.Shared.Mvvm.Tests;

/// <summary>
/// <see cref="ObservableObject"/> のテスト用に <c>SetProperty</c>/<c>OnPropertyChanged</c> を公開するダミー実装。
/// </summary>
public class DummyObservableObject : ObservableObject
{
	private string _name = string.Empty;

	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	public void RaiseNameChanged() => OnPropertyChanged(nameof(Name));
}

/// <summary>
/// <see cref="ObservableObject"/> の単体テスト。
/// </summary>
public class ObservableObjectTests
{
	/// <summary>
	/// パス条件: プロパティの値を変更するとPropertyChangedイベントが発火すること
	/// </summary>
	[Fact]
	public void SetProperty_値を変更するとPropertyChangedが発火する()
	{
		var target = new DummyObservableObject();
		var raisedPropertyNames = new List<string>();
		target.PropertyChanged += (_, e) => raisedPropertyNames.Add(e.PropertyName ?? string.Empty);

		target.Name = "東京";

		Assert.Equal(["Name"], raisedPropertyNames);
	}

	/// <summary>
	/// パス条件: 現在の値と同じ値を設定した場合はPropertyChangedが発火しないこと
	/// </summary>
	[Fact]
	public void SetProperty_同じ値を設定するとPropertyChangedが発火しない()
	{
		var target = new DummyObservableObject { Name = "東京" };
		var raisedCount = 0;
		target.PropertyChanged += (_, _) => raisedCount++;

		target.Name = "東京";

		Assert.Equal(0, raisedCount);
	}

	/// <summary>
	/// パス条件: OnPropertyChangedを直接呼ぶと、フィールドの変更なしにPropertyChangedが発火すること
	/// </summary>
	[Fact]
	public void OnPropertyChanged_直接呼ぶとPropertyChangedが発火する()
	{
		var target = new DummyObservableObject();
		var raisedPropertyNames = new List<string>();
		target.PropertyChanged += (_, e) => raisedPropertyNames.Add(e.PropertyName ?? string.Empty);

		target.RaiseNameChanged();

		Assert.Equal(["Name"], raisedPropertyNames);
	}
}
