namespace DSPCore;

/// <summary>
/// 提供 ItemProto 的快捷建造栏绑定扩展方法。
/// Provides ItemProto extension methods for quick build bar binding.
/// </summary>
public static class ItemProtoQuickBarExtensions
{
    /// <summary>
    /// 把当前物品原型绑定到快捷建造栏槽位。
    /// Binds the current item proto to a quick build bar slot.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="category">建造栏分类，从 1 开始。Build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <returns>绑定写入当前作者默认绑定时返回 true。Returns true when the binding becomes the current author default.</returns>
    public static bool SetBuildBar(this ItemProto item, int category, int row, int index)
    {
        return item != null && BuildBar.BindQuickBar(category, row, index, item.ID);
    }

    /// <summary>
    /// 把当前物品原型绑定到快捷建造栏槽位，并返回冲突与覆盖结果。
    /// Binds the current item proto to a quick build bar slot and returns conflict and replacement details.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="category">建造栏分类，从 1 开始。Build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="conflictPolicy">已有默认绑定时的处理策略。Policy used when another default binding already exists.</param>
    /// <returns>结构化绑定结果。Structured binding result.</returns>
    public static BuildBarBindResult SetBuildBarWithResult(
        this ItemProto item,
        int category,
        int row,
        int index,
        BuildBarConflictPolicy conflictPolicy = BuildBarConflictPolicy.KeepExisting)
    {
        return item == null
            ? BuildBarBindResult.Invalid(new BuildBarSlot(category, row, index), 0)
            : BuildBar.BindQuickBarWithResult(category, row, index, item.ID, conflictPolicy);
    }

    /// <summary>
    /// 把当前物品原型绑定到快捷建造栏槽位。
    /// Binds the current item proto to a quick build bar slot.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="tab">旧名称：建造栏分类，从 1 开始。Old name: build bar category, starting from 1.</param>
    /// <param name="row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="compat">兼容参数，请不要传入。Compatibility parameter; do not pass.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    [System.Obsolete("Use SetBuildBar(category, row, index) instead.")]
    public static bool SetBuildBar(this ItemProto item, int tab, int row, int index, bool compat = true)
    {
        return item.SetBuildBar(tab, row, index);
    }

    /// <summary>
    /// 使用 BuildIndex 风格的位置把当前物品原型绑定到快捷建造栏槽位。
    /// Binds the current item proto to a quick build bar slot using a BuildIndex-style position.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="buildIndex">BuildIndex 风格位置，category * 100 + index。BuildIndex-style position, category * 100 + index.</param>
    /// <param name="row">建造栏行号，从 1 开始；row = 2 对应旧 BuildBarTool 上层行。Build bar row, starting from 1; row = 2 maps to the legacy BuildBarTool top row.</param>
    /// <returns>绑定被接受时返回 true。Returns true when the binding is accepted.</returns>
    public static bool SetBuildBar(this ItemProto item, int buildIndex, int row)
    {
        return item.SetBuildBar(buildIndex / 100, row, buildIndex % 100);
    }
}
