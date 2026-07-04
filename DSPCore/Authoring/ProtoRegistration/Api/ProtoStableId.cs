using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 描述一个跨版本稳定的 Proto 身份，运行时会把它解析为实际 int ID。
/// Describes a cross-version stable proto identity that is resolved to a runtime int ID.
/// </summary>
public sealed class ProtoStableId
{
    /// <summary>
    /// 创建一个稳定 Proto 身份。
    /// Creates a stable proto identity.
    /// </summary>
    /// <param name="key">模组内稳定键。Stable key inside the mod.</param>
    /// <param name="preferredId">优先使用的 int ID；被占用时会自动分配其他 ID。Preferred int ID; another ID is allocated when it is occupied.</param>
    /// <param name="aliases">旧稳定键列表，用于重命名迁移。Old stable keys for rename migration.</param>
    public ProtoStableId(string key, int preferredId = 0, IEnumerable<string>? aliases = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Stable proto key cannot be empty.", nameof(key));
        }

        Key = key.Trim();
        PreferredId = preferredId;
        Aliases = aliases?
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// 模组内稳定键。
    /// Stable key inside the mod.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 优先使用的 int ID；小于等于 0 时只自动分配。
    /// Preferred int ID; values less than or equal to zero mean allocation only.
    /// </summary>
    public int PreferredId { get; }

    /// <summary>
    /// 旧稳定键列表，用于把旧版本身份迁移到当前键。
    /// Old stable keys used to migrate previous identities to the current key.
    /// </summary>
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>
    /// 创建稳定身份的短入口。
    /// Creates a stable identity with a short entry point.
    /// </summary>
    /// <param name="key">模组内稳定键。Stable key inside the mod.</param>
    /// <param name="preferredId">优先使用的 int ID。Preferred int ID.</param>
    /// <param name="aliases">旧稳定键列表。Old stable keys.</param>
    /// <returns>稳定 Proto 身份。Stable proto identity.</returns>
    public static ProtoStableId Of(string key, int preferredId = 0, params string[] aliases)
    {
        return new ProtoStableId(key, preferredId, aliases);
    }
}
