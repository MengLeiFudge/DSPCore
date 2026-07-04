#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace DSPCore;

internal sealed class GlobalSavesWindow : UiWindow
{
    private const float HeaderRowHeight = 64f;
    private const float SummaryRowHeight = 34f;
    private const float BlockRowHeight = 42f;
    private const float EmptyRowHeight = 42f;
    private const float Gap = 8f;

    protected override void _OnCreate()
    {
        base._OnCreate();
        var root = GetComponent<RectTransform>();
        var overview = GlobalSaveRuntime.CreateOverview();
        var rows = BuildRows(overview);
        float contentHeight = Math.Max(160f, rows.Sum(row => row.Height) + Math.Max(0, rows.Count - 1) * Gap);

        var layout = GridDsl.Grid(
            rows: [GridDsl.Px(72f), GridDsl.Fr(1f), GridDsl.Px(56f)],
            cols: [GridDsl.Fr(1f)],
            rowGap: UiPageLayout.Gap,
            children:
            [
                GridDsl.Header(OptionText.GlobalSavesTitle, OptionText.GlobalSavesSummary, row: 0, col: 0),
                GridDsl.ScrollableContentCard(
                    contentHeight: contentHeight,
                    row: 1,
                    col: 0,
                    rows: rows.Select(row => GridDsl.Px(row.Height)).ToArray(),
                    cols: [GridDsl.Fr(1f)],
                    rowGap: Gap,
                    children: rows.Select((row, index) => row.Node(index)).ToArray()),
                GridDsl.FooterCard(
                    row: 2,
                    col: 0,
                    cols: [GridDsl.Fr(1f), GridDsl.Px(120f)],
                    columnGap: 8f,
                    children:
                    [
                        GridDsl.TextNode(OptionText.GlobalSavesFooter, row: 0, col: 0, wrap: true),
                        GridDsl.ButtonNode(OptionText.Close, Close, row: 0, col: 1)
                    ])
            ]);

        GridDsl.BuildLayout(this, root, layout);
    }

    private static List<RowModel> BuildRows(GlobalSaveOverview overview)
    {
        var rows = new List<RowModel>
        {
            new HeaderRowModel(string.Format(CultureInfo.InvariantCulture, OptionText.GlobalSavesPath.Translate(), overview.Path)),
            new SummaryRowModel(string.Format(
                CultureInfo.InvariantCulture,
                OptionText.GlobalSavesCounts.Translate(),
                overview.RegisteredCount,
                overview.FileBlockCount,
                overview.IsLoaded))
        };

        if (overview.Blocks.Count == 0)
        {
            rows.Add(new EmptyRowModel());
            return rows;
        }

        rows.AddRange(overview.Blocks
            .OrderBy(block => block.ModGuid, StringComparer.Ordinal)
            .Select(block => new BlockRowModel(block)));
        return rows;
    }

    private static void BuildBlockRow(UiWindow window, RectTransform root, GlobalSaveBlockSnapshot block)
    {
        const float statusWidth = 120f;
        const float sizeWidth = 110f;
        const float gap = 12f;
        var y = root.sizeDelta.y / 2f;
        var modWidth = Math.Max(160f, root.sizeDelta.x - statusWidth - sizeWidth - gap * 2f);
        var modText = window.AddText2(0f, y, root, block.ModGuid, UiPageLayout.BodyFontSize);
        modText.rectTransform.sizeDelta = new Vector2(modWidth, modText.rectTransform.sizeDelta.y);

        var statusText = window.AddText2(modWidth + gap, y, root, GetStatusText(block), UiPageLayout.BodyFontSize);
        statusText.rectTransform.sizeDelta = new Vector2(statusWidth, statusText.rectTransform.sizeDelta.y);

        var bytesText = string.Format(CultureInfo.InvariantCulture, OptionText.GlobalSavesBytes.Translate(), block.ByteCount);
        var sizeText = window.AddText2(modWidth + statusWidth + gap * 2f, y, root, bytesText, UiPageLayout.BodyFontSize);
        sizeText.rectTransform.sizeDelta = new Vector2(sizeWidth, sizeText.rectTransform.sizeDelta.y);
    }

    private static string GetStatusText(GlobalSaveBlockSnapshot block)
    {
        if (block.IsRegistered && block.IsLoadedFromFile)
        {
            return OptionText.GlobalSavesRegistered.Translate();
        }

        if (block.IsRegistered)
        {
            return OptionText.GlobalSavesInitialized.Translate();
        }

        return OptionText.GlobalSavesFileOnly.Translate();
    }

    private abstract record RowModel(float Height)
    {
        public abstract LayoutNode Node(int row);
    }

    private sealed record HeaderRowModel(string Text) : RowModel(HeaderRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.TextNode(Text, row: row, col: 0, color: UiStyle.MutedWhite, wrap: true);
        }
    }

    private sealed record SummaryRowModel(string Text) : RowModel(SummaryRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.CardTitleNode(Text, row: row, col: 0);
        }
    }

    private sealed record EmptyRowModel() : RowModel(EmptyRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.TextNode(OptionText.GlobalSavesNoBlocks, row: row, col: 0);
        }
    }

    private sealed record BlockRowModel(GlobalSaveBlockSnapshot Block) : RowModel(BlockRowHeight)
    {
        public override LayoutNode Node(int row)
        {
            return GridDsl.Node(row: row, col: 0, build: (window, root) => BuildBlockRow(window, root, Block));
        }
    }
}
