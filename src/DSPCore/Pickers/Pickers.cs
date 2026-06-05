using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧选择器入口。
/// Author-facing picker entry point.
/// </summary>
public static class Pickers
{
    /// <summary>
    /// 注册一个选择器请求，真实 UI 弹出由运行时适配层完成。
    /// Registers a picker request; the runtime adapter opens the real UI.
    /// </summary>
    public static void Register(PickerRequest request)
    {
        DspCore.Pickers.Register(request);
    }

    /// <summary>
    /// 获取所有选择器请求。
    /// Gets all picker requests.
    /// </summary>
    public static IReadOnlyList<PickerRequest> GetAll()
    {
        return DspCore.Pickers.GetAll();
    }
}
