namespace WeatherApp.Models;

/// <summary>
/// 地名から解決された位置情報。
/// </summary>
public class GeocodingResult
{
	/// <summary>解決された地名。</summary>
	public required string Name { get; init; }

	/// <summary>緯度。</summary>
	public required double Latitude { get; init; }

	/// <summary>経度。</summary>
	public required double Longitude { get; init; }
}
