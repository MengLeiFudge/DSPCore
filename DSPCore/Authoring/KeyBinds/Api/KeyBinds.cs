using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧按键绑定入口。
/// Author-facing key binding entry point.
/// </summary>
public static class KeyBinds
{
    /// <summary>
    /// 注册一个可重绑定按键。
    /// Registers a rebindable key binding.
    /// </summary>
    /// <param name="id">按键 ID。Key binding id.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    /// <param name="displayName">显示名称本地化键或文本。Display name localization key or text.</param>
    /// <param name="defaultKey">默认按键文本。Default key text.</param>
    /// <param name="callback">触发回调。Trigger callback.</param>
    /// <param name="action">触发方式。Trigger action.</param>
    /// <param name="conflictGroup">冲突组。Conflict group.</param>
    /// <param name="canOverride">玩家是否可以重绑定。Whether players can rebind it.</param>
    /// <param name="displayPageId">旧显示页 ID，当前不控制玩家 UI。Legacy display page id; it no longer controls player UI.</param>
    public static void Register(
        string id,
        string ownerModGuid,
        string displayName,
        string defaultKey,
        Action callback,
        CoreKeyAction action = CoreKeyAction.Press,
        int conflictGroup = 0,
        bool canOverride = true,
        string? displayPageId = null)
    {
        Register(new KeyBindDescriptor(
            id,
            ownerModGuid,
            displayName,
            defaultKey,
            action,
            conflictGroup,
            canOverride,
            callback,
            displayPageId));
    }

    /// <summary>
    /// 注册一个可重绑定按键。
    /// Registers a rebindable key binding.
    /// </summary>
    public static void Register(KeyBindDescriptor descriptor)
    {
        DspCore.KeyBinds.Register(descriptor);
        KeyBindRuntime.EnsureRegisteredToVanilla();
    }

    /// <summary>
    /// 获取所有按键声明。
    /// Gets all key binding declarations.
    /// </summary>
    public static IReadOnlyCollection<KeyBindDescriptor> GetAll()
    {
        return DspCore.KeyBinds.GetAll();
    }
}
