using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧建造栏入口。
/// Author-facing build bar entry point.
/// </summary>
public static class BuildBar
{
    /// <summary>
    /// 打开 DSPCore 建造栏绑定编辑器。
    /// Opens the DSPCore build bar binding editor.
    /// </summary>
    public static void OpenEditor()
    {
        BuildBarRuntime.OpenOverrideWindow();
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    /// <param name="category">建造栏分类，从 1 开始。Build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <returns>绑定写入当前作者默认绑定时返回 true。Returns true when the binding becomes the current author default.</returns>
    public static bool BindQuickBar(int category, int row, int index, int itemId)
    {
        return DspCore.BuildBar.BindQuickBar(category, row, index, itemId);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    /// <param name="tab">旧名称：建造栏分类，从 1 开始。Old name: build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="compat">兼容参数，请不要传入。Compatibility parameter; do not pass.</param>
    /// <returns>绑定写入当前作者默认绑定时返回 true。Returns true when the binding becomes the current author default.</returns>
    [System.Obsolete("Use BindQuickBar(category, row, index, itemId) instead.")]
    public static bool BindQuickBar(int tab, int row, int index, int itemId, bool compat = true)
    {
        return BindQuickBar(tab, row, index, itemId);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    public static bool BindQuickBar(BuildBarSlot slot, int itemId)
    {
        return DspCore.BuildBar.BindQuickBar(slot, itemId);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位，并返回冲突与覆盖结果。
    /// Binds an item id to a quick build bar slot and returns conflict and replacement details.
    /// </summary>
    /// <param name="category">建造栏分类，从 1 开始。Build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="conflictPolicy">已有默认绑定时的处理策略。Policy used when another default binding already exists.</param>
    /// <returns>结构化绑定结果。Structured binding result.</returns>
    public static BuildBarBindResult BindQuickBarWithResult(
        int category,
        int row,
        int index,
        int itemId,
        BuildBarConflictPolicy conflictPolicy = BuildBarConflictPolicy.KeepExisting)
    {
        return DspCore.BuildBar.BindQuickBarWithResult(category, row, index, itemId, conflictPolicy);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位，并返回冲突与覆盖结果。
    /// Binds an item id to a quick build bar slot and returns conflict and replacement details.
    /// </summary>
    /// <param name="slot">建造栏槽位。Build bar slot.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="conflictPolicy">已有默认绑定时的处理策略。Policy used when another default binding already exists.</param>
    /// <returns>结构化绑定结果。Structured binding result.</returns>
    public static BuildBarBindResult BindQuickBarWithResult(
        BuildBarSlot slot,
        int itemId,
        BuildBarConflictPolicy conflictPolicy = BuildBarConflictPolicy.KeepExisting)
    {
        return DspCore.BuildBar.BindQuickBarWithResult(slot, itemId, conflictPolicy);
    }

    /// <summary>
    /// 获取所有建造栏绑定。
    /// Gets all build bar bindings.
    /// </summary>
    public static IReadOnlyDictionary<BuildBarSlot, int> GetAll()
    {
        return DspCore.BuildBar.GetAll();
    }

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；itemId 为 0 时表示玩家显式清空槽位。
    /// Sets a player-defined build bar override; itemId 0 explicitly empties the slot.
    /// </summary>
    /// <param name="category">建造栏分类，从 1 开始。Build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID；传 0 表示玩家显式清空该槽位。Item id; pass 0 to explicitly empty this slot.</param>
    /// <returns>覆盖被接受时返回 true。Returns true when the override is accepted.</returns>
    public static bool SetPlayerOverride(int category, int row, int index, int itemId)
    {
        return DspCore.BuildBar.SetPlayerOverride(category, row, index, itemId);
    }

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；itemId 为 0 时表示玩家显式清空槽位。
    /// Sets a player-defined build bar override; itemId 0 explicitly empties the slot.
    /// </summary>
    /// <param name="tab">旧名称：建造栏分类，从 1 开始。Old name: build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID；传 0 表示玩家显式清空该槽位。Item id; pass 0 to explicitly empty this slot.</param>
    /// <param name="compat">兼容参数，请不要传入。Compatibility parameter; do not pass.</param>
    /// <returns>覆盖被接受时返回 true。Returns true when the override is accepted.</returns>
    [System.Obsolete("Use SetPlayerOverride(category, row, index, itemId) instead.")]
    public static bool SetPlayerOverride(int tab, int row, int index, int itemId, bool compat = true)
    {
        return SetPlayerOverride(tab, row, index, itemId);
    }

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；itemId 为 0 时表示玩家显式清空槽位。
    /// Sets a player-defined build bar override; itemId 0 explicitly empties the slot.
    /// </summary>
    public static bool SetPlayerOverride(BuildBarSlot slot, int itemId)
    {
        return DspCore.BuildBar.SetPlayerOverride(slot, itemId);
    }

    /// <summary>
    /// 清除一个玩家自定义建造栏覆盖。
    /// Clears a player-defined build bar override.
    /// </summary>
    public static bool ClearPlayerOverride(BuildBarSlot slot)
    {
        return DspCore.BuildBar.ClearPlayerOverride(slot);
    }

    /// <summary>
    /// 获取作者默认绑定叠加玩家覆盖后的有效建造栏绑定。
    /// Gets effective build bar bindings after applying player overrides on top of author defaults.
    /// </summary>
    public static IReadOnlyDictionary<BuildBarSlot, int> GetEffectiveBindings()
    {
        return DspCore.BuildBar.GetEffectiveBindings();
    }
}
