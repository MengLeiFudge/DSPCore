using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using UnityEngine;

namespace DSPCore;

internal static class ResourceRuntime
{
    private static readonly Dictionary<string, Sprite> SpriteCache = new(StringComparer.Ordinal);

    public static void ApplyIcons()
    {
        foreach (var icon in DspCore.Icons.GetAll())
        {
            if (icon.TargetProtoId <= 0 || icon.TargetKind == ProtoKind.Unknown)
            {
                continue;
            }

            var sprite = ResolveSprite(icon);
            if (sprite == null)
            {
                DspCore.Logger?.LogWarning($"Icon {icon.Id} from {icon.OwnerModGuid} could not be loaded from {icon.AssetPath}.");
                continue;
            }

            var proto = ResolveTargetProto(icon.TargetKind, icon.TargetProtoId);
            if (proto == null)
            {
                DspCore.Logger?.LogWarning($"Icon {icon.Id} target {icon.TargetKind}:{icon.TargetProtoId} does not exist.");
                continue;
            }

            SetIconSprite(proto, sprite);
        }
    }

    public static Sprite? ResolveSprite(IconDescriptor icon)
    {
        if (SpriteCache.TryGetValue(icon.Id, out var cached))
        {
            return cached;
        }

        var sprite = LoadSprite(icon.AssetPath);
        if (sprite == null && icon.FallbackIconId != null && DspCore.Icons.TryGet(icon.FallbackIconId, out var fallback))
        {
            sprite = ResolveSprite(fallback);
        }

        if (sprite != null)
        {
            SpriteCache[icon.Id] = sprite;
        }

        return sprite;
    }

    private static Sprite? LoadSprite(string assetPath)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
        {
            return null;
        }

        var sprite = Resources.Load<Sprite>(assetPath);
        if (sprite != null)
        {
            return sprite;
        }

        if (!File.Exists(assetPath))
        {
            return null;
        }

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(texture, File.ReadAllBytes(assetPath)))
        {
            UnityEngine.Object.Destroy(texture);
            return null;
        }

        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    private static Proto? ResolveTargetProto(ProtoKind kind, int protoId)
    {
        return kind switch
        {
            ProtoKind.Item => LDB.items.Select(protoId),
            ProtoKind.Recipe => LDB.recipes.Select(protoId),
            ProtoKind.Tech => LDB.techs.Select(protoId),
            ProtoKind.Tutorial => LDB.tutorial.Select(protoId),
            ProtoKind.Signal => LDB.signals.Select(protoId),
            _ => null
        };
    }

    private static void SetIconSprite(Proto proto, Sprite sprite)
    {
        var field = AccessTools.Field(proto.GetType(), "_iconSprite");
        if (field != null)
        {
            field.SetValue(proto, sprite);
        }
    }
}
