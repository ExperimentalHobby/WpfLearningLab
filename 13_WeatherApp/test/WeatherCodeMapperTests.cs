using WeatherApp.Services;

namespace WeatherApp.Tests;

/// <summary>
/// <see cref="WeatherCodeMapper"/> の単体テスト。
/// </summary>
public class WeatherCodeMapperTests
{
	/// <summary>
	/// パス条件: WMO天候コードから正しい日本語の天候名を返すこと
	/// </summary>
	[Theory]
	[InlineData(0, "快晴")]
	[InlineData(2, "一部曇り")]
	[InlineData(61, "雨")]
	[InlineData(71, "雪")]
	[InlineData(95, "雷雨")]
	public void ToDescription_天候コードから正しい天候名を返す(int weatherCode, string expected)
	{
		var description = WeatherCodeMapper.ToDescription(weatherCode);

		Assert.Equal(expected, description);
	}

	/// <summary>
	/// パス条件: 未知の天候コードの場合「不明」を返すこと
	/// </summary>
	[Fact]
	public void ToDescription_未知のコードの場合不明を返す()
	{
		var description = WeatherCodeMapper.ToDescription(-1);

		Assert.Equal("不明", description);
	}
}
