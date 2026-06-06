namespace DSPCore;

/// <summary>
/// 定义按键触发方式。
/// Defines key trigger actions.
/// </summary>
public enum CoreKeyAction
{
    /// <summary>
    /// 按下时触发。
    /// Triggers on press.
    /// </summary>
    Press,

    /// <summary>
    /// 按住时触发。
    /// Triggers while held.
    /// </summary>
    Hold,

    /// <summary>
    /// 释放时触发。
    /// Triggers on release.
    /// </summary>
    Release
}
