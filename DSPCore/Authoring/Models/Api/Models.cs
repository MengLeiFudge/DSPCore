using System;

namespace DSPCore;

/// <summary>
/// 模型和预制体扩展能力的短入口。
/// Short entry point for model and prefab augmentation capabilities.
/// </summary>
public static class Models
{
    /// <summary>
    /// 注册一个模型克隆声明。
    /// Registers a model clone declaration.
    /// </summary>
    /// <param name="sourceModelIndex">来源模型索引。Source model index.</param>
    /// <param name="modelIndex">新模型索引。New model index.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="configureModel">模型配置回调。Model configuration callback.</param>
    /// <param name="configurePrefab">预制体配置回调。Prefab configuration callback.</param>
    public static void CloneModel(
        int sourceModelIndex,
        int modelIndex,
        string ownerModGuid,
        Action<ModelProto>? configureModel = null,
        Action<PrefabDesc>? configurePrefab = null)
    {
        Register(new ModelDescriptor(ownerModGuid, sourceModelIndex, modelIndex, configureModel, configurePrefab));
    }

    /// <summary>
    /// 注册一个模型克隆声明。
    /// Registers a model clone declaration.
    /// </summary>
    /// <param name="descriptor">模型描述。Model descriptor.</param>
    public static void Register(ModelDescriptor descriptor)
    {
        DspCore.Models.Register(descriptor);
    }
}
