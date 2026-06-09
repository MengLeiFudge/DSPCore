using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 收集作者声明诊断项并运行内置声明检查。
/// Collects author declaration diagnostics and runs built-in declaration checks.
/// </summary>
public sealed class DiagnosticRegistry
{
    private readonly List<DiagnosticIssue> manualIssues = new();
    private IReadOnlyList<DiagnosticIssue> builtInIssues = Array.Empty<DiagnosticIssue>();
    private List<DiagnosticIssue>? currentBuiltInIssues;

    /// <summary>
    /// 记录一个诊断项。
    /// Records a diagnostic issue.
    /// </summary>
    /// <param name="issue">诊断项。Diagnostic issue.</param>
    public void Report(DiagnosticIssue issue)
    {
        if (currentBuiltInIssues != null)
        {
            currentBuiltInIssues.Add(issue);
            return;
        }

        manualIssues.Add(issue);
    }

    /// <summary>
    /// 记录一个信息诊断项。
    /// Records an informational diagnostic issue.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="code">稳定诊断代码。Stable diagnostic code.</param>
    /// <param name="message">诊断消息。Diagnostic message.</param>
    /// <param name="subject">相关声明对象。Related declaration subject.</param>
    public void Info(string ownerModGuid, string code, string message, string? subject = null)
    {
        Report(new DiagnosticIssue(DiagnosticSeverity.Info, ownerModGuid, code, message, subject));
    }

    /// <summary>
    /// 记录一个警告诊断项。
    /// Records a warning diagnostic issue.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="code">稳定诊断代码。Stable diagnostic code.</param>
    /// <param name="message">诊断消息。Diagnostic message.</param>
    /// <param name="subject">相关声明对象。Related declaration subject.</param>
    public void Warn(string ownerModGuid, string code, string message, string? subject = null)
    {
        Report(new DiagnosticIssue(DiagnosticSeverity.Warning, ownerModGuid, code, message, subject));
    }

    /// <summary>
    /// 记录一个错误诊断项。
    /// Records an error diagnostic issue.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="code">稳定诊断代码。Stable diagnostic code.</param>
    /// <param name="message">诊断消息。Diagnostic message.</param>
    /// <param name="subject">相关声明对象。Related declaration subject.</param>
    public void Error(string ownerModGuid, string code, string message, string? subject = null)
    {
        Report(new DiagnosticIssue(DiagnosticSeverity.Error, ownerModGuid, code, message, subject));
    }

    /// <summary>
    /// 运行内置作者声明检查。
    /// Runs built-in author declaration checks.
    /// </summary>
    /// <returns>诊断项快照。Diagnostic issue snapshot.</returns>
    public IReadOnlyList<DiagnosticIssue> RunBuiltInChecks()
    {
        var nextBuiltInIssues = new List<DiagnosticIssue>();
        currentBuiltInIssues = nextBuiltInIssues;
        try
        {
            CheckProtoIds();
            CheckGridIndexes();
            CheckTabs();
            CheckOptions();
            CheckLocalizations();
        }
        finally
        {
            currentBuiltInIssues = null;
        }

        builtInIssues = nextBuiltInIssues.ToArray();
        return GetIssues();
    }

    /// <summary>
    /// 获取已记录的诊断项。
    /// Gets recorded diagnostic issues.
    /// </summary>
    /// <returns>诊断项快照。Diagnostic issue snapshot.</returns>
    public IReadOnlyList<DiagnosticIssue> GetIssues()
    {
        return manualIssues.Concat(builtInIssues).ToArray();
    }

    private void CheckProtoIds()
    {
        var registrations = DspCore.ProtoRegistration.GetAll();
        foreach (var entry in registrations)
        {
            var id = ReadInt(entry.Proto, "ID") ?? 0;
            if (id <= 0)
            {
                Error(entry.OwnerModGuid, "proto.id.missing", "Registered proto has no positive ID.", DescribeProto(entry));
            }
        }

        foreach (var group in registrations
            .Select(item => new { Entry = item, Id = ReadInt(item.Proto, "ID") ?? 0 })
            .Where(item => item.Id > 0)
            .GroupBy(item => item.Entry.Kind.ToString() + ":" + item.Id, StringComparer.Ordinal))
        {
            var items = group.ToArray();
            if (items.Length <= 1)
            {
                continue;
            }

            var first = items[0].Entry;
            Error(
                first.OwnerModGuid,
                "proto.id.duplicate",
                "Multiple registered protos use the same kind and ID: " + group.Key + ".",
                string.Join(", ", items.Select(item => DescribeProto(item.Entry)).ToArray()));
        }
    }

    private void CheckGridIndexes()
    {
        var entries = DspCore.ProtoRegistration.GetAll()
            .Select(item => new { Entry = item, GridIndex = ReadInt(item.Proto, "GridIndex") ?? 0 })
            .Where(item => (item.Entry.Kind == ProtoKind.Item || item.Entry.Kind == ProtoKind.Recipe) && item.GridIndex > 0)
            .ToArray();

        foreach (var group in entries.GroupBy(item => item.Entry.Kind.ToString() + ":" + item.GridIndex.ToString(), StringComparer.Ordinal))
        {
            var items = group.ToArray();
            if (items.Length <= 1)
            {
                continue;
            }

            var first = items[0].Entry;
            Warn(
                first.OwnerModGuid,
                "proto.gridIndex.duplicate",
                "Multiple registered protos use the same GridIndex: " + group.Key + ". Runtime may apply fallback placement.",
                string.Join(", ", items.Select(item => DescribeProto(item.Entry)).ToArray()));
        }

        var customTabs = new HashSet<int>(DspCore.Tabs.GetAll().Select(tab => DspCore.Tabs.TryGetSlot(tab.Id, out var slot) ? slot.Value : 0));
        foreach (var item in entries)
        {
            var tabValue = item.GridIndex / 1000;
            if (tabValue >= TabSlot.FirstCustomValue && !customTabs.Contains(tabValue))
            {
                Warn(
                    item.Entry.OwnerModGuid,
                    "proto.gridIndex.tabMissing",
                    "Registered proto uses a custom GridIndex tab without a matching DSPCore tab declaration.",
                    DescribeProto(item.Entry) + " GridIndex=" + item.GridIndex.ToString());
            }
        }
    }

    private void CheckTabs()
    {
        foreach (var tab in DspCore.Tabs.GetAll())
        {
            if (string.IsNullOrWhiteSpace(tab.IconId))
            {
                Warn(tab.OwnerModGuid, "tab.icon.missing", "Registered tab has no icon id.", tab.Id);
                continue;
            }

            if (!DspCore.Icons.TryGet(tab.IconId, out _))
            {
                Warn(tab.OwnerModGuid, "tab.icon.unknown", "Registered tab references an icon id that is not registered.", tab.Id + " -> " + tab.IconId);
            }
        }
    }

    private void CheckOptions()
    {
        var pageIds = new HashSet<string>(DspCore.Options.GetPages().Select(page => page.PageId), StringComparer.Ordinal);
        foreach (var option in DspCore.Options.GetAll())
        {
            if (option.PageId is not string pageId || string.IsNullOrWhiteSpace(pageId))
            {
                continue;
            }

            if (!pageIds.Contains(pageId))
            {
                Warn(
                    DSPCorePlugin.PluginGuid,
                    "option.page.unknown",
                    "Registered option references a page id that is not registered.",
                    option.Section + "/" + option.Key + " -> " + pageId);
            }
        }
    }

    private void CheckLocalizations()
    {
        foreach (var group in DspCore.Resources.GetLocalizations().GroupBy(item => item.OwnerModGuid + ":" + item.Key, StringComparer.Ordinal))
        {
            var entries = group.ToArray();
            var languages = new HashSet<string>(entries.Select(item => item.Language), StringComparer.OrdinalIgnoreCase);
            bool hasChinese = languages.Contains("zhCN") || languages.Contains("zh-CN") || languages.Contains("zh");
            bool hasEnglish = languages.Contains("enUS") || languages.Contains("en-US") || languages.Contains("en");
            if (hasChinese && hasEnglish)
            {
                continue;
            }

            Warn(
                entries[0].OwnerModGuid,
                "localization.language.incomplete",
                "Localization key should provide both Chinese and English entries for author-facing examples and runtime diagnostics.",
                entries[0].Key + " languages=" + string.Join(",", languages.OrderBy(item => item, StringComparer.OrdinalIgnoreCase).ToArray()));
        }
    }

    private static int? ReadInt(object source, string name)
    {
        var type = source.GetType();
        var field = type.GetField(name);
        if (field?.GetValue(source) is int fieldValue)
        {
            return fieldValue;
        }

        var property = type.GetProperty(name);
        return property?.GetValue(source, null) is int propertyValue ? propertyValue : null;
    }

    private static string DescribeProto(ProtoRegistrationEntry entry)
    {
        var id = ReadInt(entry.Proto, "ID") ?? 0;
        return entry.Kind + ":" + id.ToString() + " owner=" + entry.OwnerModGuid + " phase=" + entry.Phase;
    }
}
