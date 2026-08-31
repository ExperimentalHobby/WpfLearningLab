namespace AnimatedDashboard.ViewModels;

/// <summary>
/// 1枚のKPIカードのViewModel。
/// </summary>
public class KpiCardViewModel : ObservableObject
{
	private double _value;

	/// <summary>
	/// <see cref="KpiCardViewModel"/>を初期化する。
	/// </summary>
	public KpiCardViewModel(string name, string unit, double value)
	{
		Name = name;
		Unit = unit;
		_value = value;
	}

	/// <summary>指標名。</summary>
	public string Name { get; }

	/// <summary>単位。</summary>
	public string Unit { get; }

	/// <summary>
	/// 指標の値。この値が変わると、View側の<c>KpiCard</c>がStoryboardでカウントアップアニメーションする。
	/// </summary>
	public double Value { get => _value; set => SetProperty(ref _value, value); }
}
