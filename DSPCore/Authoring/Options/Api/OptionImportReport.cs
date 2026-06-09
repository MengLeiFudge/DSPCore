using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 描述一次配置导入的结果。
/// Describes the result of an option import operation.
/// </summary>
/// <param name="AppliedCount">成功写入的配置项数量。Number of option values applied.</param>
/// <param name="SkippedCount">跳过的配置项数量。Number of option values skipped.</param>
/// <param name="SkippedKeys">被跳过的配置键说明。Skipped option key descriptions.</param>
public sealed record OptionImportReport(int AppliedCount, int SkippedCount, IReadOnlyList<string> SkippedKeys);
