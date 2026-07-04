using System;

namespace DSPCore;

internal static class ResourcePaths
{
    public static string Normalize(string path)
    {
        string normalized = (path ?? string.Empty).Trim().Replace('\\', '/');
        while (normalized.Length > 1 && normalized.EndsWith("/", StringComparison.Ordinal))
        {
            normalized = normalized.Substring(0, normalized.Length - 1);
        }

        return normalized;
    }

    public static bool IsRooted(string path)
    {
        return path.StartsWith("/", StringComparison.Ordinal)
            || (path.Length >= 2 && path[1] == ':');
    }

    public static string Combine(string rootPath, string relativePath)
    {
        string root = Normalize(rootPath);
        string relative = Normalize(relativePath);
        if (string.IsNullOrEmpty(root) || string.IsNullOrEmpty(relative) || IsRooted(relative))
        {
            return relative;
        }

        if (root == "/")
        {
            return "/" + relative;
        }

        return root + "/" + relative;
    }
}
