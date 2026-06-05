using System;

namespace DSPCore;

/// <summary>
/// 描述一个可由 DSPCore 管理的补丁。
/// Describes a patch managed by DSPCore.
/// </summary>
/// <param name="Id">补丁 ID。Patch id.</param>
/// <param name="OwnerModGuid">声明该补丁的模组 GUID。GUID of the mod that declares the patch.</param>
/// <param name="Apply">应用补丁的回调。Callback that applies the patch.</param>
/// <param name="Description">补丁说明。Patch description.</param>
public sealed record PatchDescriptor(
    string Id,
    string OwnerModGuid,
    Action Apply,
    string Description);
