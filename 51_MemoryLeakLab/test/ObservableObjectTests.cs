using MemoryLeakLab.ViewModels;

namespace MemoryLeakLab.Tests;

public class ObservableObjectTests
{
	private sealed class TestObject : ObservableObject
	{
		private int _value;

		public int Value
		{
			get => _value;
			set => SetProperty(ref _value, value);
		}
	}

	/// <summary>
	/// パス条件: 値を変更するとPropertyChangedが対象プロパティ名で発火すること。
	/// </summary>
	[Fact]
	public void SetProperty_ValueChanged_RaisesPropertyChanged()
	{
		var target = new TestObject();
		string? raisedPropertyName = null;
		target.PropertyChanged += (_, e) => raisedPropertyName = e.PropertyName;

		target.Value = 1;

		Assert.Equal(nameof(TestObject.Value), raisedPropertyName);
	}

	/// <summary>
	/// パス条件: 同じ値を設定した場合はPropertyChangedが発火しないこと。
	/// </summary>
	[Fact]
	public void SetProperty_SameValue_DoesNotRaisePropertyChanged()
	{
		var target = new TestObject { Value = 1 };
		var raised = false;
		target.PropertyChanged += (_, _) => raised = true;

		target.Value = 1;

		Assert.False(raised);
	}
}
