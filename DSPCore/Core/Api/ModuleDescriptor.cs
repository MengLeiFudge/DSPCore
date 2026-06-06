using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 描述一个 DSPCore 模块。
/// Describes a DSPCore module.
/// </summary>
/// <param name="Id">模块 ID。Module id.</param>
/// <param name="DisplayName">显示名称。Display name.</param>
/// <param name="Initialize">初始化回调。Initialization callback.</param>
/// <param name="Dependencies">模块依赖 ID。Module dependency ids.</param>
public sealed record ModuleDescriptor(
    string Id,
    string DisplayName,
    Action Initialize,
    IReadOnlyList<string>? Dependencies = null);
