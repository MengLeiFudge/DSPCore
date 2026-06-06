namespace DSPCore.Preloader;

using System.Collections.Generic;
using BepInEx.Logging;
using Mono.Cecil;

/// <summary>
/// DSPCore 的 BepInEx preloader 入口。
/// BepInEx preloader entry point for DSPCore.
/// </summary>
public static class Preloader
{
    private static ManualLogSource? logger;

    /// <summary>
    /// 获取需要在加载前处理的程序集。
    /// Gets assemblies that should be processed before loading.
    /// </summary>
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    /// <summary>
    /// 初始化 preloader 日志。
    /// Initializes the preloader logger.
    /// </summary>
    public static void Initialize()
    {
        logger = Logger.CreateLogSource("DSPCore Preloader");
    }

    /// <summary>
    /// 在游戏程序集加载前应用 DSPCore patch。
    /// Applies DSPCore patches before the game assembly is loaded.
    /// </summary>
    /// <param name="assembly">目标程序集。Target assembly.</param>
    public static void Patch(AssemblyDefinition assembly)
    {
        logger?.LogInfo($"DSPCore preloader inspected {assembly.Name.Name}.");
    }
}
