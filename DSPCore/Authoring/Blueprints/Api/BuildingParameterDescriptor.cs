using System;

namespace DSPCore;

/// <summary>
/// 描述一个参与建筑复制粘贴和蓝图恢复的参数适配器。
/// Describes a parameter adapter participating in building copy-paste and blueprint restore.
/// </summary>
public sealed class BuildingParameterDescriptor
{
    /// <summary>
    /// 创建建筑参数描述。
    /// Creates a building parameter descriptor.
    /// </summary>
    /// <param name="blockId">参数块稳定 ID。Stable block ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="copy">从实体或预建筑复制参数。Copies parameters from an entity or prebuild.</param>
    /// <param name="paste">把参数粘贴到实体。Pastes parameters to an entity.</param>
    /// <param name="canPaste">可选粘贴检查。Optional paste check.</param>
    /// <param name="applyPrebuild">可选预建筑落地回调。Optional prebuild apply callback.</param>
    public BuildingParameterDescriptor(
        string blockId,
        string ownerModGuid,
        Func<PlanetFactory, int, BuildingParameterBlock?> copy,
        Action<PlanetFactory, int, BuildingParameterBlock> paste,
        Func<PlanetFactory, int, BuildingParameterBlock, bool>? canPaste = null,
        Action<PlanetFactory, int, BuildingParameterBlock>? applyPrebuild = null)
    {
        BlockId = blockId;
        OwnerModGuid = ownerModGuid;
        Copy = copy;
        Paste = paste;
        CanPaste = canPaste;
        ApplyPrebuild = applyPrebuild;
    }

    /// <summary>
    /// 参数块稳定 ID，必须在蓝图和存档中保持不变。
    /// Stable block ID that must remain unchanged in blueprints and saves.
    /// </summary>
    public string BlockId { get; }

    /// <summary>
    /// 注册该适配器的模组 GUID。
    /// GUID of the mod that registered this adapter.
    /// </summary>
    public string OwnerModGuid { get; }

    /// <summary>
    /// 从实体或预建筑复制参数块。
    /// Copies a parameter block from an entity or prebuild.
    /// </summary>
    public Func<PlanetFactory, int, BuildingParameterBlock?> Copy { get; }

    /// <summary>
    /// 将参数块粘贴到实体。
    /// Pastes a parameter block to an entity.
    /// </summary>
    public Action<PlanetFactory, int, BuildingParameterBlock> Paste { get; }

    /// <summary>
    /// 判断参数块是否可粘贴。
    /// Determines whether a parameter block can be pasted.
    /// </summary>
    public Func<PlanetFactory, int, BuildingParameterBlock, bool>? CanPaste { get; }

    /// <summary>
    /// 预建筑落地为实体后应用参数块。
    /// Applies a parameter block after a prebuild becomes an entity.
    /// </summary>
    public Action<PlanetFactory, int, BuildingParameterBlock>? ApplyPrebuild { get; }
}
