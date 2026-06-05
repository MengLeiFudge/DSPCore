using HarmonyLib;
using ABN;

namespace DSPCore;

internal static class AchievementRuntime
{
    public static bool AllowAchievementMutation()
    {
        var disabled = DspCore.Achievements.ShouldDisableAchievements();
        if (disabled)
        {
            DspCore.Logger?.LogDebug("Achievement mutation was blocked by DSPCore aggregated achievement policy.");
        }

        return !disabled;
    }

    public static bool AllowAbnormalityCheck()
    {
        return !DspCore.Achievements.ShouldBlockAbnormalityChecks();
    }

    public static bool AllowMilkyWayUpload()
    {
        return !DspCore.Achievements.ShouldBlockMilkyWayUpload();
    }

    public static bool AllowPlatformAchievement()
    {
        return !DspCore.Achievements.ShouldBlockPlatformAchievements();
    }

    public static void ClearAbnormalityRuntimeData(ref AbnormalityRuntimeData data)
    {
        if (!DspCore.Achievements.ShouldBlockAbnormalityChecks())
        {
            return;
        }

        data.triggerTime = 0L;
        data.protoId = 0;
        data.evidences = System.Array.Empty<long>();
    }
}

internal static class AchievementRuntimePatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameAbnormalityData_0925), "NotifyOnAbnormalityChecked")]
    [HarmonyPatch(typeof(GameAbnormalityData_0925), "TriggerAbnormality")]
    [HarmonyPatch(typeof(AbnormalityLogic), "GameTick")]
    private static bool AbnormalityCheck()
    {
        return AchievementRuntime.AllowAbnormalityCheck();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AbnormalityRuntimeData), "Import")]
    private static void AbnormalityRuntimeDataImport(ref AbnormalityRuntimeData __instance)
    {
        AchievementRuntime.ClearAbnormalityRuntimeData(ref __instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameAbnormalityData_0925), "IsAbnormalTriggerred")]
    private static bool IsAbnormalTriggerred(ref bool __result)
    {
        if (AchievementRuntime.AllowAbnormalityCheck())
        {
            return true;
        }

        __result = false;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MilkyWayWebClient), "SendUploadLoginRequest")]
    [HarmonyPatch(typeof(MilkyWayWebClient), "SendUploadRecordRequest")]
    [HarmonyPatch(typeof(STEAMX), "UploadScoreToLeaderboard")]
    private static bool MilkyWayUpload()
    {
        return AchievementRuntime.AllowMilkyWayUpload();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), nameof(AchievementSystem.UnlockAchievement))]
    private static bool UnlockAchievement()
    {
        return AchievementRuntime.AllowAchievementMutation();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), nameof(AchievementSystem.SetAchievementProgress))]
    private static bool SetAchievementProgress()
    {
        return AchievementRuntime.AllowAchievementMutation();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), nameof(AchievementSystem.PullAchievementsFromPlatform))]
    private static bool PullAchievementsFromPlatform()
    {
        return AchievementRuntime.AllowPlatformAchievement();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), nameof(AchievementSystem.SyncAchievementsToPlatform))]
    private static bool SyncAchievementsToPlatform()
    {
        return AchievementRuntime.AllowPlatformAchievement();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), "UnlockPlatformAchievement")]
    private static bool UnlockPlatformAchievement()
    {
        return AchievementRuntime.AllowPlatformAchievement();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), "SetPlatformAchievementProgress")]
    private static bool SetPlatformAchievementProgress()
    {
        return AchievementRuntime.AllowPlatformAchievement();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(STEAMX), "UnlockAchievement")]
    [HarmonyPatch(typeof(STEAMX), "SetAchievementProgress")]
    [HarmonyPatch(typeof(SteamAchievementManager), "UnlockAchievement")]
    [HarmonyPatch(typeof(SteamAchievementManager), "SetAchievementProgress")]
    [HarmonyPatch(typeof(SteamAchievementManager), "Update")]
    [HarmonyPatch(typeof(SteamAchievementManager), "Start")]
    [HarmonyPatch(typeof(RAILX), "UnlockAchievement")]
    [HarmonyPatch(typeof(RAILX), "SetAchievementProgress")]
    [HarmonyPatch(typeof(RailAchievementManager), "UnlockAchievement")]
    [HarmonyPatch(typeof(RailAchievementManager), "SetAchievementProgress")]
    [HarmonyPatch(typeof(RailAchievementManager), "Update")]
    [HarmonyPatch(typeof(RailAchievementManager), "Start")]
    [HarmonyPatch(typeof(XGPX), "AddTask")]
    [HarmonyPatch(typeof(XGPX), "BeginStoreStats")]
    [HarmonyPatch(typeof(XGPX), "UnlockAchievement")]
    [HarmonyPatch(typeof(XGPX), "SetAchievementProgress")]
    private static bool PlatformAchievement()
    {
        return AchievementRuntime.AllowPlatformAchievement();
    }
}
