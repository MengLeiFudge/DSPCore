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
/// <param name="IsEnabled">可选启用条件。Optional enable condition.</param>
/// <param name="GetDisabledReason">可选禁用原因。Optional disabled reason.</param>
/// <param name="RequiredPluginGuid">可选必需插件 GUID。Optional required plugin GUID.</param>
/// <param name="MinimumPluginVersion">可选最低插件版本。Optional minimum plugin version.</param>
public sealed record PatchDescriptor(
    string Id,
    string OwnerModGuid,
    Action Apply,
    string Description,
    Func<bool>? IsEnabled = null,
    Func<string?>? GetDisabledReason = null,
    string? RequiredPluginGuid = null,
    string? MinimumPluginVersion = null);
