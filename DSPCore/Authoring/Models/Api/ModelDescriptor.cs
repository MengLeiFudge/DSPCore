using System;

namespace DSPCore;

/// <summary>
/// 描述一个从原版或其他模型克隆出的模型扩展。
/// Describes a model extension cloned from a vanilla or modded model.
/// </summary>
public sealed class ModelDescriptor
{
    /// <summary>
    /// 创建模型描述。
    /// Creates a model descriptor.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="sourceModelIndex">来源模型索引。Source model index.</param>
    /// <param name="modelIndex">新模型索引。New model index.</param>
    /// <param name="configureModel">模型配置回调。Model configuration callback.</param>
    /// <param name="configurePrefab">预制体配置回调。Prefab configuration callback.</param>
    public ModelDescriptor(
        string ownerModGuid,
        int sourceModelIndex,
        int modelIndex,
        Action<ModelProto>? configureModel = null,
        Action<PrefabDesc>? configurePrefab = null)
    {
        OwnerModGuid = ownerModGuid;
        SourceModelIndex = sourceModelIndex;
        ModelIndex = modelIndex;
        ConfigureModel = configureModel;
        ConfigurePrefab = configurePrefab;
    }

    /// <summary>
    /// 注册该模型的模组 GUID。
    /// GUID of the mod that registered this model.
    /// </summary>
    public string OwnerModGuid { get; }

    /// <summary>
    /// 来源模型索引。
    /// Source model index.
    /// </summary>
    public int SourceModelIndex { get; }

    /// <summary>
    /// 新模型索引。
    /// New model index.
    /// </summary>
    public int ModelIndex { get; }

    /// <summary>
    /// 模型配置回调。
    /// Model configuration callback.
    /// </summary>
    public Action<ModelProto>? ConfigureModel { get; }

    /// <summary>
    /// 预制体配置回调。
    /// Prefab configuration callback.
    /// </summary>
    public Action<PrefabDesc>? ConfigurePrefab { get; }
}
