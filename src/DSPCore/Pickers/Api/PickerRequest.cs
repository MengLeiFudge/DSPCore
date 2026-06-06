using System;

namespace DSPCore;

/// <summary>
/// 描述一个选择器打开请求。
/// Describes a picker popup request.
/// </summary>
/// <param name="Kind">选择器类型。Picker kind.</param>
/// <param name="OwnerModGuid">调用方模组 GUID。Calling mod GUID.</param>
/// <param name="Filter">过滤器回调。Filter callback.</param>
/// <param name="ShowLocked">是否显示未解锁项。Whether to show locked entries.</param>
/// <param name="ShowAll">是否显示所有项。Whether to show all entries.</param>
/// <param name="OnReturn">选择完成回调。Selection callback.</param>
public sealed record PickerRequest(
    PickerKind Kind,
    string OwnerModGuid,
    Func<object, bool>? Filter = null,
    bool ShowLocked = false,
    bool ShowAll = false,
    Action<object?>? OnReturn = null);
