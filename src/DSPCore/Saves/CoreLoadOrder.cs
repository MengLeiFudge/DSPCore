namespace DSPCore;

/// <summary>
/// 定义模块化存档导入时机。
/// Defines when modular save data should be imported.
/// </summary>
public enum CoreLoadOrder
{
    /// <summary>
    /// 在游戏主要加载流程前导入。
    /// Imports before the main game loading flow.
    /// </summary>
    Preload,

    /// <summary>
    /// 在游戏主要加载流程后导入。
    /// Imports after the main game loading flow.
    /// </summary>
    Postload
}
