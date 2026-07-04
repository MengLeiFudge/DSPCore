namespace DSPCore;

/// <summary>
/// 描述一次建造栏作者默认绑定请求的结构化结果。
/// Describes the structured result of an author build bar default binding request.
/// </summary>
public sealed record BuildBarBindResult(
    BuildBarBindStatus Status,
    BuildBarSlot Slot,
    int RequestedItemId,
    int ExistingItemId,
    int AppliedItemId)
{
    /// <summary>
    /// 本次调用是否把请求的绑定作为当前作者默认绑定保留下来。
    /// Whether this call leaves the requested binding as the current author default binding.
    /// </summary>
    public bool Applied => Status == BuildBarBindStatus.Applied ||
        Status == BuildBarBindStatus.AlreadyBound ||
        Status == BuildBarBindStatus.Replaced;

    /// <summary>
    /// 本次调用是否遇到已有其他作者默认绑定。
    /// Whether this call found another existing author default binding.
    /// </summary>
    public bool HasConflict => Status == BuildBarBindStatus.Occupied ||
        Status == BuildBarBindStatus.Replaced;

    /// <summary>
    /// 创建非法参数结果。
    /// Creates an invalid-argument result.
    /// </summary>
    public static BuildBarBindResult Invalid(BuildBarSlot slot, int requestedItemId)
    {
        return new BuildBarBindResult(BuildBarBindStatus.Invalid, slot, requestedItemId, 0, 0);
    }
}
