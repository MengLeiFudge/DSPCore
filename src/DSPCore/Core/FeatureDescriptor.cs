using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 描述一个 DSPCore 功能块。
/// Describes a DSPCore feature block.
/// </summary>
/// <param name="Id">功能块 ID。Feature block id.</param>
/// <param name="DisplayName">显示名称。Display name.</param>
/// <param name="Priority">初始化优先级，数值越小越早。Initialization priority; lower values run earlier.</param>
/// <param name="Initialize">初始化回调。Initialization callback.</param>
/// <param name="Dependencies">依赖的功能块 ID。Dependent feature block ids.</param>
public sealed record FeatureDescriptor(
    string Id,
    string DisplayName,
    int Priority,
    Action Initialize,
    IReadOnlyList<string>? Dependencies = null);
