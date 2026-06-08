using System.Linq;
using BepInEx.Bootstrap;

namespace DSPCore;

internal static class MultiplayerRuntime
{
    public static bool IsNebulaAvailable { get; private set; }

    public static void Initialize()
    {
        IsNebulaAvailable = Chainloader.PluginInfos.Keys.Any(key => key.Contains("nebula") || key.Contains("Nebula"));
    }

    public static void ApplyClientIntoOtherSave()
    {
        foreach (var descriptor in DspCore.Multiplayer.GetClientSaveInitializers())
        {
            try
            {
                descriptor.IntoOtherSave();
            }
            catch (System.Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            }
        }
    }
}
