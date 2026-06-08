using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理模型和预制体扩展声明。
/// Manages model and prefab augmentation declarations.
/// </summary>
public sealed class ModelRegistry
{
    private readonly List<ModelDescriptor> descriptors = new();

    /// <summary>
    /// 注册一个模型克隆声明。
    /// Registers a model clone declaration.
    /// </summary>
    /// <param name="descriptor">模型描述。Model descriptor.</param>
    public void Register(ModelDescriptor descriptor)
    {
        descriptors.RemoveAll(item => item.ModelIndex == descriptor.ModelIndex);
        descriptors.Add(descriptor);
    }

    /// <summary>
    /// 获取所有模型描述快照。
    /// Gets a snapshot of all model descriptors.
    /// </summary>
    /// <returns>模型描述集合。Model descriptor collection.</returns>
    public IReadOnlyList<ModelDescriptor> GetAll()
    {
        return descriptors.ToArray();
    }
}
