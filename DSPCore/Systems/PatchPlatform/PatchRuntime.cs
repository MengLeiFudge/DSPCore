using HarmonyLib;

namespace DSPCore;

internal static class PatchRuntime
{
    private static readonly System.Collections.Generic.HashSet<string> AppliedPatchIds = new(System.StringComparer.Ordinal);
    private static Harmony? activeHarmony;

    public static void ApplyRegisteredPatches(Harmony harmony)
    {
        activeHarmony = harmony;
        foreach (var descriptor in DspCore.Patches.GetAll())
        {
            Apply(descriptor);
        }
    }

    public static void ApplyIfReady(PatchDescriptor descriptor)
    {
        if (activeHarmony == null)
        {
            return;
        }

        Apply(descriptor);
    }

    private static void Apply(PatchDescriptor descriptor)
    {
        if (AppliedPatchIds.Contains(descriptor.Id))
        {
            return;
        }

        try
        {
            var requiredPluginGuid = descriptor.RequiredPluginGuid;
            if (!string.IsNullOrWhiteSpace(requiredPluginGuid) &&
                !PatchConditions.PluginLoaded(requiredPluginGuid!, descriptor.MinimumPluginVersion))
            {
                var reason = PatchConditions.GetPluginRequirementReason(requiredPluginGuid!, descriptor.MinimumPluginVersion);
                DspCore.Logger?.LogInfo($"DSPCore patch {descriptor.Id} disabled: {reason}");
                return;
            }

            if (descriptor.IsEnabled != null && !descriptor.IsEnabled())
            {
                var reason = descriptor.GetDisabledReason?.Invoke() ?? "condition returned false";
                DspCore.Logger?.LogInfo($"DSPCore patch {descriptor.Id} disabled: {reason}");
                return;
            }

            descriptor.Apply();
            AppliedPatchIds.Add(descriptor.Id);
            DspCore.Logger?.LogInfo($"DSPCore patch {descriptor.Id} applied.");
        }
        catch (System.Exception ex)
        {
            AppliedPatchIds.Add(descriptor.Id);
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"DSPCore patch {descriptor.Id} failed: {ex}");
        }
    }
}
