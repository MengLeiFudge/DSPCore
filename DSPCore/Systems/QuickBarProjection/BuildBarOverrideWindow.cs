#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DSPCore;

internal sealed class BuildBarOverrideWindow : UiWindow
{
    private const int CategoryCount = 12;
    private const int MinimumRowCount = 2;
    private const int IndexCount = 12;
    private const float CategoryRowHeight = 42f;
    private const float SlotRowHeight = 42f;
    private const float Gap = 8f;
    private static BuildBarSlot selectedSlot = new(1, 1, 1);

    public static BuildBarSlot SelectedSlot => selectedSlot;

    protected override void _OnCreate()
    {
        base._OnCreate();
        Build();
    }

    private void Build()
    {
        var root = GetComponent<RectTransform>();
        var rows = new List<RowModel>();
        for (var category = 1; category <= CategoryCount; category++)
        {
            rows.Add(new CategoryRowModel(category));
            for (var row = 1; row <= GetEditableRowCount(); row++)
            {
                rows.Add(new SlotRowModel(category, row));
            }
        }

        var layout = GridDsl.Grid(
            rows: [GridDsl.Px(72f), GridDsl.Fr(1f), GridDsl.Px(56f)],
            cols: [GridDsl.Fr(1f)],
            rowGap: UiPageLayout.Gap,
            children:
            [
                GridDsl.Header(BuildBarText.Title, BuildBarText.Summary, row: 0, col: 0),
                GridDsl.ScrollableContentCard(
                    contentHeight: rows.Sum(item => item.Height) + Math.Max(0, rows.Count - 1) * Gap,
                    row: 1,
                    col: 0,
                    rows: rows.Select(item => GridDsl.Px(item.Height)).ToArray(),
                    cols: [GridDsl.Fr(1f)],
                    rowGap: Gap,
                    children: rows.Select((item, index) => item.Node(index)).ToArray()),
                GridDsl.FooterCard(
                    row: 2,
                    col: 0,
                    cols: [GridDsl.Fr(1f), GridDsl.Px(100f), GridDsl.Px(84f), GridDsl.Px(84f), GridDsl.Px(120f)],
                    columnGap: 8f,
                    children:
                    [
                        GridDsl.TextNode(GetFooterText(), row: 0, col: 0, wrap: true),
                        GridDsl.ButtonNode(BuildBarText.SelectItem, () => BuildBarRuntime.OpenSlotEditor(selectedSlot), row: 0, col: 1),
                        GridDsl.ButtonNode(BuildBarText.ClearSlot, () => BuildBarRuntime.ClearSlot(selectedSlot), row: 0, col: 2),
                        GridDsl.ButtonNode(BuildBarText.UseDefault, () => BuildBarRuntime.UseDefault(selectedSlot), row: 0, col: 3),
                        GridDsl.ButtonNode(BuildBarText.Close, Close, row: 0, col: 4)
                    ])
            ]);

        GridDsl.BuildLayout(this, root, layout);
    }

    private static void BuildSlotRow(UiWindow window, RectTransform root, int category, int row)
    {
        const float labelWidth = 76f;
        const float buttonGap = 6f;
        var cellWidth = Math.Max(54f, (root.sizeDelta.x - labelWidth - buttonGap * (IndexCount - 1)) / IndexCount);
        var y = root.sizeDelta.y / 2f;
        window.AddText2(0f, y, root, string.Format(CultureInfo.InvariantCulture, BuildBarText.RowLabel.Translate(), row), UiPageLayout.BodyFontSize);

        for (var index = 1; index <= IndexCount; index++)
        {
            var slot = new BuildBarSlot(category, row, index);
            var x = labelWidth + (index - 1) * (cellWidth + buttonGap);
            var label = GetSlotLabel(slot);
            var capturedSlot = slot;
            window.AddButton(
                x,
                y,
                cellWidth,
                root,
                label,
                UiPageLayout.HintFontSize,
                $"buildbar-slot-{category}-{row}-{index}",
                () => SelectSlot(capturedSlot));
        }
    }

    private static string GetSlotLabel(BuildBarSlot slot)
    {
        var prefix = slot.Equals(selectedSlot) ? "> " : string.Empty;
        var overrides = DspCore.BuildBar.GetPlayerOverrides();
        if (overrides.TryGetValue(slot, out var overrideItemId))
        {
            return prefix + (overrideItemId == 0 ? BuildBarText.ExplicitEmpty.Translate() : GetItemName(overrideItemId));
        }

        if (DspCore.BuildBar.GetAll().TryGetValue(slot, out var defaultItemId))
        {
            return prefix + GetItemName(defaultItemId);
        }

        return prefix + BuildBarText.EmptySlot.Translate();
    }

    private static int GetEditableRowCount()
    {
        return Math.Max(
            MinimumRowCount,
            DspCore.BuildBar.GetAll()
                .Keys
                .Concat(DspCore.BuildBar.GetPlayerOverrides().Keys)
                .Select(slot => slot.Row)
                .DefaultIfEmpty(MinimumRowCount)
                .Max());
    }

    private static void SelectSlot(BuildBarSlot slot)
    {
        selectedSlot = slot;
        BuildBarRuntime.ReopenOverrideWindow();
    }

    private static string GetFooterText()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0} C{1} R{2} #{3}",
            BuildBarText.Footer.Translate(),
            selectedSlot.Category,
            selectedSlot.Row,
            selectedSlot.Index);
    }

    private static string GetItemName(int itemId)
    {
        var item = LDB.items?.Select(itemId);
        var name = item?.Name.Translate();
        return string.IsNullOrWhiteSpace(name) ? itemId.ToString(CultureInfo.InvariantCulture) : name;
    }

    private abstract record RowModel(float Height)
    {
        public abstract LayoutNode Node(int row);
    }

    private sealed record CategoryRowModel(int Category) : RowModel(CategoryRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.CardTitleNode(
                string.Format(CultureInfo.InvariantCulture, BuildBarText.CategoryPage.Translate(), Category),
                row: row,
                col: 0);
        }
    }

    private sealed record SlotRowModel(int Category, int BuildBarRow) : RowModel(SlotRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.Node(row: row, col: 0, build: (window, root) => BuildSlotRow(window, root, Category, BuildBarRow));
        }
    }
}
