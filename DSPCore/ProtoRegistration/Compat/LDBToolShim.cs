using System;

namespace xiaoye97;

/// <summary>
/// 旧 LDBTool 兼容入口；Proto 入口归 ProtoRegistration/Compat，建造栏入口转交 BuildBar/Compat。
/// Legacy LDBTool compatibility entry point; proto entries live in ProtoRegistration/Compat and build bar entries delegate to BuildBar/Compat.
/// </summary>
[Obsolete("Use DSPCore.ProtoRegistration instead. This namespace is kept only for source/binary migration.")]
public static class LDBTool
{
    /// <summary>
    /// 旧 PreAddProto API；请迁移到 DSPCore.ProtoRegistration.Register 或类型化注册方法。
    /// Legacy PreAddProto API; migrate to DSPCore.ProtoRegistration.Register or typed registration methods.
    /// </summary>
    /// <param name="proto">Proto 对象。Proto object.</param>
    [Obsolete("Use DSPCore.ProtoRegistration.Register(..., CoreDataPhase.Data, ...) or typed registration methods instead.")]
    public static void PreAddProto(object proto)
    {
        DSPCore.DspCore.ProtoRegistration.Register(proto.GetType(), proto, "legacy.ldbtool", DSPCore.CoreDataPhase.Data, DSPCore.ProtoKind.Unknown, "Legacy PreAddProto bridge");
    }

    /// <summary>
    /// 旧 PostAddProto API；请迁移到 DSPCore.ProtoRegistration.Register 或类型化注册方法。
    /// Legacy PostAddProto API; migrate to DSPCore.ProtoRegistration.Register or typed registration methods.
    /// </summary>
    /// <param name="proto">Proto 对象。Proto object.</param>
    [Obsolete("Use DSPCore.ProtoRegistration.Register(..., CoreDataPhase.DataFinalFixes, ...) or typed registration methods instead.")]
    public static void PostAddProto(object proto)
    {
        DSPCore.DspCore.ProtoRegistration.Register(proto.GetType(), proto, "legacy.ldbtool", DSPCore.CoreDataPhase.DataFinalFixes, DSPCore.ProtoKind.Unknown, "Legacy PostAddProto bridge");
    }

    /// <summary>
    /// 旧 SetBuildBar API；请迁移到 ItemProto.SetBuildBar 或 DSPCore.BuildBar.BindQuickBar。
    /// Legacy SetBuildBar API; migrate to ItemProto.SetBuildBar or DSPCore.BuildBar.BindQuickBar.
    /// </summary>
    /// <param name="category">建造分类。Build category.</param>
    /// <param name="index">按钮索引。Button index.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    [Obsolete("Use ItemProto.SetBuildBar(tab, row, index) or DSPCore.BuildBar.BindQuickBar(tab, row, index, itemId) instead.")]
    public static void SetBuildBar(int category, int index, int itemId)
    {
        DSPCore.LegacyBuildBarCompatibility.SetBuildBar(category, index, itemId);
    }
}
