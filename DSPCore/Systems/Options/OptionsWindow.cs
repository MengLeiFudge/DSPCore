#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSPCore;

internal sealed class OptionsWindow : UiWindow
{
    private const float PageHeaderHeight = 30f;
    private const float OptionRowHeight = 46f;
    private const float EmptyRowHeight = 42f;
    private const float LabelWidth = 230f;
    private const float DescriptionWidth = 360f;
    private const float CaptureButtonWidth = 86f;
    private const float Gap = 12f;
    private KeyCaptureState keyCapture;

    protected override void _OnCreate()
    {
        base._OnCreate();
        var root = GetComponent<RectTransform>();
        var groups = BuildGroups();
        var rows = BuildRows(groups);
        float contentHeight = Math.Max(160f, rows.Sum(row => row.Height) + Math.Max(0, rows.Count - 1) * 8f);

        var layout = GridDsl.Grid(
            rows: [GridDsl.Px(72f), GridDsl.Fr(1f), GridDsl.Px(56f)],
            cols: [GridDsl.Fr(1f)],
            rowGap: UiPageLayout.Gap,
            children:
            [
                GridDsl.Header("DSPCore Settings", "Unified options registered by mods.", row: 0, col: 0),
                GridDsl.ScrollableContentCard(
                    contentHeight: contentHeight,
                    row: 1,
                    col: 0,
                    rows: rows.Select(row => GridDsl.Px(row.Height)).ToArray(),
                    cols: [GridDsl.Fr(1f)],
                    rowGap: 8f,
                    children: rows.Select((row, index) => row.Node(index)).ToArray()),
                GridDsl.FooterCard(
                    row: 2,
                    col: 0,
                    cols: [GridDsl.Fr(1f), GridDsl.Px(120f)],
                    children:
                    [
                        GridDsl.TextNode("Changes are written to the DSPCore BepInEx config.", row: 0, col: 0),
                        GridDsl.ButtonNode("Close", Close, row: 0, col: 1)
                    ])
            ]);

        GridDsl.BuildLayout(this, root, layout);
    }

    protected override void _OnUpdate()
    {
        base._OnUpdate();
        UpdateKeyCapture();
    }

    private static List<OptionGroup> BuildGroups()
    {
        var pages = DspCore.Options.GetPages()
            .OrderBy(page => page.Order)
            .ThenBy(page => page.PageId, StringComparer.Ordinal)
            .ToDictionary(page => page.PageId, page => page, StringComparer.Ordinal);

        return DspCore.Options.GetAll()
            .OrderBy(option => pages.TryGetValue(option.PageId ?? string.Empty, out var page) ? page.Order : int.MaxValue)
            .ThenBy(option => option.PageId ?? string.Empty, StringComparer.Ordinal)
            .ThenBy(option => option.Section, StringComparer.Ordinal)
            .ThenBy(option => option.Key, StringComparer.Ordinal)
            .GroupBy(option => option.PageId ?? string.Empty, StringComparer.Ordinal)
            .Select(group =>
            {
                pages.TryGetValue(group.Key, out var page);
                return new OptionGroup(page?.Title ?? "General", group.ToList());
            })
            .ToList();
    }

    private static List<RowModel> BuildRows(List<OptionGroup> groups)
    {
        var rows = new List<RowModel>();
        if (groups.Count == 0)
        {
            rows.Add(new EmptyRowModel());
            return rows;
        }

        foreach (var group in groups)
        {
            rows.Add(new PageRowModel(group.Title));
            rows.AddRange(group.Options.Select(option => new OptionRowModel(option)));
        }

        return rows;
    }

    private static void BuildOptionRow(UiWindow window, RectTransform root, OptionDescriptor option)
    {
        float height = root.sizeDelta.y;
        float controlX = LabelWidth + DescriptionWidth + Gap * 2f;
        float controlWidth = Math.Max(160f, root.sizeDelta.x - controlX);
        window.AddText2(0f, height / 2f, root, option.DisplayName ?? option.Key, UiPageLayout.BodyFontSize);
        var descriptionText = window.AddText2(LabelWidth + Gap, height / 2f, root, GetOptionDescription(option), UiPageLayout.BodyFontSize);
        descriptionText.rectTransform.sizeDelta = new Vector2(DescriptionWidth, descriptionText.rectTransform.sizeDelta.y);

        if (option.Kind == OptionValueKind.Bool)
        {
            bool current = DspCore.Options.GetBool(option.Section, option.Key, bool.TryParse(option.DefaultValue, out var fallback) && fallback);
            var checkBox = UiCheckBox.CreateCheckBox(controlX, height / 2f, root, current, string.Empty, UiPageLayout.BodyFontSize);
            checkBox.OnChecked += () => OptionRuntime.SetString(option.Section, option.Key, checkBox.Checked.ToString());
            return;
        }

        if (option.Kind == OptionValueKind.Enum && option.Choices is { Length: > 0 } choices)
        {
            var current = DspCore.Options.GetString(option.Section, option.Key);
            var selected = Array.FindIndex(choices, choice => choice.Equals(current, StringComparison.OrdinalIgnoreCase));
            if (selected < 0)
            {
                selected = Math.Max(0, Array.FindIndex(choices, choice => choice.Equals(option.DefaultValue, StringComparison.OrdinalIgnoreCase)));
            }

            window.AddComboBox(controlX, height / 2f, root, UiPageLayout.BodyFontSize)
                .WithItems(choices)
                .WithIndex(selected)
                .WithSize(controlWidth, 0f)
                .WithOnSelChanged(index =>
                {
                    if (index >= 0 && index < choices.Length)
                    {
                        OptionRuntime.SetString(option.Section, option.Key, choices[index]);
                    }
                });
            return;
        }

        if ((option.Kind == OptionValueKind.IntRange || option.Kind == OptionValueKind.FloatRange) &&
            option.Minimum.HasValue &&
            option.Maximum.HasValue)
        {
            BuildRangeControl(window, root, option, controlX, height / 2f, controlWidth);
            return;
        }

        var optionsWindow = window as OptionsWindow;
        var keyBinding = option.Kind == OptionValueKind.KeyBinding && optionsWindow != null;
        var inputWidth = keyBinding
            ? Math.Max(120f, controlWidth - CaptureButtonWidth - Gap)
            : controlWidth;
        var input = window.AddInputField(
            controlX,
            height / 2f,
            root,
            DspCore.Options.GetString(option.Section, option.Key),
            UiPageLayout.BodyFontSize);
        input.GetComponent<RectTransform>().sizeDelta = new Vector2(inputWidth, input.GetComponent<RectTransform>().sizeDelta.y);
        input.onEndEdit.AddListener(value =>
        {
            if (!IsValid(option.Kind, value))
            {
                input.text = DspCore.Options.GetString(option.Section, option.Key);
                return;
            }

            OptionRuntime.SetString(option.Section, option.Key, value);
            descriptionText.text = GetOptionDescription(option).Translate();
        });

        if (keyBinding)
        {
            window.AddButton(
                controlX + inputWidth + Gap,
                height / 2f,
                CaptureButtonWidth,
                root,
                "Capture",
                UiPageLayout.BodyFontSize,
                "capture-key-button",
                () => optionsWindow.StartKeyCapture(option, input, descriptionText));
        }
    }

    private void StartKeyCapture(OptionDescriptor option, InputField input, Text descriptionText)
    {
        keyCapture = new KeyCaptureState(option, input, descriptionText, Time.frameCount);
        input.text = "Press a key...";
        input.ActivateInputField();
    }

    private void UpdateKeyCapture()
    {
        if (keyCapture == null || Time.frameCount <= keyCapture.StartFrame)
        {
            return;
        }

        if (!KeyBindRuntime.TryCaptureCurrentKey(out var keyText))
        {
            return;
        }

        OptionRuntime.SetString(keyCapture.Option.Section, keyCapture.Option.Key, keyText);
        keyCapture.Input.text = keyText;
        keyCapture.DescriptionText.text = GetOptionDescription(keyCapture.Option).Translate();
        keyCapture.Input.DeactivateInputField();
        keyCapture = null;
    }

    private static void BuildRangeControl(UiWindow window, RectTransform root, OptionDescriptor option, float x, float y, float width)
    {
        var minimum = option.Minimum.GetValueOrDefault();
        var maximum = Math.Max(minimum, option.Maximum.GetValueOrDefault());
        var value = ReadRangeValue(option, minimum, maximum);
        var slider = window.AddSlider(x, y, root, value, minimum, maximum, option.Kind == OptionValueKind.IntRange ? "F0" : "0.###", width);

        slider.OnValueChanged += () =>
        {
            var stepped = ApplyStep(slider.Value, option.Step.GetValueOrDefault(), minimum, maximum);
            if (!Mathf.Approximately(stepped, slider.Value))
            {
                slider.Value = stepped;
            }

            if (option.Kind == OptionValueKind.IntRange)
            {
                OptionRuntime.SetString(option.Section, option.Key, Mathf.RoundToInt(stepped).ToString(CultureInfo.InvariantCulture));
                slider.SetLabelText(Mathf.RoundToInt(stepped).ToString(CultureInfo.InvariantCulture));
                return;
            }

            OptionRuntime.SetString(option.Section, option.Key, stepped.ToString(CultureInfo.InvariantCulture));
            slider.SetLabelText(stepped.ToString("0.###", CultureInfo.InvariantCulture));
        };
    }

    private static float ReadRangeValue(OptionDescriptor option, float minimum, float maximum)
    {
        var text = DspCore.Options.GetString(option.Section, option.Key);
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
            OptionValueKind.KeyBinding => KeyBindRuntime.IsValidKeyText(value),
            _ => true
        };
    }

    private static string GetOptionDescription(OptionDescriptor option)
    {
        return option.Kind == OptionValueKind.KeyBinding
            ? KeyBinds.BuildOptionDescription(option)
            : option.Description;
    }

    private abstract record RowModel(float Height)
    {
        public abstract LayoutNode Node(int row);
    }

    private sealed record EmptyRowModel() : RowModel(EmptyRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.TextNode("No options registered.", row: row, col: 0);
        }
    }

    private sealed record PageRowModel(string Title) : RowModel(PageHeaderHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.CardTitleNode(Title, row: row, col: 0);
        }
    }

    private sealed record OptionRowModel(OptionDescriptor Option) : RowModel(OptionRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.Node(row: row, col: 0, build: (window, root) => BuildOptionRow(window, root, Option));
        }
    }

    private sealed record OptionGroup(string Title, IReadOnlyList<OptionDescriptor> Options);

    private sealed record KeyCaptureState(OptionDescriptor Option, InputField Input, Text DescriptionText, int StartFrame);
}
