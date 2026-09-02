namespace MazeSolverVisualizer.Services;

/// <summary>
/// 迷路探索の結果。可視化用の訪問順序と、見つかった経路(見つからない場合は<see langword="null"/>)を保持する。
/// </summary>
/// <param name="VisitedOrder">探索でセルを訪問した順序(アニメーション表示に使う)。</param>
/// <param name="Path">スタートからゴールまでの経路。ゴールに到達できなかった場合は<see langword="null"/>。</param>
public record MazeSolverResult(IReadOnlyList<(int X, int Y)> VisitedOrder, IReadOnlyList<(int X, int Y)>? Path);
