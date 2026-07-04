using System;

namespace DSPCore;

/// <summary>
/// 提供 TechProto 的作者侧链式注册扩展。
/// Provides author-facing chainable registration extensions for TechProto.
/// </summary>
public static class TechProtoAuthoringExtensions
{
    /// <summary>
    /// 设置科技图标标签。
    /// Sets the tech icon tag.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="iconTag">图标标签。Icon tag.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto SetIconTag(this TechProto tech, string iconTag)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        tech.IconTag = iconTag;
        return tech;
    }

    /// <summary>
    /// 设置科技是否为隐藏科技。
    /// Sets whether the tech is hidden.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="hidden">是否隐藏。Whether the tech is hidden.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto SetHidden(this TechProto tech, bool hidden = true)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        tech.IsHiddenTech = hidden;
        return tech;
    }

    /// <summary>
    /// 设置隐式前置科技。
    /// Sets implicit prerequisite techs.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="techIds">隐式前置科技 ID。Implicit prerequisite tech ids.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto SetPreTechsImplicit(this TechProto tech, params int[] techIds)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        tech.PreTechsImplicit = CopyOrEmpty(techIds);
        return tech;
    }

    /// <summary>
    /// 追加隐式前置科技。
    /// Appends implicit prerequisite techs.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="techIds">要追加的隐式前置科技 ID。Implicit prerequisite tech ids to append.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto AddPreTechsImplicit(this TechProto tech, params int[] techIds)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        tech.PreTechsImplicit = Append(tech.PreTechsImplicit, techIds);
        return tech;
    }

    /// <summary>
    /// 设置科技完成后发放的物品奖励。
    /// Sets item awards granted when the tech completes.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="itemIds">物品 ID。Item ids.</param>
    /// <param name="counts">物品数量。Item counts.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto SetAddItems(this TechProto tech, int[] itemIds, int[] counts)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        EnsureSameLength(itemIds, counts, nameof(counts));
        tech.AddItems = CopyOrEmpty(itemIds);
        tech.AddItemCounts = CopyOrEmpty(counts);
        return tech;
    }

    /// <summary>
    /// 设置科技完成后发放的物品奖励。
    /// Sets item awards granted when the tech completes.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="itemIds">物品 ID。Item ids.</param>
    /// <param name="counts">物品数量。Item counts.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto GrantItems(this TechProto tech, int[] itemIds, int[] counts)
    {
        return tech.SetAddItems(itemIds, counts);
    }

    /// <summary>
    /// 设置科技属性覆盖物品。
    /// Sets item costs used for tech property override calculation.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="itemIds">物品 ID。Item ids.</param>
    /// <param name="counts">物品数量。Item counts.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto SetPropertyOverrideItems(this TechProto tech, int[] itemIds, int[] counts)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        EnsureSameLength(itemIds, counts, nameof(counts));
        tech.PropertyOverrideItems = CopyOrEmpty(itemIds);
        tech.PropertyItemCounts = CopyOrEmpty(counts);
        return tech;
    }

    /// <summary>
    /// 注册当前科技原型。
    /// Registers the current tech proto.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto RegisterTech(this TechProto tech, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        return Techs.Register(tech, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 使用稳定身份注册当前科技原型。
    /// Registers the current tech proto with a stable identity.
    /// </summary>
    /// <param name="tech">科技原型。Tech proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="stableId">稳定 Proto 身份。Stable proto identity.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto RegisterTech(this TechProto tech, string ownerModGuid, ProtoStableId stableId, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (tech == null)
        {
            throw new ArgumentNullException(nameof(tech));
        }

        return Techs.Register(tech, ownerModGuid, stableId, phase, purpose);
    }

    private static int[] CopyOrEmpty(int[]? values)
    {
        if (values == null || values.Length == 0)
        {
            return Array.Empty<int>();
        }

        var copy = new int[values.Length];
        Array.Copy(values, copy, values.Length);
        return copy;
    }

    private static int[] Append(int[]? current, int[]? values)
    {
        if (values == null || values.Length == 0)
        {
            return CopyOrEmpty(current);
        }

        if (current == null || current.Length == 0)
        {
            return CopyOrEmpty(values);
        }

        var merged = new int[current.Length + values.Length];
        Array.Copy(current, merged, current.Length);
        Array.Copy(values, 0, merged, current.Length, values.Length);
        return merged;
    }

    private static void EnsureSameLength(int[]? itemIds, int[]? counts, string countsParameterName)
    {
        int itemLength = itemIds?.Length ?? 0;
        int countLength = counts?.Length ?? 0;
        if (itemLength != countLength)
        {
            throw new ArgumentException("Item ids and counts must have the same length.", countsParameterName);
        }
    }
}
