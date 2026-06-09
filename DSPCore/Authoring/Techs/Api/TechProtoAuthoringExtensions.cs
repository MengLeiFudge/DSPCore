using System;

namespace DSPCore;

/// <summary>
/// 提供 TechProto 的作者侧链式注册扩展。
/// Provides author-facing chainable registration extensions for TechProto.
/// </summary>
public static class TechProtoAuthoringExtensions
{
    /// <summary>
    /// 注册当前科技原型。
    /// Registers the current tech proto.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto RegisterTech(this TechProto tech, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        return Techs.Register(tech, ownerModGuid, phase, purpose);
    }
}
