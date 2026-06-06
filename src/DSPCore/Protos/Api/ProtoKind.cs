namespace DSPCore;

/// <summary>
/// 定义 DSPCore 可管理的原型功能类型。
/// Defines proto feature kinds managed by DSPCore.
/// </summary>
public enum ProtoKind
{
    /// <summary>
    /// 未指定原型类型。
    /// Unspecified proto kind.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 物品原型。
    /// Item proto.
    /// </summary>
    Item,

    /// <summary>
    /// 配方原型。
    /// Recipe proto.
    /// </summary>
    Recipe,

    /// <summary>
    /// 科技原型。
    /// Tech proto.
    /// </summary>
    Tech,

    /// <summary>
    /// 指引或教程原型。
    /// Guide or tutorial proto.
    /// </summary>
    Tutorial,

    /// <summary>
    /// 模型或建筑绑定原型。
    /// Model or building-binding proto.
    /// </summary>
    Model,

    /// <summary>
    /// 信号原型。
    /// Signal proto.
    /// </summary>
    Signal
}
