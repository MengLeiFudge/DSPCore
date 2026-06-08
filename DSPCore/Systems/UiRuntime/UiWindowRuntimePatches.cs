using HarmonyLib;

namespace DSPCore;

/// <summary>
/// DSPCore UI 窗口生命周期的运行时补丁。
/// Runtime patches for DSPCore UI window lifecycle.
/// </summary>
internal static class UiWindowRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIRoot), "_OnDestroy")]
    private static void UIRoot_OnDestroy_Postfix()
    {
        UiWindowManager.FreeAllWindows();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIRoot), "_OnOpen")]
    private static void UIRoot_OnOpen_Postfix()
    {
        UiWindowManager.InitAllWindows();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIRoot), "_OnUpdate")]
    private static void UIRoot_OnUpdate_Postfix()
    {
        UiWindowManager.UpdateAllWindows();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIGame), nameof(UIGame.ShutAllFunctionWindow))]
    private static void UIGame_ShutAllFunctionWindow_Postfix()
    {
        UiWindowManager.CloseFunctionalWindows();
    }
}
