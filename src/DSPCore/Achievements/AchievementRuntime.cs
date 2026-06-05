using HarmonyLib;
using ABN;

namespace DSPCore;

internal static class AchievementRuntime
{
    public static bool AllowAchievementMutation()
    {
        var blocked = DspCore.Achievements.ShouldBlockAchievementAccess();
        if (blocked)
        {
            DspCore.Logger?.LogDebug("Achievement mutation was blocked by DSPCore aggregated achievement policy.");
        }

        return !blocked;
    }

    public static bool AllowAbnormalityCheck()
    {
        return !DspCore.Achievements.ShouldBlockAbnormalityChecks();
    }

    public static bool AllowLeaderboardUpload()
    {
        return !DspCore.Achievements.ShouldBlockLeaderboardUpload();
    }

    public static bool AllowPlatformMetadata()
    {
        return !DspCore.Achievements.ShouldBlockPlatformMetadata();
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
    [HarmonyPatch(typeof(GameAbnormalityData_0925), nameof(GameAbnormalityData_0925.NotifyOnAbnormalityChecked))]
    [HarmonyPatch(typeof(GameAbnormalityData_0925), nameof(GameAbnormalityData_0925.TriggerAbnormality))]
    [HarmonyPatch(typeof(AbnormalityLogic), nameof(AbnormalityLogic.GameTick))]
    private static bool AbnormalityCheck()
    {
        return AchievementRuntime.AllowAbnormalityCheck();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AbnormalityRuntimeData), nameof(AbnormalityRuntimeData.Import))]
    private static void AbnormalityRuntimeDataImport(ref AbnormalityRuntimeData __instance)
    {
        AchievementRuntime.ClearAbnormalityRuntimeData(ref __instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameAbnormalityData_0925), nameof(GameAbnormalityData_0925.IsAbnormalTriggerred))]
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
    [HarmonyPatch(typeof(MilkyWayWebClient), nameof(MilkyWayWebClient.SendUploadLoginRequest))]
    [HarmonyPatch(typeof(MilkyWayWebClient), nameof(MilkyWayWebClient.SendUploadRecordRequest))]
    [HarmonyPatch(typeof(STEAMX), nameof(STEAMX.UploadScoreToLeaderboard))]
    private static bool MilkyWayUpload()
    {
        return AchievementRuntime.AllowLeaderboardUpload();
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
        return AchievementRuntime.AllowPlatformMetadata();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), nameof(AchievementSystem.SyncAchievementsToPlatform))]
    private static bool SyncAchievementsToPlatform()
    {
        return AchievementRuntime.AllowPlatformMetadata();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), "UnlockPlatformAchievement")]
    private static bool UnlockPlatformAchievement()
    {
        return AchievementRuntime.AllowPlatformMetadata();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AchievementSystem), "SetPlatformAchievementProgress")]
    private static bool SetPlatformAchievementProgress()
    {
        return AchievementRuntime.AllowPlatformMetadata();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(STEAMX), nameof(STEAMX.UnlockAchievement))]
    [HarmonyPatch(typeof(STEAMX), nameof(STEAMX.SetAchievementProgress))]
    [HarmonyPatch(typeof(SteamAchievementManager), nameof(SteamAchievementManager.UnlockAchievement))]
    [HarmonyPatch(typeof(SteamAchievementManager), nameof(SteamAchievementManager.SetAchievementProgress))]
    [HarmonyPatch(typeof(SteamAchievementManager), "Update")]
    [HarmonyPatch(typeof(SteamAchievementManager), "Start")]
    [HarmonyPatch(typeof(RAILX), nameof(RAILX.UnlockAchievement))]
    [HarmonyPatch(typeof(RAILX), nameof(RAILX.SetAchievementProgress))]
    [HarmonyPatch(typeof(RailAchievementManager), nameof(RailAchievementManager.UnlockAchievement))]
    [HarmonyPatch(typeof(RailAchievementManager), nameof(RailAchievementManager.SetAchievementProgress))]
    [HarmonyPatch(typeof(RailAchievementManager), "Update")]
    [HarmonyPatch(typeof(RailAchievementManager), "Start")]
    [HarmonyPatch(typeof(XGPX), nameof(XGPX.AddTask))]
    [HarmonyPatch(typeof(XGPX), nameof(XGPX.BeginStoreStats))]
    [HarmonyPatch(typeof(XGPX), nameof(XGPX.UnlockAchievement))]
    [HarmonyPatch(typeof(XGPX), nameof(XGPX.SetAchievementProgress))]
    private static bool PlatformAchievement()
    {
        return AchievementRuntime.AllowPlatformMetadata();
    }
}
