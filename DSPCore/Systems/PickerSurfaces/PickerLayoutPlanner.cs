using System;
using System.Collections.Generic;

namespace DSPCore;

internal readonly struct PickerGridMetrics
{
    public PickerGridMetrics(int columns, int rows)
    {
        Columns = Math.Max(1, columns);
        Rows = Math.Max(1, rows);
    }

    public int Columns { get; }

    public int Rows { get; }

    public int Capacity => Columns * Rows;
}

internal readonly struct PickerLayoutEntry<T>
{
    public PickerLayoutEntry(int gridIndex, uint iconIndex, T value)
    {
        GridIndex = gridIndex;
        IconIndex = iconIndex;
        Value = value;
    }

    public int GridIndex { get; }

    public uint IconIndex { get; }

    public T Value { get; }
}

internal readonly struct PickerResolvedEntry<T>
{
    public PickerResolvedEntry(int slot, uint iconIndex, T value)
    {
        Slot = slot;
        IconIndex = iconIndex;
        Value = value;
    }

    public int Slot { get; }

    public uint IconIndex { get; }

    public T Value { get; }
}

internal sealed class PickerLayoutResult<T>
{
    public PickerLayoutResult(PickerGridMetrics metrics, IReadOnlyList<PickerResolvedEntry<T>> entries)
    {
        Metrics = metrics;
        Entries = entries;
    }

    public PickerGridMetrics Metrics { get; }

    public IReadOnlyList<PickerResolvedEntry<T>> Entries { get; }
}

internal static class PickerLayoutPlanner
{
    public static PickerLayoutResult<T> Plan<T>(IReadOnlyList<PickerLayoutEntry<T>> entries, int defaultColumns, int defaultRows)
    {
        var columns = Math.Max(1, defaultColumns);
        var rows = Math.Max(1, defaultRows);
        for (var i = 0; i < entries.Count; i++)
        {
            var row = GetGridRow(entries[i].GridIndex);
            var column = GetGridColumn(entries[i].GridIndex);
            if (row < 0 || column < 0)
            {
                continue;
            }

            rows = Math.Max(rows, row + 1);
            columns = Math.Max(columns, column + 1);
        }

        rows = Math.Max(rows, (entries.Count + columns - 1) / columns);
        var metrics = new PickerGridMetrics(columns, rows);
        var occupied = new bool[metrics.Capacity];
        var resolved = new List<PickerResolvedEntry<T>>(entries.Count);
        for (var i = 0; i < entries.Count; i++)
        {
            var row = GetGridRow(entries[i].GridIndex);
            var column = GetGridColumn(entries[i].GridIndex);
            if (row < 0 || column < 0)
            {
                continue;
            }

            var preferredSlot = row * metrics.Columns + column;
            var slot = FindVisibleSlot(occupied, preferredSlot);
            if (slot < 0)
            {
                continue;
            }

            occupied[slot] = true;
            resolved.Add(new PickerResolvedEntry<T>(slot, entries[i].IconIndex, entries[i].Value));
        }

        return new PickerLayoutResult<T>(metrics, resolved);
    }

    public static bool IsOnPage(int gridIndex, int page)
    {
        return gridIndex >= 1101 && gridIndex / 1000 == page && GetGridRow(gridIndex) >= 0 && GetGridColumn(gridIndex) >= 0;
    }

    public static bool TryGetCell(int gridIndex, out int row, out int column)
    {
        row = GetGridRow(gridIndex);
        column = GetGridColumn(gridIndex);
        return gridIndex >= 1101 && row >= 0 && column >= 0;
    }

    private static int GetGridRow(int gridIndex)
    {
        var page = gridIndex / 1000;
        return (gridIndex - page * 1000) / 100 - 1;
    }

    private static int GetGridColumn(int gridIndex)
    {
        return gridIndex % 100 - 1;
    }

    private static int FindVisibleSlot(bool[] occupied, int preferredSlot)
    {
        if (preferredSlot >= 0 && preferredSlot < occupied.Length && !occupied[preferredSlot])
        {
            return preferredSlot;
        }

        for (var i = Math.Max(0, preferredSlot + 1); i < occupied.Length; i++)
        {
            if (!occupied[i])
            {
                return i;
            }
        }

        for (var i = 0; i < Math.Min(preferredSlot, occupied.Length); i++)
        {
            if (!occupied[i])
            {
                return i;
            }
        }

        return -1;
    }
}
