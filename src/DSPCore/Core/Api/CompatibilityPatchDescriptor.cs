using System;

namespace DSPCore;

/// <summary>
/// 描述一个兼容补丁。
/// Describes a compatibility patch.
/// </summary>
/// <param name="Id">补丁 ID。Patch id.</param>
/// <param name="TargetModGuid">目标模组 GUID。Target mod GUID.</param>
/// <param name="TargetVersionRange">目标版本范围。Target version range.</param>
/// <param name="Reason">补丁原因。Patch reason.</param>
/// <param name="Apply">应用补丁的回调。Callback that applies the patch.</param>
public sealed record CompatibilityPatchDescriptor(
    string Id,
    string TargetModGuid,
    string TargetVersionRange,
    string Reason,
    Action Apply);
