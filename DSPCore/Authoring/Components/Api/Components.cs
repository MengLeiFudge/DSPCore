using System;

namespace DSPCore;

/// <summary>
/// 实体组件能力的短入口。
/// Short entry point for entity component capabilities.
/// </summary>
public static class Components
{
    /// <summary>
    /// 注册一个带无参构造函数的实体组件类型。
    /// Registers an entity component type with a parameterless constructor.
    /// </summary>
    /// <typeparam name="TComponent">组件类型。Component type.</typeparam>
    /// <param name="componentId">组件稳定 ID。Stable component ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="itemId">可选物品 ID 匹配。Optional item ID match.</param>
    /// <param name="modelIndex">可选模型索引匹配。Optional model index match.</param>
    /// <param name="predicate">可选自定义匹配。Optional custom match.</param>
    public static void Register<TComponent>(
        string componentId,
        string ownerModGuid,
        int? itemId = null,
        int? modelIndex = null,
        Func<PrefabDesc, bool>? predicate = null)
        where TComponent : CoreFactoryComponent, new()
    {
        Register(new ComponentDescriptor(
            componentId,
            ownerModGuid,
            static (factory, entityId, desc, prebuildId) => new TComponent(),
            itemId,
            modelIndex,
            predicate));
    }

    /// <summary>
    /// 注册实体组件描述。
    /// Registers an entity component descriptor.
    /// </summary>
    /// <param name="descriptor">组件描述。Component descriptor.</param>
    public static void Register(ComponentDescriptor descriptor)
    {
        DspCore.Components.Register(descriptor);
    }

    /// <summary>
    /// 尝试获取指定实体上的组件实例。
    /// Tries to get a component instance on an entity.
    /// </summary>
    /// <typeparam name="T">组件类型。Component type.</typeparam>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="entityId">实体 ID。Entity ID.</param>
    /// <param name="component">组件实例。Component instance.</param>
    /// <returns>存在并类型匹配时返回 true。Returns true when present and type matched.</returns>
    public static bool TryGet<T>(PlanetFactory factory, int entityId, out T component)
        where T : CoreFactoryComponent
    {
        return EntityLifecycleRuntime.TryGet(factory, entityId, out component);
    }
}
