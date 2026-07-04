namespace DSPCore;

/// <summary>
/// 建造栏作者默认绑定请求的结果状态。
/// Result status for an author build bar default binding request.
/// </summary>
public enum BuildBarBindStatus
{
    /// <summary>
    /// 绑定已写入空槽位。
    /// The binding was applied to an empty slot.
    /// </summary>
    Applied = 0,

    /// <summary>
    /// 槽位已经绑定到同一物品。
    /// The slot was already bound to the same item.
    /// </summary>
    AlreadyBound = 1,

    /// <summary>
    /// 槽位已有其他默认绑定，本次请求未覆盖。
    /// The slot had another default binding and this request did not replace it.
    /// </summary>
    Occupied = 2,

    /// <summary>
    /// 槽位已有其他默认绑定，本次请求已显式覆盖。
    /// The slot had another default binding and this request explicitly replaced it.
    /// </summary>
    Replaced = 3,

    /// <summary>
    /// 请求参数非法。
    /// The request arguments were invalid.
    /// </summary>
    Invalid = 4
}
