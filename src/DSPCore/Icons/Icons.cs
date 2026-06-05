using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧图标注册入口。
/// Author-facing icon registration entry point.
/// </summary>
public static class Icons
{
    /// <summary>
    /// 注册一个图标描述。
    /// Registers an icon descriptor.
    /// </summary>
    public static void Register(IconDescriptor descriptor)
    {
        DspCore.Icons.Register(descriptor);
    }

    /// <summary>
    /// 尝试按图标 ID 获取图标描述。
    /// Tries to get an icon descriptor by icon id.
    /// </summary>
    public static bool TryGet(string id, out IconDescriptor descriptor)
    {
        return DspCore.Icons.TryGet(id, out descriptor);
    }

    /// <summary>
    /// 获取所有图标描述。
    /// Gets all icon descriptors.
    /// </summary>
    public static IReadOnlyCollection<IconDescriptor> GetAll()
    {
        return DspCore.Icons.GetAll();
    }
}
