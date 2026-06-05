using System;

namespace xiaoye97;

/// <summary>
/// 旧 LDBTool 兼容入口；请迁移到 DSPCore.DspCore.Protos。
/// Legacy LDBTool compatibility entry point; migrate to DSPCore.DspCore.Protos.
/// </summary>
[Obsolete("Use DSPCore.DspCore.Protos instead. This namespace is kept only for source/binary migration.")]
public static class LDBTool
{
    /// <summary>
    /// 旧 PreAddProto API；请迁移到 DSPCore.DspCore.Protos.RegisterPreload。
    /// Legacy PreAddProto API; migrate to DSPCore.DspCore.Protos.RegisterPreload.
    /// </summary>
    /// <param name="proto">Proto 对象。Proto object.</param>
    [Obsolete("Use DSPCore.DspCore.Protos.RegisterPreload(proto.GetType(), proto, ownerModGuid) instead.")]
    public static void PreAddProto(object proto)
    {
        DSPCore.DspCore.Protos.RegisterPreload(proto.GetType(), proto, "legacy.ldbtool");
    }

    /// <summary>
    /// 旧 PostAddProto API；请迁移到 DSPCore.DspCore.Protos.RegisterPostload。
    /// Legacy PostAddProto API; migrate to DSPCore.DspCore.Protos.RegisterPostload.
    /// </summary>
    /// <param name="proto">Proto 对象。Proto object.</param>
    [Obsolete("Use DSPCore.DspCore.Protos.RegisterPostload(proto.GetType(), proto, ownerModGuid) instead.")]
    public static void PostAddProto(object proto)
    {
        DSPCore.DspCore.Protos.RegisterPostload(proto.GetType(), proto, "legacy.ldbtool");
    }

    /// <summary>
    /// 旧 SetBuildBar API；请迁移到 DSPCore.DspCore.BuildBar.SetBuildBar。
    /// Legacy SetBuildBar API; migrate to DSPCore.DspCore.BuildBar.SetBuildBar.
    /// </summary>
    /// <param name="category">建造分类。Build category.</param>
    /// <param name="index">按钮索引。Button index.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    [Obsolete("Use DSPCore.DspCore.BuildBar.SetBuildBar(category, index, itemId) instead.")]
    public static void SetBuildBar(int category, int index, int itemId)
    {
        DSPCore.DspCore.BuildBar.SetBuildBar(category, index, itemId);
    }
}
