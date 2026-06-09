using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace DSPCore;

internal static class IconRuntime
{
    private static readonly Dictionary<string, Sprite> SpriteCache = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, AssetBundle> AssetBundleCache = new(StringComparer.Ordinal);

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

        if (IconAssetPaths.TryParseEmbedded(assetPath, out var assemblyName, out var resourceName))
        {
            return LoadEmbeddedSprite(assemblyName, resourceName);
        }

        if (IconAssetPaths.TryParseAssetBundle(assetPath, out var bundlePath, out var assetName))
        {
            return LoadAssetBundleSprite(bundlePath, assetName);
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

        return LoadPngSprite(File.ReadAllBytes(assetPath));
    }

    private static Sprite? LoadEmbeddedSprite(string assemblyName, string resourceName)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(item => string.Equals(item.GetName().Name, assemblyName, StringComparison.Ordinal));
        if (assembly == null)
        {
            DspCore.Logger?.LogWarning($"Embedded icon assembly {assemblyName} is not loaded.");
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            DspCore.Logger?.LogWarning($"Embedded icon resource {resourceName} was not found in {assemblyName}.");
            return null;
        }

        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return LoadPngSprite(buffer.ToArray());
    }

    private static Sprite? LoadAssetBundleSprite(string bundlePath, string assetName)
    {
        if (!AssetBundleCache.TryGetValue(bundlePath, out var bundle) || bundle == null)
        {
            if (!File.Exists(bundlePath))
            {
                DspCore.Logger?.LogWarning($"AssetBundle icon bundle {bundlePath} does not exist.");
                return null;
            }

            bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                DspCore.Logger?.LogWarning($"AssetBundle icon bundle {bundlePath} could not be loaded.");
                return null;
            }

            AssetBundleCache[bundlePath] = bundle;
        }

        var sprite = bundle.LoadAsset<Sprite>(assetName);
        if (sprite != null)
        {
            return sprite;
        }

        var texture = bundle.LoadAsset<Texture2D>(assetName);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        DspCore.Logger?.LogWarning($"AssetBundle icon asset {assetName} was not found in {bundlePath}.");
        return null;
    }

    private static Sprite? LoadPngSprite(byte[] data)
    {
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(texture, data))
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
