using BepInEx;
using HarmonyLib;

namespace DSPCore;

/// <summary>
/// DSPCore 的 BepInEx 插件入口，负责启动运行时桥接。
/// BepInEx plugin entry point for DSPCore, responsible for starting runtime bridges.
/// </summary>
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class DSPCorePlugin : BaseUnityPlugin
{
    /// <summary>
    /// DSPCore 的 BepInEx GUID。
    /// BepInEx GUID for DSPCore.
    /// </summary>
    public const string PluginGuid = "com.menglei.dsp.core";

    /// <summary>
    /// DSPCore 的 BepInEx 名称。
    /// BepInEx name for DSPCore.
    /// </summary>
    public const string PluginName = "DSPCore";

    /// <summary>
    /// DSPCore 的插件版本。
    /// Plugin version for DSPCore.
    /// </summary>
    public const string PluginVersion = DspCore.Version;

    private Harmony? harmony;

    private void Awake()
    {
        DspCore.InitializeRuntime(Logger);
        ErrorRuntime.Initialize();
        SaveRuntime.RegisterLegacyHandlers();
        harmony = new Harmony(PluginGuid);
        harmony.PatchAll(typeof(ProtoRuntimePatches));
        harmony.PatchAll(typeof(BuildBarRuntimePatches));
        harmony.PatchAll(typeof(SaveRuntimePatches));
        harmony.PatchAll(typeof(AchievementRuntimePatches));
        harmony.PatchAll(typeof(ErrorRuntimePatches));
        harmony.PatchAll(typeof(LocalizationRuntimePatches));
        harmony.PatchAll(typeof(TabRuntimePatches));
        harmony.PatchAll(typeof(RecipeTypeRuntimePatches));
        Logger.LogInfo("DSPCore runtime bridges are initialized.");
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        ErrorRuntime.Dispose();
    }

    private void Update()
    {
        KeyBindRuntime.Update();
        PickerRuntime.Update();
    }
}
