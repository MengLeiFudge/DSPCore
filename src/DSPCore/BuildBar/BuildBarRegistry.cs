using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理建造栏按钮绑定，只负责把物品绑定到指定 tab/row/index 槽位。
/// Manages build bar button bindings and only maps items to tab/row/index slots.
/// </summary>
public sealed class BuildBarRegistry
{
    private readonly Dictionary<BuildBarSlot, int> bindings = new();

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    /// <param name="tab">建造栏分页/分类，从 1 开始。Build bar tab/category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    public bool BindQuickBar(int tab, int row, int index, int itemId)
    {
        return BindQuickBar(new BuildBarSlot(tab, row, index), itemId);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    /// <param name="slot">建造栏槽位。Build bar slot.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    public bool BindQuickBar(BuildBarSlot slot, int itemId)
    {
        if (slot.Tab < 1 || slot.Row < 1 || slot.Index < 1 || itemId <= 0)
        {
            return false;
        }

        bindings[slot] = itemId;
        return true;
    }

    /// <summary>
    /// 旧 SetBuildBar 兼容入口，将 category/index/layer 映射到 tab/row/index。
    /// Legacy SetBuildBar bridge that maps category/index/layer to tab/row/index.
    /// </summary>
    /// <param name="category">旧建造分类；映射为 tab。Legacy build category; mapped to tab.</param>
    /// <param name="index">旧按钮索引；映射为 index。Legacy button index; mapped to index.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="layer">旧建造栏层级；映射为 row。Legacy build bar layer; mapped to row.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    [System.Obsolete("Use BindQuickBar(tab, row, index, itemId) instead.")]
    public bool SetBuildBar(int category, int index, int itemId, int layer = 1)
    {
        return BindQuickBar(category, layer, index, itemId);
    }

    /// <summary>
    /// 使用旧 BuildBarTool 层级语义设置一个建造栏按钮绑定。
    /// Sets a build bar button binding using legacy BuildBarTool tier semantics.
    /// </summary>
    /// <param name="category">建造分类，从 1 开始。Build category, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="tier">旧建造栏层级。Legacy build bar tier.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    [System.Obsolete("Use BindQuickBar(tab, row, index, itemId) instead.")]
    public bool SetBuildBar(int category, int index, int itemId, BuildBarTier tier)
    {
        return SetBuildBar(category, index, itemId, (int)tier);
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
}
