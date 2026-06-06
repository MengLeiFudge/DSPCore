using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DSPCore;

internal static class ErrorRuntime
{
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized)
        {
            return;
        }

        Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
        initialized = true;
    }

    public static void Dispose()
    {
        if (!initialized)
        {
            return;
        }

        Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        initialized = false;
    }

    public static void RecordFatalWindowError(string errorString, string stackTrace)
    {
        DspCore.Errors.Report(new ErrorReport("DSPCore.FatalWindow", "FatalError", errorString, stackTrace));
    }

    public static void EnsureWindowButtons(UIFatalErrorTip tip)
    {
        if (tip == null || tip.resetButton == null || tip.resetButton.transform == null)
        {
            return;
        }

        EnsureButton(tip, "DSPCoreCopyButton", "Copy", -90f, _ => GUIUtility.systemCopyBuffer = tip.errorLogText?.text ?? string.Empty);
        EnsureButton(tip, "DSPCoreCloseButton", "Close", -180f, _ => tip._Close());
    }

    private static void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert)
        {
            return;
        }

        DspCore.Errors.Report(new ErrorReport("UnityLog", type.ToString(), condition, stackTrace));
    }

    private static void EnsureButton(UIFatalErrorTip tip, string name, string text, float xOffset, Action<int> onClick)
    {
        var parent = tip.resetButton.transform.parent;
        if (parent == null || parent.Find(name) != null)
        {
            return;
        }

        var buttonObject = UnityEngine.Object.Instantiate(tip.resetButton.gameObject, parent, false);
        buttonObject.name = name;
        var rectTransform = buttonObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += new Vector2(xOffset, 0f);
        }

        var uiButton = buttonObject.GetComponent<UIButton>();
        if (uiButton != null)
        {
            uiButton.onClick += onClick.Invoke;
        }

        var label = buttonObject.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.text = text;
        }
    }
}

internal static class ErrorRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIFatalErrorTip), nameof(UIFatalErrorTip.ShowError))]
    private static void ShowError(string errorString, string stackTrace, UIFatalErrorTip __instance)
    {
        ErrorRuntime.RecordFatalWindowError(errorString, stackTrace);
        ErrorRuntime.EnsureWindowButtons(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIFatalErrorTip), nameof(UIFatalErrorTip.ShowAssertionFailed))]
    private static void ShowAssertionFailed(string errorString, string stackTrace, UIFatalErrorTip __instance)
    {
        ErrorRuntime.RecordFatalWindowError("Assertion Failed: " + errorString, stackTrace);
        ErrorRuntime.EnsureWindowButtons(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIFatalErrorTip), "_OnOpen")]
    private static void OnOpen(UIFatalErrorTip __instance)
    {
        ErrorRuntime.EnsureWindowButtons(__instance);
    }
}
