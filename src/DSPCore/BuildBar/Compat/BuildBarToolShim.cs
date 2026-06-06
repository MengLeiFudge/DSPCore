namespace BuildBarTool;

/// <summary>
/// 旧 BuildBarTool 兼容入口；请迁移到 ItemProto.SetBuildBar 或 DSPCore.BuildBar.BindQuickBar。
/// Legacy BuildBarTool compatibility entry point; migrate to ItemProto.SetBuildBar or DSPCore.BuildBar.BindQuickBar.
/// </summary>
[System.Obsolete("Use DSPCore.BuildBar instead.")]
public static class BuildBarTool
{
    /// <summary>
    /// 旧 SetBuildBar API；请迁移到 ItemProto.SetBuildBar 或 DSPCore.BuildBar.BindQuickBar。
    /// Legacy SetBuildBar API; migrate to ItemProto.SetBuildBar or DSPCore.BuildBar.BindQuickBar.
    /// </summary>
    /// <param name="category">建造分类。Build category.</param>
    /// <param name="index">按钮索引。Button index.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="isTopRow">是否为扩展上层行。Whether the binding targets the extended top row.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    [System.Obsolete("Use ItemProto.SetBuildBar(tab, row, index) or DSPCore.BuildBar.BindQuickBar(tab, row, index, itemId) instead.")]
    public static bool SetBuildBar(int category, int index, int itemId, bool isTopRow)
    {
        return DSPCore.LegacyBuildBarCompatibility.SetBuildBar(category, index, itemId, isTopRow ? 2 : 1);
    }
}
