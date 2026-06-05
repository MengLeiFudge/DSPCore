using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理建造栏按钮绑定，包括旧 BuildBarTool 的第二行语义。
/// Manages build bar button bindings, including the legacy BuildBarTool second-row semantics.
/// </summary>
public sealed class BuildBarRegistry
{
    private readonly Dictionary<BuildBarSlot, int> bindings = new();

    /// <summary>
    /// 设置一个建造栏按钮绑定。
    /// Sets a build bar button binding.
    /// </summary>
    /// <param name="category">建造分类，从 1 开始。Build category, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="layer">建造栏层级，从 1 开始。Build bar layer, starting from 1.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    public bool SetBuildBar(int category, int index, int itemId, int layer = 1)
    {
        if (category < 1 || index < 1 || itemId <= 0 || layer < 1)
        {
            return false;
        }

        bindings[new BuildBarSlot(category, index, layer)] = itemId;
        return true;
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
