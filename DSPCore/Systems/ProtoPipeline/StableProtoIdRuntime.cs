using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BepInEx;

namespace DSPCore;

internal static class StableProtoIdRuntime
{
    private const int CustomIdStart = 12000;
    private const string MappingRelativePath = "DSPCore/StableProtoIds.tsv";
    private static readonly Dictionary<string, int> Mappings = new(StringComparer.Ordinal);
    private static bool loaded;
    private static bool dirty;

    public static void Resolve(IReadOnlyList<ProtoRegistrationEntry> entries, IReadOnlyCollection<int> existingIds)
    {
        EnsureLoaded();
        var used = new HashSet<int>(existingIds.Where(id => id > 0));
        var claimedStableKeys = new Dictionary<string, ProtoRegistrationEntry>(StringComparer.Ordinal);

        foreach (var entry in entries)
        {
            if (entry.Proto is not Proto proto || entry.StableId == null)
            {
                continue;
            }

            var key = MakeKey(entry.Kind, entry.OwnerModGuid, entry.StableId.Key);
            if (claimedStableKeys.TryGetValue(key, out var previous))
            {
                throw new InvalidOperationException(
                    $"Stable proto key {key} is registered more than once by {previous.OwnerModGuid} and {entry.OwnerModGuid}.");
            }

            claimedStableKeys[key] = entry;
            var resolved = ResolveOne(entry, proto, key, used);
            proto.ID = resolved;
            used.Add(resolved);
        }

        if (dirty)
        {
            Save();
        }
    }

    public static int ResolveForRegistration(ProtoKind kind, string ownerModGuid, ProtoStableId stableId, int currentId, IReadOnlyCollection<int> registeredIds)
    {
        EnsureLoaded();
        var used = new HashSet<int>(registeredIds.Where(id => id > 0));
        var key = MakeKey(kind, ownerModGuid, stableId.Key);
        if (Mappings.TryGetValue(key, out var mapped) && mapped > 0)
        {
            return mapped;
        }

        foreach (var alias in stableId.Aliases)
        {
            var aliasKey = MakeKey(kind, ownerModGuid, alias);
            if (Mappings.TryGetValue(aliasKey, out mapped) && mapped > 0)
            {
                Mappings[key] = mapped;
                dirty = true;
                Save();
                return mapped;
            }
        }

        var preferred = stableId.PreferredId > 0 ? stableId.PreferredId : currentId;
        var resolved = preferred > 0 && !used.Contains(preferred) ? preferred : Allocate(used);
        Mappings[key] = resolved;
        dirty = true;
        Save();
        return resolved;
    }

    private static int ResolveOne(ProtoRegistrationEntry entry, Proto proto, string key, HashSet<int> used)
    {
        if (Mappings.TryGetValue(key, out var mapped) && mapped > 0 && used.Contains(mapped))
        {
            throw new InvalidOperationException($"Stable proto key {key} maps to occupied runtime ID {mapped}.");
        }

        if (mapped > 0 && mapped == proto.ID)
        {
            return mapped;
        }

        if (mapped > 0)
        {
            return mapped;
        }

        foreach (var alias in entry.StableId!.Aliases)
        {
            var aliasKey = MakeKey(entry.Kind, entry.OwnerModGuid, alias);
            if (Mappings.TryGetValue(aliasKey, out mapped) && mapped > 0 && used.Contains(mapped))
            {
                throw new InvalidOperationException($"Stable proto alias {aliasKey} maps to occupied runtime ID {mapped}.");
            }

            if (mapped > 0 && mapped == proto.ID)
            {
                Mappings[key] = mapped;
                dirty = true;
                return mapped;
            }

            if (mapped > 0)
            {
                Mappings[key] = mapped;
                dirty = true;
                return mapped;
            }
        }

        var preferred = entry.StableId.PreferredId > 0 ? entry.StableId.PreferredId : proto.ID;
        var resolved = preferred > 0 && !used.Contains(preferred) ? preferred : Allocate(used);
        Mappings[key] = resolved;
        dirty = true;
        return resolved;
    }

    private static int Allocate(HashSet<int> used)
    {
        var id = Math.Max(CustomIdStart, used.Count == 0 ? CustomIdStart : used.Max() + 1);
        while (used.Contains(id))
        {
            id++;
        }

        return id;
    }

    private static string MakeKey(ProtoKind kind, string ownerModGuid, string stableKey)
    {
        return kind + "\t" + ownerModGuid.Trim() + "\t" + stableKey.Trim();
    }

    private static void EnsureLoaded()
    {
        if (loaded)
        {
            return;
        }

        loaded = true;
        var path = GetPath();
        if (!File.Exists(path))
        {
            return;
        }

        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var parts = line.Split('\t');
            if (parts.Length < 4 || !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                continue;
            }

            Mappings[parts[0] + "\t" + parts[1] + "\t" + parts[2]] = id;
        }
    }

    private static void Save()
    {
        var path = GetPath();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var lines = new List<string> { "# kind\townerModGuid\tstableKey\truntimeId" };
        lines.AddRange(Mappings
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => pair.Key + "\t" + pair.Value.ToString(CultureInfo.InvariantCulture)));
        File.WriteAllLines(path, lines);
        dirty = false;
    }

    private static string GetPath()
    {
        return Path.Combine(Paths.ConfigPath, MappingRelativePath);
    }
}
