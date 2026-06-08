namespace DSPCore;

/// <summary>
/// 蓝图和建筑参数能力的短入口。
/// Short entry point for blueprint and building parameter capabilities.
/// </summary>
public static class Blueprints
{
    /// <summary>
    /// 注册建筑参数块适配器。
    /// Registers a building parameter block adapter.
    /// </summary>
    /// <param name="descriptor">参数块描述。Parameter block descriptor.</param>
    public static void Register(BuildingParameterDescriptor descriptor)
    {
        DspCore.Blueprints.Register(descriptor);
    }
}
