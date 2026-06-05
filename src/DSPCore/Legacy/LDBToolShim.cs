using System;

namespace xiaoye97;

/// <summary>
/// 旧 LDBTool 兼容入口；请迁移到 DSPCore.Protos。
/// Legacy LDBTool compatibility entry point; migrate to DSPCore.Protos.
/// </summary>
[Obsolete("Use DSPCore.Protos instead. This namespace is kept only for source/binary migration.")]
public static class LDBTool
{
    /// <summary>
    /// 旧 PreAddProto API；请迁移到 DSPCore.Protos.Register 或类型化注册方法。
    /// Legacy PreAddProto API; migrate to DSPCore.Protos.Register or typed registration methods.
    /// </summary>
    /// <param name="proto">Proto 对象。Proto object.</param>
    [Obsolete("Use DSPCore.Protos.Register(..., CoreDataPhase.Data, ...) or typed registration methods instead.")]
    public static void PreAddProto(object proto)
    {
        DSPCore.Protos.RegisterPreload(proto.GetType(), proto, "legacy.ldbtool");
    }

    /// <summary>
    /// 旧 PostAddProto API；请迁移到 DSPCore.Protos.Register 或类型化注册方法。
    /// Legacy PostAddProto API; migrate to DSPCore.Protos.Register or typed registration methods.
    /// </summary>
    /// <param name="proto">Proto 对象。Proto object.</param>
    [Obsolete("Use DSPCore.Protos.Register(..., CoreDataPhase.DataFinalFixes, ...) or typed registration methods instead.")]
    public static void PostAddProto(object proto)
    {
        DSPCore.Protos.RegisterPostload(proto.GetType(), proto, "legacy.ldbtool");
    }

    /// <summary>
    /// 旧 SetBuildBar API；请迁移到 ItemProto.BindQuickBar 或 DSPCore.BuildBar.BindQuickBar。
    /// Legacy SetBuildBar API; migrate to ItemProto.BindQuickBar or DSPCore.BuildBar.BindQuickBar.
    /// </summary>
    /// <param name="category">建造分类。Build category.</param>
    /// <param name="index">按钮索引。Button index.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    [Obsolete("Use ItemProto.BindQuickBar(tab, row, index) or DSPCore.BuildBar.BindQuickBar(tab, row, index, itemId) instead.")]
    public static void SetBuildBar(int category, int index, int itemId)
    {
        DSPCore.BuildBar.BindQuickBar(category, 1, index, itemId);
    }
}
