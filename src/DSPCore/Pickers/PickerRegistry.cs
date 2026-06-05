using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理选择器请求和过滤扩展。
/// Manages picker requests and filter extensions.
/// </summary>
public sealed class PickerRegistry
{
    private readonly List<PickerRequest> requests = new();

    /// <summary>
    /// 注册一个选择器请求，真实 UI 弹出由运行时适配层完成。
    /// Registers a picker request; the runtime adapter opens the real UI.
    /// </summary>
    /// <param name="request">选择器请求。Picker request.</param>
    public void Register(PickerRequest request)
    {
        requests.Add(request);
    }

    /// <summary>
    /// 获取所有选择器请求。
    /// Gets all picker requests.
    /// </summary>
    /// <returns>选择器请求快照。Snapshot of picker requests.</returns>
    public IReadOnlyList<PickerRequest> GetAll()
    {
        return requests.ToArray();
    }
}
