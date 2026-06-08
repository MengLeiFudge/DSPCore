using HarmonyLib;

namespace DSPCore;

internal static class PatchRuntime
{
    public static void ApplyRegisteredPatches(Harmony harmony)
    {
        foreach (var descriptor in DspCore.Patches.GetAll())
        {
            try
            {
                var requiredPluginGuid = descriptor.RequiredPluginGuid;
                if (!string.IsNullOrWhiteSpace(requiredPluginGuid) &&
                    !PatchConditions.PluginLoaded(requiredPluginGuid!, descriptor.MinimumPluginVersion))
                {
                    var reason = PatchConditions.GetPluginRequirementReason(requiredPluginGuid!, descriptor.MinimumPluginVersion);
                    DspCore.Logger?.LogInfo($"DSPCore patch {descriptor.Id} disabled: {reason}");
                    continue;
                }

                if (descriptor.IsEnabled != null && !descriptor.IsEnabled())
                {
                    var reason = descriptor.GetDisabledReason?.Invoke() ?? "condition returned false";
                    DspCore.Logger?.LogInfo($"DSPCore patch {descriptor.Id} disabled: {reason}");
                    continue;
                }

                descriptor.Apply();
                DspCore.Logger?.LogInfo($"DSPCore patch {descriptor.Id} applied.");
            }
            catch (System.Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
                DspCore.Logger?.LogError($"DSPCore patch {descriptor.Id} failed: {ex}");
            }
        }
    }
}
