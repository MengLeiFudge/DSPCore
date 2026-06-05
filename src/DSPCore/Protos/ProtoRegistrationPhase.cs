namespace DSPCore;

/// <summary>
/// 定义 Proto 注册阶段。
/// Defines the phase of Proto registration.
/// </summary>
public enum ProtoRegistrationPhase
{
    /// <summary>
    /// LDB 加载前。
    /// Before LDB loading.
    /// </summary>
    Preload,

    /// <summary>
    /// LDB 加载后。
    /// After LDB loading.
    /// </summary>
    Postload
}
