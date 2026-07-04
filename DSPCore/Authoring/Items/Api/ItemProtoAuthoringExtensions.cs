using System;

namespace DSPCore;

/// <summary>
/// 提供 ItemProto 的作者侧链式配置扩展。
/// Provides author-facing chainable configuration extensions for ItemProto.
/// </summary>
public static class ItemProtoAuthoringExtensions
{
    /// <summary>
    /// 设置物品图标标签。
    /// Sets the item's icon tag.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="iconTag">图标标签。Icon tag.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto SetIconTag(this ItemProto item, string iconTag)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        item.IconTag = iconTag;
        return item;
    }

    /// <summary>
    /// 设置原版 UnlockKey 数值。
    /// Sets the vanilla UnlockKey value.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="unlockKey">原版 UnlockKey。Vanilla UnlockKey.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto SetUnlockKey(this ItemProto item, int unlockKey)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        item.UnlockKey = unlockKey;
        return item;
    }

    /// <summary>
    /// 设置物品直接解锁，等价于 UnlockKey = -1。
    /// Makes the item always unlocked, equivalent to UnlockKey = -1.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto UnlockAlways(this ItemProto item)
    {
        return item.SetUnlockKey(-1);
    }

    /// <summary>
    /// 设置物品通过黑雾掉落解锁，等价于 UnlockKey = -2。
    /// Makes the item unlock through enemy drops, equivalent to UnlockKey = -2.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto UnlockByEnemyDrop(this ItemProto item)
    {
        return item.SetUnlockKey(-2);
    }

    /// <summary>
    /// 设置物品跟随另一个物品的解锁状态，等价于 UnlockKey = sourceItemId。
    /// Makes the item follow another item's unlock state, equivalent to UnlockKey = sourceItemId.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="sourceItemId">被跟随的物品 ID。Source item id to follow.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto UnlockLike(this ItemProto item, int sourceItemId)
    {
        if (sourceItemId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sourceItemId), "Source item id must be positive.");
        }

        return item.SetUnlockKey(sourceItemId);
    }

    /// <summary>
    /// 清除特殊 UnlockKey，让物品回到原版配方解锁路径。
    /// Clears the special UnlockKey so the item follows the vanilla recipe unlock path.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto UnlockByRecipe(this ItemProto item)
    {
        return item.SetUnlockKey(0);
    }

    /// <summary>
    /// 设置物品前置科技覆盖。
    /// Sets the item's pre-tech override.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="techId">科技 ID；0 表示清除覆盖。Tech id; 0 clears the override.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto SetPreTechOverride(this ItemProto item, int techId)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (techId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(techId), "Tech id cannot be negative.");
        }

        item.PreTechOverride = techId;
        return item;
    }

    /// <summary>
    /// 设置物品是否使用增产剂增产语义。
    /// Sets whether the item uses proliferator productivity semantics.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="productive">是否增产。Whether the item is productive.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto SetProductive(this ItemProto item, bool productive = true)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        item.Productive = productive;
        return item;
    }

    /// <summary>
    /// 设置物品在分页或选择器中的 GridIndex。
    /// Sets the item's GridIndex for tabs or picker surfaces.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="tab">分页槽位。Tab slot.</param>
    /// <param name="row">行编号。Row number.</param>
    /// <param name="index">格子编号。Cell index.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto SetGridIndex(this ItemProto item, TabSlot tab, int row, int index)
    {
        return item.SetGridIndex(tab.Value, row, index);
    }

    /// <summary>
    /// 设置物品在分页或选择器中的 GridIndex。
    /// Sets the item's GridIndex for tabs or picker surfaces.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="tab">游戏分类编号。Game category value.</param>
    /// <param name="row">行编号。Row number.</param>
    /// <param name="index">格子编号。Cell index.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto SetGridIndex(this ItemProto item, int tab, int row, int index)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        item.GridIndex = GridIndexes.From(tab, row, index);
        return item;
    }

    /// <summary>
    /// 注册当前物品原型。
    /// Registers the current item proto.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto RegisterItem(this ItemProto item, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        Items.Register(item, ownerModGuid, phase, purpose);
        return item;
    }

    /// <summary>
    /// 使用稳定身份注册当前物品原型。
    /// Registers the current item proto with a stable identity.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="stableId">稳定 Proto 身份。Stable proto identity.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto RegisterItem(this ItemProto item, string ownerModGuid, ProtoStableId stableId, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        Items.Register(item, ownerModGuid, stableId, phase, purpose);
        return item;
    }

    /// <summary>
    /// 注册一个图标并绑定到当前物品。
    /// Registers an icon and binds it to the current item.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto BindIcon(this ItemProto item, string id, string ownerModGuid, string assetPath, string? fallbackIconId = null)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        Icons.BindToProto(id, ownerModGuid, assetPath, ProtoKind.Item, item.ID, fallbackIconId);
        return item;
    }

    /// <summary>
    /// 通过资源包注册一个图标并绑定到当前物品。
    /// Registers an icon through a resource pack and binds it to the current item.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="pack">资源包短入口。Resource-pack short entry.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto BindIcon(this ItemProto item, ModResourcePack pack, string id, string assetPath, string? fallbackIconId = null)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (pack == null)
        {
            throw new ArgumentNullException(nameof(pack));
        }

        pack.BindIconToProto(id, assetPath, ProtoKind.Item, item.ID, fallbackIconId);
        return item;
    }

    /// <summary>
    /// 通过资源包注册一个嵌入 PNG 图标并绑定到当前物品。
    /// Registers an embedded PNG icon through a resource pack and binds it to the current item.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="pack">资源包短入口。Resource-pack short entry.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="resourceName">manifest resource name。Manifest resource name.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto BindEmbeddedIcon(this ItemProto item, ModResourcePack pack, string id, string resourceName, string? fallbackIconId = null)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (pack == null)
        {
            throw new ArgumentNullException(nameof(pack));
        }

        pack.BindEmbeddedIconToProto(id, resourceName, ProtoKind.Item, item.ID, fallbackIconId);
        return item;
    }

    /// <summary>
    /// 通过资源包注册一个 AssetBundle 图标并绑定到当前物品。
    /// Registers an AssetBundle icon through a resource pack and binds it to the current item.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <param name="pack">资源包短入口。Resource-pack short entry.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="bundlePath">AssetBundle 文件路径。AssetBundle file path.</param>
    /// <param name="assetName">AssetBundle 内资源名。Asset name inside the AssetBundle.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个物品原型，便于继续链式调用。The same item proto for chaining.</returns>
    public static ItemProto BindAssetBundleIcon(this ItemProto item, ModResourcePack pack, string id, string bundlePath, string assetName, string? fallbackIconId = null)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (pack == null)
        {
            throw new ArgumentNullException(nameof(pack));
        }

        pack.BindAssetBundleIconToProto(id, bundlePath, assetName, ProtoKind.Item, item.ID, fallbackIconId);
        return item;
    }
}
