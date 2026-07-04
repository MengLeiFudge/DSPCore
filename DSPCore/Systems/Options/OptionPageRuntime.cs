#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DSPCore;

internal static class OptionPageRuntime
{
    private const string PageObjectName = "dspcore-option-page";
    private const string TabObjectName = "dspcore-option-tab";
    private const float TabWidth = 160f;
    private const float PageWidth = 900f;
    private const float PageHeight = 520f;
    private const float HeaderHeight = 54f;
    private const float RowHeight = 42f;
    private const float GroupHeight = 30f;
    private const float LabelWidth = 210f;
    private const float ControlWidth = 220f;
    private const float ResetWidth = 76f;

    public static int TabIndex { get; private set; } = -1;

    public static void EnsurePage(UIOptionWindow window)
    {
        if (!window)
        {
            return;
        }

        EnsureTab(window);
        var root = EnsurePageRoot(window);
        RebuildPage(root);
        ApplyVisibility(window);
    }

    public static void SetTabIndex(UIOptionWindow window)
    {
        EnsurePage(window);
        SetPageActive(window, true);
    }

    public static void ApplyVisibility(UIOptionWindow window)
    {
        SetPageActive(window, false);
    }

    public static void SaveOptions()
    {
        foreach (var option in DspCore.Options.GetAll())
        {
            if (!UiOptionPageHost.IsRenderableOption(option) || !PendingValues.TryGetValue(OptionRegistry.KeyOf(option.Section, option.Key), out var value))
            {
                continue;
            }

            OptionRuntime.SetString(option.Section, option.Key, value);
        }

        PendingValues.Clear();
    }

    public static void DiscardOptions()
    {
        PendingValues.Clear();
    }

    internal static string GetPendingValue(OptionDescriptor option)
    {
        return PendingValues.TryGetValue(OptionRegistry.KeyOf(option.Section, option.Key), out var value)
            ? value
            : DspCore.Options.GetString(option.Section, option.Key);
    }

    internal static void SetPendingValue(OptionDescriptor option, string value)
    {
        PendingValues[OptionRegistry.KeyOf(option.Section, option.Key)] = value;
    }

    private static readonly Dictionary<string, string> PendingValues = new(StringComparer.Ordinal);

    private static void EnsureTab(UIOptionWindow window)
    {
        if (window.transform.Find(TabObjectName) != null)
        {
            return;
        }

        var oldButtons = window.tabButtons ?? Array.Empty<UIButton>();
        var oldTexts = window.tabTexts ?? Array.Empty<Text>();
        if (oldButtons.Length == 0)
        {
            return;
        }

        TabIndex = oldButtons.Length;
        var sourceButton = oldButtons[oldButtons.Length - 1];
        var parent = sourceButton.transform.parent;
        var tab = UnityEngine.Object.Instantiate(sourceButton, parent);
        tab.gameObject.name = TabObjectName;
        tab.data = TabIndex;
        ClearButtonCallbacks(tab);
        tab.onClick += _ => SetPageActive(window, true);
        if (tab.button != null)
        {
            tab.button.onClick.RemoveAllListeners();
            tab.button.onClick.AddListener((UnityAction)(() => SetPageActive(window, true)));
        }

        if (tab.transform is RectTransform tabRect && sourceButton.transform is RectTransform sourceRect)
        {
            tabRect.anchorMin = sourceRect.anchorMin;
            tabRect.anchorMax = sourceRect.anchorMax;
            tabRect.pivot = sourceRect.pivot;
            tabRect.sizeDelta = sourceRect.sizeDelta;
            tabRect.anchoredPosition = sourceRect.anchoredPosition + new Vector2(TabWidth, 0f);
        }

        var text = tab.GetComponentInChildren<Text>(true);
        if (text == null && oldTexts.Length > 0)
        {
            text = UnityEngine.Object.Instantiate(oldTexts[oldTexts.Length - 1], tab.transform);
        }

        if (text != null)
        {
            SetTabText(tab, text);
        }
    }

    private static RectTransform EnsurePageRoot(UIOptionWindow window)
    {
        var existing = window.transform.Find(PageObjectName) as RectTransform;
        if (existing != null)
        {
            return existing;
        }

        var obj = new GameObject(PageObjectName, typeof(RectTransform), typeof(UiOptionPageHost));
        var rect = obj.GetComponent<RectTransform>();
        rect.SetParent(window.transform, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(PageWidth, PageHeight);
        rect.anchoredPosition = Vector2.zero;
        return rect;
    }

    private static void RebuildPage(RectTransform root)
    {
        var host = root.GetComponent<UiOptionPageHost>();
        host.Rebuild(root);
    }

    private static void SetTabText(UIButton button, Text text)
    {
        foreach (var localizer in button.GetComponentsInChildren<Localizer>(true))
        {
            localizer.stringKey = OptionText.Title;
            localizer.translation = OptionText.Title.Translate();
        }

        foreach (var childText in button.GetComponentsInChildren<Text>(true))
        {
            childText.text = OptionText.Title.Translate();
        }

        text.text = OptionText.Title.Translate();
    }

    private static void SetPageActive(UIOptionWindow window, bool active)
    {
        if (!window)
        {
            return;
        }

        var page = window.transform.Find(PageObjectName);
        if (page)
        {
            page.gameObject.SetActive(active);
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
            DspCore.Logger?.LogWarning($"Failed to clear UIButton.{fieldName} for DSPCore option tab: {ex.Message}");
        }
    }

    internal sealed class UiOptionPageHost : MonoBehaviour
    {
        private bool built;

        public void Rebuild(RectTransform root)
        {
            if (built)
            {
                return;
            }

            built = true;
            UiRoundedSpriteFactory.GetFillSprite();
            UiRoundedSpriteFactory.GetBorderSprite();
            BuildRoot(root);
        }

        public static bool IsRenderableOption(OptionDescriptor option)
        {
            return option.Kind != OptionValueKind.KeyBinding;
        }

        private static void BuildRoot(RectTransform root)
        {
            var rows = BuildRows();
            float contentHeight = Math.Max(PageHeight - HeaderHeight - 24f, rows.Sum(row => row.Height) + Math.Max(0, rows.Count - 1) * 6f + 18f);
            var header = UiPageLayout.CreatePageHeader(null, root, OptionText.Title, OptionText.Summary, "dspcore-option-page-header", PageWidth, HeaderHeight);
            var content = UiPageLayout.CreateScrollableContentCard(
                root,
                "dspcore-option-page-content",
                0f,
                HeaderHeight + 14f,
                PageWidth,
                PageHeight - HeaderHeight - 14f,
                contentHeight);

            float y = 12f;
            foreach (var row in rows)
            {
                row.Build(content, y, PageWidth - 40f);
                y += row.Height + 6f;
            }
        }

        private static List<PageRow> BuildRows()
        {
            var pages = DspCore.Options.GetPages()
                .OrderBy(page => page.Order)
                .ThenBy(page => page.PageId, StringComparer.Ordinal)
                .ToDictionary(page => page.PageId, page => page, StringComparer.Ordinal);
            var options = DspCore.Options.GetAll()
                .Where(IsRenderableOption)
                .OrderBy(option => pages.TryGetValue(option.PageId ?? string.Empty, out var page) ? page.Order : int.MaxValue)
                .ThenBy(option => option.PageId ?? string.Empty, StringComparer.Ordinal)
                .ThenBy(option => option.Order)
                .ThenBy(option => option.Section, StringComparer.Ordinal)
                .ThenBy(option => option.Key, StringComparer.Ordinal)
                .ToArray();
            if (options.Length == 0)
            {
                return new List<PageRow> { new EmptyRow() };
            }

            return options
                .GroupBy(option => option.PageId ?? string.Empty, StringComparer.Ordinal)
                .SelectMany(group =>
                {
                    pages.TryGetValue(group.Key, out var page);
                    var rows = new List<PageRow> { new GroupRow(page?.Title ?? OptionText.General) };
                    rows.AddRange(group.Select(option => new OptionRow(option)));
                    return rows;
                })
                .ToList();
        }
    }

    private abstract record PageRow(float Height)
    {
        public abstract void Build(RectTransform parent, float y, float width);
    }

    private sealed record EmptyRow() : PageRow(RowHeight)
    {
        public override void Build(RectTransform parent, float y, float width)
        {
            UiPageLayout.AddCenteredText(parent, OptionText.NoOptions, UiPageLayout.BodyFontSize, UiPageLayout.EmptyStateTextColor, TextAnchor.MiddleCenter, 0f, y, width, RowHeight, "dspcore-option-empty", wrap: true);
        }
    }

    private sealed record GroupRow(string Title) : PageRow(GroupHeight)
    {
        public override void Build(RectTransform parent, float y, float width)
        {
            UiPageLayout.AddCenteredText(parent, Title, UiPageLayout.CardTitleFontSize, UiStyle.Orange, TextAnchor.MiddleLeft, 0f, y, width, GroupHeight, "dspcore-option-group");
        }
    }

    private sealed record OptionRow(OptionDescriptor Option) : PageRow(RowHeight)
    {
        public override void Build(RectTransform parent, float y, float width)
        {
            var displayName = Option.DisplayName ?? Option.Key;
            UiPageLayout.AddCenteredText(parent, displayName, UiPageLayout.BodyFontSize, UiStyle.White, TextAnchor.MiddleLeft, 0f, y, LabelWidth, RowHeight, "dspcore-option-label", wrap: true);
            BuildControl(parent, y + RowHeight * 0.5f, width);
            if (Option.CanReset)
            {
                AddButton(parent, width - ResetWidth, y + RowHeight * 0.5f, ResetWidth, OptionText.Reset, () =>
                {
                    SetPendingValue(Option, Option.DefaultValue);
                    OptionRuntime.SetString(Option.Section, Option.Key, Option.DefaultValue);
                    RebuildAll(parent);
                });
            }
        }

        private void BuildControl(RectTransform parent, float middleY, float width)
        {
            float controlX = Math.Min(LabelWidth + 24f, width - ControlWidth - ResetWidth - 16f);
            if (Option.Kind == OptionValueKind.Bool)
            {
                bool current = bool.TryParse(GetPendingValue(Option), out var value)
                    ? value
                    : bool.TryParse(Option.DefaultValue, out value) && value;
                var checkBox = UiCheckBox.CreateCheckBox(controlX, middleY, parent, current, string.Empty, UiPageLayout.BodyFontSize);
                checkBox.OnChecked += () => SetPendingValue(Option, checkBox.Checked.ToString());
                return;
            }

            if (Option.Kind == OptionValueKind.Enum && Option.Choices is { Length: > 0 } choices)
            {
                var current = GetPendingValue(Option);
                var selected = Array.FindIndex(choices, choice => choice.Equals(current, StringComparison.OrdinalIgnoreCase));
                if (selected < 0)
                {
                    selected = Math.Max(0, Array.FindIndex(choices, choice => choice.Equals(Option.DefaultValue, StringComparison.OrdinalIgnoreCase)));
                }

                UiComboBox.CreateComboBox(controlX, middleY, parent)
                    .WithItems(choices)
                    .WithIndex(selected)
                    .WithSize(ControlWidth, 0f)
                    .WithFontSize(UiPageLayout.BodyFontSize)
                    .WithOnSelChanged(index =>
                    {
                        if (index >= 0 && index < choices.Length)
                        {
                            SetPendingValue(Option, choices[index]);
                        }
                    });
                return;
            }

            if ((Option.Kind == OptionValueKind.IntRange || Option.Kind == OptionValueKind.FloatRange) &&
                Option.Minimum.HasValue &&
                Option.Maximum.HasValue)
            {
                BuildRange(parent, controlX, middleY);
                return;
            }

            var input = CreateInput(parent, controlX, middleY, ControlWidth, GetPendingValue(Option));
            input.onEndEdit.AddListener(value =>
            {
                if (!IsValid(Option.Kind, value))
                {
                    input.text = GetPendingValue(Option);
                    return;
                }

                SetPendingValue(Option, value);
            });
        }

        private void BuildRange(RectTransform parent, float controlX, float middleY)
        {
            var minimum = Option.Minimum.GetValueOrDefault();
            var maximum = Math.Max(minimum, Option.Maximum.GetValueOrDefault());
            var value = ReadRangeValue(Option, minimum, maximum);
            var slider = UiSlider.CreateSlider(controlX, middleY, parent, value, minimum, maximum, Option.Kind == OptionValueKind.IntRange ? "F0" : "0.###", ControlWidth);
            slider.OnValueChanged += () =>
            {
                var stepped = ApplyStep(slider.Value, Option.Step.GetValueOrDefault(), minimum, maximum);
                if (!Mathf.Approximately(stepped, slider.Value))
                {
                    slider.Value = stepped;
                }

                if (Option.Kind == OptionValueKind.IntRange)
                {
                    var text = Mathf.RoundToInt(stepped).ToString(CultureInfo.InvariantCulture);
                    SetPendingValue(Option, text);
                    slider.SetLabelText(text);
                    return;
                }

                SetPendingValue(Option, stepped.ToString(CultureInfo.InvariantCulture));
                slider.SetLabelText(stepped.ToString("0.###", CultureInfo.InvariantCulture));
            };
        }

        private static void RebuildAll(RectTransform parent)
        {
            var root = parent;
            while (root != null && root.name != PageObjectName)
            {
                root = root.parent as RectTransform;
            }

            if (root == null)
            {
                return;
            }

            foreach (Transform child in root)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

            var host = root.GetComponent<UiOptionPageHost>();
            typeof(UiOptionPageHost).GetField("built", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(host, false);
            host.Rebuild(root);
        }
    }

    private static InputField CreateInput(RectTransform parent, float x, float y, float width, string text)
    {
        var input = UnityEngine.Object.Instantiate(UIRoot.instance.uiGame.stationWindow.nameInput);
        input.gameObject.name = "dspcore-option-input";
        var button = input.GetComponent<UIButton>();
        if (button != null)
        {
            UnityEngine.Object.Destroy(button);
        }

        var image = input.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(1f, 1f, 1f, 0.05f);
        }

        var rect = input.transform as RectTransform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        input.text = text;
        input.textComponent.fontSize = UiPageLayout.BodyFontSize;
        input.onValueChanged.RemoveAllListeners();
        input.onEndEdit.RemoveAllListeners();
        return input;
    }

    private static void AddButton(RectTransform parent, float x, float y, float width, string text, UnityAction onClick)
    {
        var button = UnityEngine.Object.Instantiate(UIRoot.instance.uiGame.statWindow.performancePanelUI.cpuActiveButton);
        button.gameObject.name = "dspcore-option-button";
        var rect = button.transform as RectTransform;
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(x, -y);
        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        button.CloseTip();
        button.tips = new UIButton.TipSettings();
        button.SetText(text);
        button.button.onClick.RemoveAllListeners();
        button.button.onClick.AddListener(onClick);
    }

    private static float ReadRangeValue(OptionDescriptor option, float minimum, float maximum)
    {
        var text = GetPendingValue(option);
        if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            float.TryParse(option.DefaultValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        return Math.Min(maximum, Math.Max(minimum, value));
    }

    private static float ApplyStep(float value, float step, float minimum, float maximum)
    {
        if (step > 0f)
        {
            value = minimum + Mathf.Round((value - minimum) / step) * step;
        }

        return Math.Min(maximum, Math.Max(minimum, value));
    }

    private static bool IsValid(OptionValueKind kind, string value)
    {
        return kind switch
        {
            OptionValueKind.Int => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            OptionValueKind.Float => float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _),
            _ => true
        };
    }
}

internal static class OptionPageRuntimePatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIOptionWindow), "_OnCreate")]
    private static void OnCreatePrefix()
    {
        KeyBindRuntime.EnsureRegisteredToVanilla();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIOptionWindow), "_OnCreate")]
    private static void OnCreate(UIOptionWindow __instance)
    {
        OptionPageRuntime.EnsurePage(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
    private static void OnOpen(UIOptionWindow __instance)
    {
        KeyBindRuntime.EnsureRuntimeArrays();
        OptionPageRuntime.EnsurePage(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIOptionWindow), "OnTabButtonClick")]
    private static void OnTabButtonClick(UIOptionWindow __instance)
    {
        OptionPageRuntime.ApplyVisibility(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIOptionWindow), nameof(UIOptionWindow.OnApplyClick))]
    private static void OnApply()
    {
        OptionPageRuntime.SaveOptions();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIOptionWindow), nameof(UIOptionWindow.OnCancelClick))]
    private static void OnCancel()
    {
        OptionPageRuntime.DiscardOptions();
    }
}
