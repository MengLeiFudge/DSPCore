using System;

namespace DSPCore;

/// <summary>
/// 蓝图和建筑参数能力的短入口。
/// Short entry point for blueprint and building parameter capabilities.
/// </summary>
public static class Blueprints
{
    /// <summary>
    /// 注册一个使用整数负载的建筑参数块适配器。
    /// Registers a building parameter block adapter using integer payloads.
    /// </summary>
    /// <param name="blockId">参数块稳定 ID。Stable block ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="copy">从实体或预建筑复制整数负载。Copies integer payload from an entity or prebuild.</param>
    /// <param name="paste">把整数负载粘贴到实体。Pastes integer payload to an entity.</param>
    /// <param name="canPaste">可选粘贴检查。Optional paste check.</param>
    /// <param name="applyPrebuild">可选预建筑落地回调。Optional prebuild apply callback.</param>
    public static void Register(
        string blockId,
        string ownerModGuid,
        Func<PlanetFactory, int, int[]?> copy,
        Action<PlanetFactory, int, int[]> paste,
        Func<PlanetFactory, int, int[], bool>? canPaste = null,
        Action<PlanetFactory, int, int[]>? applyPrebuild = null)
    {
        Register(new BuildingParameterDescriptor(
            blockId,
            ownerModGuid,
            (factory, objectId) =>
            {
                var data = copy(factory, objectId);
                return data == null ? null : new BuildingParameterBlock(blockId, data);
            },
            (factory, entityId, block) => paste(factory, entityId, block.Data),
            canPaste == null ? null : (factory, entityId, block) => canPaste(factory, entityId, block.Data),
            applyPrebuild == null ? null : (factory, entityId, block) => applyPrebuild(factory, entityId, block.Data)));
    }

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
