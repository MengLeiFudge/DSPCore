using System;

namespace DSPCore;

/// <summary>
/// 提供 RecipeProto 的作者侧链式配置扩展。
/// Provides author-facing chainable configuration extensions for RecipeProto.
/// </summary>
public static class RecipeProtoAuthoringExtensions
{
    /// <summary>
    /// 设置配方在分页或选择器中的 GridIndex。
    /// Sets the recipe's GridIndex for tabs or picker surfaces.
    /// </summary>
    /// <param name="recipe">配方原型。Recipe proto.</param>
    /// <param name="tab">分页槽位。Tab slot.</param>
    /// <param name="row">行编号。Row number.</param>
    /// <param name="index">格子编号。Cell index.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto SetGridIndex(this RecipeProto recipe, TabSlot tab, int row, int index)
    {
        return recipe.SetGridIndex(tab.Value, row, index);
    }

    /// <summary>
    /// 设置配方在分页或选择器中的 GridIndex。
    /// Sets the recipe's GridIndex for tabs or picker surfaces.
    /// </summary>
    /// <param name="recipe">配方原型。Recipe proto.</param>
    /// <param name="tab">游戏分类编号。Game category value.</param>
    /// <param name="row">行编号。Row number.</param>
    /// <param name="index">格子编号。Cell index.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto SetGridIndex(this RecipeProto recipe, int tab, int row, int index)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        recipe.GridIndex = GridIndexes.From(tab, row, index);
        return recipe;
    }

    /// <summary>
    /// 注册当前配方原型。
    /// Registers the current recipe proto.
    /// </summary>
    /// <param name="recipe">配方原型。Recipe proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto RegisterRecipe(this RecipeProto recipe, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        Recipes.Register(recipe, ownerModGuid, phase, purpose);
        return recipe;
    }

    /// <summary>
    /// 注册一个图标并绑定到当前配方。
    /// Registers an icon and binds it to the current recipe.
    /// </summary>
    /// <param name="recipe">配方原型。Recipe proto.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto BindIcon(this RecipeProto recipe, string id, string ownerModGuid, string assetPath, string? fallbackIconId = null)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        Icons.BindToProto(id, ownerModGuid, assetPath, ProtoKind.Recipe, recipe.ID, fallbackIconId);
        return recipe;
    }

    /// <summary>
    /// 通过资源包注册一个图标并绑定到当前配方。
    /// Registers an icon through a resource pack and binds it to the current recipe.
    /// </summary>
    /// <param name="recipe">配方原型。Recipe proto.</param>
    /// <param name="pack">资源包短入口。Resource-pack short entry.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="assetPath">资源路径。Asset path.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto BindIcon(this RecipeProto recipe, ModResourcePack pack, string id, string assetPath, string? fallbackIconId = null)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (pack == null)
        {
            throw new ArgumentNullException(nameof(pack));
        }

        pack.BindIconToProto(id, assetPath, ProtoKind.Recipe, recipe.ID, fallbackIconId);
        return recipe;
    }

    /// <summary>
    /// 通过资源包注册一个嵌入 PNG 图标并绑定到当前配方。
    /// Registers an embedded PNG icon through a resource pack and binds it to the current recipe.
    /// </summary>
    /// <param name="recipe">配方原型。Recipe proto.</param>
    /// <param name="pack">资源包短入口。Resource-pack short entry.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="resourceName">manifest resource name。Manifest resource name.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto BindEmbeddedIcon(this RecipeProto recipe, ModResourcePack pack, string id, string resourceName, string? fallbackIconId = null)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (pack == null)
        {
            throw new ArgumentNullException(nameof(pack));
        }

        pack.BindEmbeddedIconToProto(id, resourceName, ProtoKind.Recipe, recipe.ID, fallbackIconId);
        return recipe;
    }

    /// <summary>
    /// 通过资源包注册一个 AssetBundle 图标并绑定到当前配方。
    /// Registers an AssetBundle icon through a resource pack and binds it to the current recipe.
    /// </summary>
    /// <param name="recipe">配方原型。Recipe proto.</param>
    /// <param name="pack">资源包短入口。Resource-pack short entry.</param>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="bundlePath">AssetBundle 文件路径。AssetBundle file path.</param>
    /// <param name="assetName">AssetBundle 内资源名。Asset name inside the AssetBundle.</param>
    /// <param name="fallbackIconId">可选 fallback 图标 ID。Optional fallback icon id.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto BindAssetBundleIcon(this RecipeProto recipe, ModResourcePack pack, string id, string bundlePath, string assetName, string? fallbackIconId = null)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (pack == null)
        {
            throw new ArgumentNullException(nameof(pack));
        }

        pack.BindAssetBundleIconToProto(id, bundlePath, assetName, ProtoKind.Recipe, recipe.ID, fallbackIconId);
        return recipe;
    }
}
