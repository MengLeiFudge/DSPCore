using System;
using System.Collections.Generic;

namespace CommonAPI;

/// <summary>
/// 旧 CommonAPI 插件入口；请迁移到 DSPCore.DspCore。
/// Legacy CommonAPI plugin entry point; migrate to DSPCore.DspCore.
/// </summary>
[Obsolete("Use DSPCore.DspCore instead.")]
public static class CommonAPIPlugin
{
    /// <summary>
    /// 旧 CommonAPI GUID。
    /// Legacy CommonAPI GUID.
    /// </summary>
    public const string GUID = "dsp.common-api.CommonAPI";

    /// <summary>
    /// 判断子模块是否已加载；初版通过 DSPCore 模块注册表查询。
    /// Checks whether a submodule is loaded; first version queries the DSPCore module registry.
    /// </summary>
    /// <param name="submodule">子模块名称。Submodule name.</param>
    /// <returns>存在同名模块时返回 true。Returns true when a module with the same name exists.</returns>
    [Obsolete("Use DSPCore.DspCore.Modules.TryGet instead.")]
    public static bool IsSubmoduleLoaded(string submodule)
    {
        return DSPCore.DspCore.Modules.TryGet(submodule, out _);
    }
}

/// <summary>
/// 旧 CommonAPI 子模块依赖属性；请迁移到 DSPCore.ModuleRegistry 注册。
/// Legacy CommonAPI submodule dependency attribute; migrate to DSPCore.ModuleRegistry registration.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true)]
[Obsolete("Use DSPCore.ModuleRegistry and ModuleDescriptor instead.")]
public sealed class CommonAPISubmoduleDependencyAttribute : Attribute
{
    /// <summary>
    /// 创建旧子模块依赖属性。
    /// Creates a legacy submodule dependency attribute.
    /// </summary>
    /// <param name="submodules">子模块名称。Submodule names.</param>
    public CommonAPISubmoduleDependencyAttribute(params string[] submodules)
    {
        Submodules = submodules;
    }

    /// <summary>
    /// 请求的旧子模块名称。
    /// Requested legacy submodule names.
    /// </summary>
    public IReadOnlyList<string> Submodules { get; }
}
