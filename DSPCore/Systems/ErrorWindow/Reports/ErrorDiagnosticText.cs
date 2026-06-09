#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace DSPCore;

internal static class ErrorDiagnosticText
{
    public static string Build(string focalText, int maxReports, int maxPatchedMethods)
    {
        maxReports = Math.Max(1, maxReports);
        maxPatchedMethods = Math.Max(0, maxPatchedMethods);

        var reports = DspCore.Errors.GetReports()
            .Reverse()
            .Take(maxReports)
            .Reverse()
            .ToArray();

        var builder = new StringBuilder(8192);
        builder.AppendLine("DSPCore Diagnostic Snapshot");
        builder.AppendLine("GeneratedUtc: " + DateTime.UtcNow.ToString("O"));
        builder.AppendLine("DSPCoreVersion: " + DspCore.Version);
        builder.AppendLine("LoadedPlugins: " + Chainloader.PluginInfos.Count.ToString());
        builder.AppendLine("Reports: " + DspCore.Errors.GetReports().Count.ToString());
        builder.AppendLine();

        AppendFocalText(builder, focalText);
        AppendCandidatePlugins(builder, focalText, reports);
        AppendReports(builder, reports);
        AppendDeclarations(builder);
        AppendHarmonyPatchMap(builder, maxPatchedMethods);
        return builder.ToString();
    }

    private static void AppendFocalText(StringBuilder builder, string focalText)
    {
        if (string.IsNullOrWhiteSpace(focalText))
        {
            return;
        }

        builder.AppendLine("== Focal Error Text ==");
        builder.AppendLine(focalText.Trim());
        builder.AppendLine();
    }

    private static void AppendCandidatePlugins(StringBuilder builder, string focalText, IReadOnlyList<ErrorReport> reports)
    {
        var haystack = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(focalText))
        {
            haystack.AppendLine(focalText);
        }

        foreach (var report in reports)
        {
            haystack.AppendLine(report.OwnerModGuid);
            haystack.AppendLine(report.ErrorType);
            haystack.AppendLine(report.Message);
            haystack.AppendLine(report.StackTrace);
        }

        string text = haystack.ToString();
        var candidates = Chainloader.PluginInfos.Values
            .Where(plugin => ContainsToken(text, plugin.Metadata.GUID) || ContainsToken(text, plugin.Metadata.Name))
            .OrderBy(plugin => plugin.Metadata.GUID, StringComparer.Ordinal)
            .ToArray();

        builder.AppendLine("== Candidate Plugins By Text Hit ==");
        if (candidates.Length == 0)
        {
            builder.AppendLine("- none");
            builder.AppendLine();
            return;
        }

        foreach (var plugin in candidates)
        {
            builder.Append("- ");
            builder.Append(plugin.Metadata.GUID);
            builder.Append(" | ");
            builder.Append(plugin.Metadata.Name);
            builder.Append(" | ");
            builder.AppendLine(plugin.Metadata.Version.ToString());
        }

        builder.AppendLine();
    }

    private static bool ContainsToken(string haystack, string token)
    {
        return !string.IsNullOrWhiteSpace(token)
            && haystack.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static void AppendReports(StringBuilder builder, IReadOnlyList<ErrorReport> reports)
    {
        builder.AppendLine("== Recent Error Reports ==");
        if (reports.Count == 0)
        {
            builder.AppendLine("- none");
            builder.AppendLine();
            return;
        }

        foreach (var report in reports)
        {
            builder.Append("- ");
            builder.Append(report.OwnerModGuid);
            builder.Append(" | ");
            builder.Append(report.ErrorType);
            builder.Append(" | ");
            builder.AppendLine(FirstLine(report.Message));
        }

        builder.AppendLine();
    }

    private static void AppendDeclarations(StringBuilder builder)
    {
        builder.AppendLine("== DSPCore Declarations ==");
        builder.AppendLine("Features:");
        foreach (var feature in DspCore.Features.GetAll())
        {
            builder.Append("- ");
            builder.Append(feature.Id);
            builder.Append(" | priority ");
            builder.Append(feature.Priority.ToString());
            builder.Append(" | ");
            builder.AppendLine(feature.DisplayName);
        }

        builder.AppendLine("Modules:");
        foreach (var module in DspCore.Modules.GetAll().OrderBy(item => item.Id, StringComparer.Ordinal))
        {
            builder.Append("- ");
            builder.Append(module.Id);
            builder.Append(" | ");
            builder.AppendLine(module.DisplayName);
        }

        builder.AppendLine("Declared patches:");
        var patches = DspCore.Patches.GetAll();
        if (patches.Count == 0)
        {
            builder.AppendLine("- none");
        }
        else
        {
            foreach (var patch in patches.OrderBy(item => item.OwnerModGuid, StringComparer.Ordinal).ThenBy(item => item.Id, StringComparer.Ordinal))
            {
                builder.Append("- ");
                builder.Append(patch.OwnerModGuid);
                builder.Append(" | ");
                builder.Append(patch.Id);
                builder.Append(" | ");
                builder.AppendLine(patch.Description);
            }
        }

        builder.AppendLine();
    }

    private static void AppendHarmonyPatchMap(StringBuilder builder, int maxPatchedMethods)
    {
        builder.AppendLine("== Harmony Patch Map ==");
        MethodBase[] methods;
        try
        {
            methods = Harmony.GetAllPatchedMethods()
                .OrderBy(FormatMethod, StringComparer.Ordinal)
                .Take(maxPatchedMethods)
                .ToArray();
        }
        catch (Exception ex)
        {
            builder.AppendLine("- unavailable: " + ex.GetType().Name + ": " + ex.Message);
            builder.AppendLine();
            return;
        }

        if (methods.Length == 0)
        {
            builder.AppendLine("- none");
            builder.AppendLine();
            return;
        }

        foreach (var method in methods)
        {
            var owners = GetPatchOwners(method);
            builder.Append("- ");
            builder.Append(FormatMethod(method));
            builder.Append(" <- ");
            builder.AppendLine(owners.Length == 0 ? "unknown" : string.Join(", ", owners));
        }

        builder.AppendLine();
    }

    private static string[] GetPatchOwners(MethodBase method)
    {
        try
        {
            var info = Harmony.GetPatchInfo(method);
            return info?.Owners?.OrderBy(owner => owner, StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private static string FormatMethod(MethodBase method)
    {
        return (method.DeclaringType?.FullName ?? "<unknown>") + "." + method.Name;
    }

    private static string FirstLine(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        int index = value.IndexOfAny(new[] { '\r', '\n' });
        return index >= 0 ? value.Substring(0, index) : value;
    }
}
