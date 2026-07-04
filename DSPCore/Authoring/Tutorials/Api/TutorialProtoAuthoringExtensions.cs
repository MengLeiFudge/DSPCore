using System;

namespace DSPCore;

/// <summary>
/// 提供 TutorialProto 的作者侧链式注册扩展。
/// Provides author-facing chainable registration extensions for TutorialProto.
/// </summary>
public static class TutorialProtoAuthoringExtensions
{
    /// <summary>
    /// 注册当前指引或教程原型。
    /// Registers the current guide or tutorial proto.
    /// </summary>
    /// <param name="tutorial">指引或教程原型。Guide or tutorial proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个指引或教程原型，便于继续链式调用。The same guide or tutorial proto for chaining.</returns>
    public static TutorialProto RegisterTutorial(this TutorialProto tutorial, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (tutorial == null)
        {
            throw new ArgumentNullException(nameof(tutorial));
        }

        return Tutorials.Register(tutorial, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 使用稳定身份注册当前指引或教程原型。
    /// Registers the current guide or tutorial proto with a stable identity.
    /// </summary>
    /// <param name="tutorial">指引或教程原型。Guide or tutorial proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="stableId">稳定 Proto 身份。Stable proto identity.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个指引或教程原型，便于继续链式调用。The same guide or tutorial proto for chaining.</returns>
    public static TutorialProto RegisterTutorial(this TutorialProto tutorial, string ownerModGuid, ProtoStableId stableId, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (tutorial == null)
        {
            throw new ArgumentNullException(nameof(tutorial));
        }

        return Tutorials.Register(tutorial, ownerModGuid, stableId, phase, purpose);
    }
}
