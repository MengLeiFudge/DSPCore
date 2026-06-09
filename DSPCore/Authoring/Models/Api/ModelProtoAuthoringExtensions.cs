using System;

namespace DSPCore;

/// <summary>
/// 提供 ModelProto 的作者侧链式配置扩展。
/// Provides author-facing chainable configuration extensions for ModelProto.
/// </summary>
public static class ModelProtoAuthoringExtensions
{
    /// <summary>
    /// 声明从当前模型克隆出一个新模型。
    /// Declares a new model cloned from the current model.
    /// </summary>
    /// <param name="source">来源模型原型。Source model proto.</param>
    /// <param name="modelIndex">新模型索引。New model index.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="configureModel">模型配置回调。Model configuration callback.</param>
    /// <param name="configurePrefab">预制体配置回调。Prefab configuration callback.</param>
    /// <returns>同一个来源模型原型，便于继续链式调用。The same source model proto for chaining.</returns>
    public static ModelProto CloneAsModel(
        this ModelProto source,
        int modelIndex,
        string ownerModGuid,
        Action<ModelProto>? configureModel = null,
        Action<PrefabDesc>? configurePrefab = null)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Models.CloneModel(source.ID, modelIndex, ownerModGuid, configureModel, configurePrefab);
        return source;
    }
}
