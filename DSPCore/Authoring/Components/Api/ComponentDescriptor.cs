using System;

namespace DSPCore;

/// <summary>
/// 描述一个由 DSPCore 托管的实体组件类型。
/// Describes an entity component type managed by DSPCore.
/// </summary>
public sealed class ComponentDescriptor
{
    /// <summary>
    /// 创建实体组件描述。
    /// Creates an entity component descriptor.
    /// </summary>
    /// <param name="componentId">组件稳定 ID。Stable component ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="factory">组件工厂。Component factory.</param>
    /// <param name="itemId">可选物品 ID 匹配。Optional item ID match.</param>
    /// <param name="modelIndex">可选模型索引匹配。Optional model index match.</param>
    /// <param name="predicate">可选自定义匹配。Optional custom match.</param>
    public ComponentDescriptor(
        string componentId,
        string ownerModGuid,
        Func<PlanetFactory, int, PrefabDesc, int, CoreFactoryComponent> factory,
        int? itemId = null,
        int? modelIndex = null,
        Func<PrefabDesc, bool>? predicate = null)
    {
        ComponentId = componentId;
        OwnerModGuid = ownerModGuid;
        Factory = factory;
        ItemId = itemId;
        ModelIndex = modelIndex;
        Predicate = predicate;
    }

    /// <summary>
    /// 组件稳定 ID，必须在存档中保持不变。
    /// Stable component ID that must remain unchanged in saves.
    /// </summary>
    public string ComponentId { get; }

    /// <summary>
    /// 注册该组件的模组 GUID。
    /// GUID of the mod that registered this component.
    /// </summary>
    public string OwnerModGuid { get; }

    /// <summary>
    /// 可选物品 ID 匹配条件。
    /// Optional item ID match condition.
    /// </summary>
    public int? ItemId { get; }

    /// <summary>
    /// 可选模型索引匹配条件。
    /// Optional model index match condition.
    /// </summary>
    public int? ModelIndex { get; }

    /// <summary>
    /// 可选自定义 PrefabDesc 匹配条件。
    /// Optional custom PrefabDesc match condition.
    /// </summary>
    public Func<PrefabDesc, bool>? Predicate { get; }

    /// <summary>
    /// 创建组件实例的工厂函数。
    /// Factory function that creates a component instance.
    /// </summary>
    public Func<PlanetFactory, int, PrefabDesc, int, CoreFactoryComponent> Factory { get; }

    /// <summary>
    /// 判断该描述是否适用于当前建筑。
    /// Determines whether this descriptor applies to the current building.
    /// </summary>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="entityId">实体 ID。Entity ID.</param>
    /// <param name="desc">预制体描述。Prefab descriptor.</param>
    /// <returns>匹配时返回 true。Returns true when matched.</returns>
    public bool Matches(PlanetFactory factory, int entityId, PrefabDesc desc)
    {
        if (string.IsNullOrWhiteSpace(ComponentId) || desc == null)
        {
            return false;
        }

        if (ItemId.HasValue && factory.entityPool[entityId].protoId != ItemId.Value)
        {
            return false;
        }

        if (ModelIndex.HasValue && desc.modelIndex != ModelIndex.Value)
        {
            return false;
        }

        return Predicate?.Invoke(desc) ?? true;
    }
}
