using System;
using System.Reflection;

namespace DSPCore;

internal static class IconAssetPaths
{
    private const string EmbeddedPrefix = "embedded://";
    private const string AssetBundlePrefix = "assetbundle://";
    private const string AssetBundleSeparator = "|";

    public static string Embedded(Assembly assembly, string resourceName)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        if (string.IsNullOrWhiteSpace(resourceName))
        {
            throw new ArgumentException("Embedded icon resource name cannot be empty.", nameof(resourceName));
        }

        return EmbeddedPrefix + assembly.GetName().Name + "/" + resourceName;
    }

    public static bool TryParseEmbedded(string assetPath, out string assemblyName, out string resourceName)
    {
        assemblyName = string.Empty;
        resourceName = string.Empty;
        if (!assetPath.StartsWith(EmbeddedPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        var body = assetPath.Substring(EmbeddedPrefix.Length);
        var separator = body.IndexOf('/');
        if (separator <= 0 || separator == body.Length - 1)
        {
            return false;
        }

        assemblyName = body.Substring(0, separator);
        resourceName = body.Substring(separator + 1);
        return true;
    }

    public static string AssetBundle(string bundlePath, string assetName)
    {
        if (string.IsNullOrWhiteSpace(bundlePath))
        {
            throw new ArgumentException("AssetBundle path cannot be empty.", nameof(bundlePath));
        }

        if (string.IsNullOrWhiteSpace(assetName))
        {
            throw new ArgumentException("AssetBundle asset name cannot be empty.", nameof(assetName));
        }

        return AssetBundlePrefix + bundlePath + AssetBundleSeparator + assetName;
    }

    public static bool TryParseAssetBundle(string assetPath, out string bundlePath, out string assetName)
    {
        bundlePath = string.Empty;
        assetName = string.Empty;
        if (!assetPath.StartsWith(AssetBundlePrefix, StringComparison.Ordinal))
        {
            return false;
        }

        var body = assetPath.Substring(AssetBundlePrefix.Length);
        var separator = body.LastIndexOf(AssetBundleSeparator, StringComparison.Ordinal);
        if (separator <= 0 || separator == body.Length - 1)
        {
            return false;
        }

        bundlePath = body.Substring(0, separator);
        assetName = body.Substring(separator + AssetBundleSeparator.Length);
        return true;
    }
}
