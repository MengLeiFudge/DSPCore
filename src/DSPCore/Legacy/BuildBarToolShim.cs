namespace BuildBarTool;

/// <summary>
/// 旧 BuildBarTool 兼容入口；请迁移到 DSPCore.DspCore.BuildBar。
/// Legacy BuildBarTool compatibility entry point; migrate to DSPCore.DspCore.BuildBar.
/// </summary>
[System.Obsolete("Use DSPCore.DspCore.BuildBar instead.")]
public static class BuildBarTool
{
    /// <summary>
    /// 旧 SetBuildBar API；请迁移到 DSPCore.DspCore.BuildBar.SetBuildBar。
    /// Legacy SetBuildBar API; migrate to DSPCore.DspCore.BuildBar.SetBuildBar.
    /// </summary>
    /// <param name="category">建造分类。Build category.</param>
    /// <param name="index">按钮索引。Button index.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="isTopRow">是否为扩展上层行。Whether the binding targets the extended top row.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    [System.Obsolete("Use DSPCore.DspCore.BuildBar.SetBuildBar with DSPCore.BuildBarTier instead.")]
    public static bool SetBuildBar(int category, int index, int itemId, bool isTopRow)
    {
        return DSPCore.DspCore.BuildBar.SetBuildBar(
            category,
            index,
            itemId,
            isTopRow ? DSPCore.BuildBarTier.Secondary : DSPCore.BuildBarTier.Primary);
    }
}
