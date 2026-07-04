using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理建造栏按钮绑定，只负责把物品绑定到指定 category/row/index 槽位。
/// Manages build bar button bindings and only maps items to category/row/index slots.
/// </summary>
public sealed class BuildBarRegistry
{
    private readonly Dictionary<BuildBarSlot, int> bindings = new();
    private readonly Dictionary<BuildBarSlot, int> playerOverrides = new();

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    /// <param name="category">建造栏分类，从 1 开始。Build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <returns>绑定写入当前作者默认绑定时返回 true。Returns true when the binding becomes the current author default.</returns>
    public bool BindQuickBar(int category, int row, int index, int itemId)
    {
        return BindQuickBar(new BuildBarSlot(category, row, index), itemId);
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
    public bool BindQuickBar(int tab, int row, int index, int itemId, bool compat = true)
    {
        return BindQuickBar(tab, row, index, itemId);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    /// <param name="slot">建造栏槽位。Build bar slot.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <returns>绑定写入当前作者默认绑定时返回 true。Returns true when the binding becomes the current author default.</returns>
    public bool BindQuickBar(BuildBarSlot slot, int itemId)
    {
        return BindQuickBarWithResult(slot, itemId).Applied;
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
    public BuildBarBindResult BindQuickBarWithResult(
        int category,
        int row,
        int index,
        int itemId,
        BuildBarConflictPolicy conflictPolicy = BuildBarConflictPolicy.KeepExisting)
    {
        return BindQuickBarWithResult(new BuildBarSlot(category, row, index), itemId, conflictPolicy);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位，并返回冲突与覆盖结果。
    /// Binds an item id to a quick build bar slot and returns conflict and replacement details.
    /// </summary>
    /// <param name="slot">建造栏槽位。Build bar slot.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="conflictPolicy">已有默认绑定时的处理策略。Policy used when another default binding already exists.</param>
    /// <returns>结构化绑定结果。Structured binding result.</returns>
    public BuildBarBindResult BindQuickBarWithResult(
        BuildBarSlot slot,
        int itemId,
        BuildBarConflictPolicy conflictPolicy = BuildBarConflictPolicy.KeepExisting)
    {
        if (slot.Category < 1 || slot.Row < 1 || slot.Index < 1 || itemId <= 0)
        {
            return BuildBarBindResult.Invalid(slot, itemId);
        }

        if (!bindings.TryGetValue(slot, out var existingItemId))
        {
            bindings[slot] = itemId;
            return new BuildBarBindResult(BuildBarBindStatus.Applied, slot, itemId, 0, itemId);
        }

        if (existingItemId == itemId)
        {
            return new BuildBarBindResult(BuildBarBindStatus.AlreadyBound, slot, itemId, existingItemId, existingItemId);
        }

        if (conflictPolicy != BuildBarConflictPolicy.ReplaceExisting)
        {
            return new BuildBarBindResult(BuildBarBindStatus.Occupied, slot, itemId, existingItemId, existingItemId);
        }

        bindings[slot] = itemId;
        return new BuildBarBindResult(BuildBarBindStatus.Replaced, slot, itemId, existingItemId, itemId);
    }

    /// <summary>
    /// 获取所有建造栏绑定。
    /// Gets all build bar bindings.
    /// </summary>
    /// <returns>建造栏绑定快照。Snapshot of build bar bindings.</returns>
    public IReadOnlyDictionary<BuildBarSlot, int> GetAll()
    {
        return new Dictionary<BuildBarSlot, int>(bindings);
    }

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；覆盖优先于作者默认绑定。
    /// Sets a player-defined build bar override; overrides take precedence over author defaults.
    /// </summary>
    /// <param name="slot">建造栏槽位。Build bar slot.</param>
    /// <param name="itemId">物品 ID；传 0 表示玩家显式清空该槽位。Item id; pass 0 to explicitly empty this slot.</param>
    /// <returns>覆盖被接受时返回 true。Returns true when the override is accepted.</returns>
    public bool SetPlayerOverride(BuildBarSlot slot, int itemId)
    {
        if (slot.Category < 1 || slot.Row < 1 || slot.Index < 1 || itemId < 0)
        {
            return false;
        }

        playerOverrides[slot] = itemId;
        return true;
    }

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；覆盖优先于作者默认绑定。
    /// Sets a player-defined build bar override; overrides take precedence over author defaults.
    /// </summary>
    /// <param name="category">建造栏分类，从 1 开始。Build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID；传 0 表示玩家显式清空该槽位。Item id; pass 0 to explicitly empty this slot.</param>
    /// <returns>覆盖被接受时返回 true。Returns true when the override is accepted.</returns>
    public bool SetPlayerOverride(int category, int row, int index, int itemId)
    {
        return SetPlayerOverride(new BuildBarSlot(category, row, index), itemId);
    }

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；覆盖优先于作者默认绑定。
    /// Sets a player-defined build bar override; overrides take precedence over author defaults.
    /// </summary>
    /// <param name="tab">旧名称：建造栏分类，从 1 开始。Old name: build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID；传 0 表示玩家显式清空该槽位。Item id; pass 0 to explicitly empty this slot.</param>
    /// <param name="compat">兼容参数，请不要传入。Compatibility parameter; do not pass.</param>
    /// <returns>覆盖被接受时返回 true。Returns true when the override is accepted.</returns>
    [System.Obsolete("Use SetPlayerOverride(category, row, index, itemId) instead.")]
    public bool SetPlayerOverride(int tab, int row, int index, int itemId, bool compat = true)
    {
        return SetPlayerOverride(tab, row, index, itemId);
    }

    /// <summary>
    /// 清除一个玩家自定义建造栏覆盖。
    /// Clears a player-defined build bar override.
    /// </summary>
    /// <param name="slot">建造栏槽位。Build bar slot.</param>
    /// <returns>存在并移除时返回 true。Returns true when an override was removed.</returns>
    public bool ClearPlayerOverride(BuildBarSlot slot)
    {
        return playerOverrides.Remove(slot);
    }

    /// <summary>
    /// 清除所有玩家自定义建造栏覆盖。
    /// Clears all player-defined build bar overrides.
    /// </summary>
    public void ClearPlayerOverrides()
    {
        playerOverrides.Clear();
    }

    /// <summary>
    /// 获取所有玩家自定义建造栏覆盖。
    /// Gets all player-defined build bar overrides.
    /// </summary>
    /// <returns>玩家覆盖快照。Snapshot of player overrides.</returns>
    public IReadOnlyDictionary<BuildBarSlot, int> GetPlayerOverrides()
    {
        return new Dictionary<BuildBarSlot, int>(playerOverrides);
    }

    /// <summary>
    /// 获取作者默认绑定叠加玩家覆盖后的有效建造栏绑定。
    /// Gets effective build bar bindings after applying player overrides on top of author defaults.
    /// </summary>
    /// <returns>有效绑定快照。Snapshot of effective bindings.</returns>
    public IReadOnlyDictionary<BuildBarSlot, int> GetEffectiveBindings()
    {
        var effective = new Dictionary<BuildBarSlot, int>(bindings);
        foreach (var pair in playerOverrides)
        {
            if (pair.Value == 0)
            {
                effective.Remove(pair.Key);
            }
            else
            {
                effective[pair.Key] = pair.Value;
            }
        }

        return effective;
    }
}
