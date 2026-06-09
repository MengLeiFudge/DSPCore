using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧条件补丁声明入口。
/// Author-facing conditional patch declaration entry point.
/// </summary>
public static class Patches
{
    /// <summary>
    /// 注册一个条件补丁。
    /// Registers a conditional patch.
    /// </summary>
    /// <param name="id">补丁 ID。Patch id.</param>
    /// <param name="ownerModGuid">声明该补丁的模组 GUID。GUID of the mod that declares the patch.</param>
    /// <param name="apply">应用补丁的回调。Callback that applies the patch.</param>
    /// <param name="description">补丁说明。Patch description.</param>
    /// <param name="isEnabled">可选启用条件。Optional enable condition.</param>
    /// <param name="getDisabledReason">可选禁用原因。Optional disabled reason.</param>
    /// <param name="requiredPluginGuid">可选必需插件 GUID。Optional required plugin GUID.</param>
    /// <param name="minimumPluginVersion">可选最低插件版本。Optional minimum plugin version.</param>
    public static void Register(
        string id,
        string ownerModGuid,
        Action apply,
        string description = "",
        Func<bool>? isEnabled = null,
        Func<string?>? getDisabledReason = null,
        string? requiredPluginGuid = null,
        string? minimumPluginVersion = null)
    {
        Register(new PatchDescriptor(
            id,
            ownerModGuid,
            apply,
            description,
            isEnabled,
            getDisabledReason,
            requiredPluginGuid,
            minimumPluginVersion));
    }

    /// <summary>
    /// 注册一个依赖指定插件的条件补丁。
    /// Registers a conditional patch that depends on another plugin.
    /// </summary>
    /// <param name="id">补丁 ID。Patch id.</param>
    /// <param name="ownerModGuid">声明该补丁的模组 GUID。GUID of the mod that declares the patch.</param>
    /// <param name="requiredPluginGuid">必需插件 GUID。Required plugin GUID.</param>
    /// <param name="apply">应用补丁的回调。Callback that applies the patch.</param>
    /// <param name="description">补丁说明。Patch description.</param>
    /// <param name="minimumPluginVersion">可选最低插件版本。Optional minimum plugin version.</param>
    /// <param name="isEnabled">可选启用条件。Optional enable condition.</param>
    /// <param name="getDisabledReason">可选禁用原因。Optional disabled reason.</param>
    public static void RegisterForPlugin(
        string id,
        string ownerModGuid,
        string requiredPluginGuid,
        Action apply,
        string description = "",
        string? minimumPluginVersion = null,
        Func<bool>? isEnabled = null,
        Func<string?>? getDisabledReason = null)
    {
        Register(
            id,
            ownerModGuid,
            apply,
            description,
            isEnabled,
            getDisabledReason,
            requiredPluginGuid,
            minimumPluginVersion);
    }

    /// <summary>
    /// 注册一个补丁描述。
    /// Registers a patch descriptor.
    /// </summary>
    /// <param name="descriptor">补丁描述。Patch descriptor.</param>
    public static void Register(PatchDescriptor descriptor)
    {
        DspCore.Patches.Register(descriptor);
    }

    /// <summary>
    /// 获取所有补丁描述。
    /// Gets all patch descriptors.
    /// </summary>
    /// <returns>补丁描述快照。Snapshot of patch descriptors.</returns>
    public static IReadOnlyList<PatchDescriptor> GetAll()
    {
        return DspCore.Patches.GetAll();
    }
}
