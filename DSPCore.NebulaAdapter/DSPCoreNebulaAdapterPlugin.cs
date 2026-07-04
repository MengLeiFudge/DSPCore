using BepInEx;
using NebulaAPI;

namespace DSPCore.NebulaAdapter;

/// <summary>
/// DSPCore 的 Nebula 适配器入口。
/// Nebula adapter entry point for DSPCore.
/// </summary>
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(DSPCorePlugin.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(NebulaModAPI.API_GUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(NebulaModAPI.NEBULA_MODID, BepInDependency.DependencyFlags.SoftDependency)]
public sealed class DSPCoreNebulaAdapterPlugin : BaseUnityPlugin
{
    /// <summary>
    /// DSPCore Nebula 适配器的 BepInEx GUID。
    /// BepInEx GUID for the DSPCore Nebula adapter.
    /// </summary>
    public const string PluginGuid = "com.menglei.dsp.core.nebula-adapter";

    /// <summary>
    /// DSPCore Nebula 适配器的 BepInEx 名称。
    /// BepInEx name for the DSPCore Nebula adapter.
    /// </summary>
    public const string PluginName = "DSPCore.NebulaAdapter";

    /// <summary>
    /// DSPCore Nebula 适配器版本。
    /// DSPCore Nebula adapter version.
    /// </summary>
    public const string PluginVersion = DspCore.Version;

    private NebulaMultiplayerTransport? transport;

    private void Awake()
    {
        NebulaModAPI.RegisterPackets(typeof(DSPCoreNebulaAdapterPlugin).Assembly);
        transport = new NebulaMultiplayerTransport(Logger);
        Multiplayer.RegisterTransport(transport);
        Logger.LogInfo("DSPCore Nebula adapter is initialized.");
    }

    private void OnDestroy()
    {
        if (transport != null)
        {
            Multiplayer.UnregisterTransport(transport);
            transport = null;
        }
    }
}
