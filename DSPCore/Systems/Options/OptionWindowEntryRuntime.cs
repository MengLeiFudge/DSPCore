using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DSPCore;

internal static class OptionWindowEntryRuntime
{
    private const string ButtonName = "dspcore-options-entry-button";
    private const string KeyButtonName = "dspcore-key-options-entry-button";
    private const float ButtonWidth = 150f;
    private const float KeyButtonWidth = 220f;
    private const float ButtonGap = 12f;
    private const float KeyButtonHeight = 30f;
    private const float KeyButtonTopPadding = 8f;

    public static void EnsureButton(UIOptionWindow window)
    {
        if (window?.cancelButton == null)
        {
            return;
        }

        var parent = window.cancelButton.transform.parent as RectTransform;
        if (parent == null)
        {
            return;
        }

        var existing = parent.Find(ButtonName)?.GetComponent<UIButton>();
        var button = existing ?? UnityEngine.Object.Instantiate(window.cancelButton, parent);
        button.gameObject.name = ButtonName;
        button.data = 0;
        ClearButtonCallbacks(button);
        button.onClick += OnButtonClick;
        if (button.button != null)
        {
            button.button.onClick.RemoveAllListeners();
        }

        button.CloseTip();
        button.tips = new UIButton.TipSettings();
        button.gameObject.SetActive(true);
        button.transform.SetAsLastSibling();
        PositionButton(window.cancelButton, button);
        SetButtonText(button, OptionText.EntryButton);
        EnsureKeyBindingButton(window, button);
    }

    private static void OnButtonClick(int _)
    {
        OptionRuntime.OpenWindow();
    }

    private static void PositionButton(UIButton cancelButton, UIButton button)
    {
        var cancelRect = cancelButton.transform as RectTransform;
        var buttonRect = button.transform as RectTransform;
        if (cancelRect == null || buttonRect == null)
        {
            return;
        }

        buttonRect.anchorMin = cancelRect.anchorMin;
        buttonRect.anchorMax = cancelRect.anchorMax;
        buttonRect.pivot = cancelRect.pivot;
        buttonRect.sizeDelta = new Vector2(ButtonWidth, cancelRect.sizeDelta.y);
        buttonRect.anchoredPosition = cancelRect.anchoredPosition - new Vector2(ButtonWidth + ButtonGap, 0f);
    }

    private static void EnsureKeyBindingButton(UIOptionWindow window, UIButton sourceButton)
    {
        var content = window.keyScrollContentRect;
        if (content == null)
        {
            return;
        }

        var existing = content.Find(KeyButtonName)?.GetComponent<UIButton>();
        var button = existing ?? UnityEngine.Object.Instantiate(sourceButton, content);
        button.gameObject.name = KeyButtonName;
        button.data = 0;
        ClearButtonCallbacks(button);
        button.onClick += OnButtonClick;
        if (button.button != null)
        {
            button.button.onClick.RemoveAllListeners();
        }

        button.CloseTip();
        button.tips = new UIButton.TipSettings();
        button.gameObject.SetActive(true);
        button.transform.SetAsLastSibling();
        PositionKeyBindingButton(window, button);
        SetButtonText(button, OptionText.KeyEntryButton);
    }

    private static void PositionKeyBindingButton(UIOptionWindow window, UIButton button)
    {
        var content = window.keyScrollContentRect;
        var buttonRect = button.transform as RectTransform;
        if (content == null || buttonRect == null)
        {
            return;
        }

        var baselineHeight = 36f * DSPGame.key.builtinKeys.Length + 8f;
        var requiredHeight = baselineHeight + KeyButtonHeight + KeyButtonTopPadding;
        content.sizeDelta = new Vector2(content.sizeDelta.x, Math.Max(content.sizeDelta.y, requiredHeight));

        buttonRect.anchorMin = new Vector2(0.5f, 1f);
        buttonRect.anchorMax = new Vector2(0.5f, 1f);
        buttonRect.pivot = new Vector2(0.5f, 1f);
        buttonRect.sizeDelta = new Vector2(KeyButtonWidth, KeyButtonHeight);
        buttonRect.anchoredPosition = new Vector2(0f, -baselineHeight - KeyButtonTopPadding);
    }

    private static void SetButtonText(UIButton button, string key)
    {
        var translated = key.Translate();
        foreach (var localizer in button.GetComponentsInChildren<Localizer>(true))
        {
            localizer.stringKey = key;
            localizer.translation = translated;
        }

        foreach (var text in button.GetComponentsInChildren<Text>(true))
        {
            text.text = translated;
        }
    }

    private static void ClearButtonCallbacks(UIButton button)
    {
        ClearEvent(button, "onMouseDown");
        ClearEvent(button, "onClick");
        ClearEvent(button, "onDoubleClick");
        ClearEvent(button, "onRightClick");
        ClearEvent(button, "onClickEnable");
    }

    private static void ClearEvent(UIButton button, string fieldName)
    {
        try
        {
            AccessTools.Field(typeof(UIButton), fieldName)?.SetValue(button, null);
        }
        catch (Exception ex)
        {
            DspCore.Logger?.LogWarning($"Failed to clear UIButton.{fieldName} for DSPCore options entry: {ex.Message}");
        }
    }
}

internal static class OptionWindowEntryRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIOptionWindow), "_OnCreate")]
    private static void OnCreate(UIOptionWindow __instance)
    {
        OptionWindowEntryRuntime.EnsureButton(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
    private static void OnOpen(UIOptionWindow __instance)
    {
        OptionWindowEntryRuntime.EnsureButton(__instance);
    }
}
