namespace DSPCore;

/// <summary>
/// 表示蓝图、复制粘贴和预建筑中的 DSPCore tagged 参数块。
/// Represents a DSPCore tagged parameter block in blueprints, copy-paste, and prebuilds.
/// </summary>
/// <param name="BlockId">参数块稳定 ID。Stable block ID.</param>
/// <param name="Data">整数参数负载。Integer payload.</param>
public sealed record BuildingParameterBlock(string BlockId, int[] Data);
