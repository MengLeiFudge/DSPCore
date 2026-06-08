using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace DSPCore;

internal static class PickerRuntime
{
    private const int DefaultItemPickerColumns = 14;
    private const int DefaultItemPickerRows = 8;
    private const int DefaultRecipePickerColumns = 14;
    private const int DefaultRecipePickerRows = 8;
    private const int DefaultSignalPickerColumns = 14;
    private const int DefaultSignalPickerRows = 10;
    private const float LargePickerGridSize = 46f;
    private const float TagPickerGridSize = 26f;

    private static readonly FieldInfo? ItemPickerIndexArrayField = AccessTools.Field(typeof(UIItemPicker), "indexArray");
    private static readonly FieldInfo? ItemPickerProtoArrayField = AccessTools.Field(typeof(UIItemPicker), "protoArray");
    private static readonly FieldInfo? ItemPickerCurrentTypeField = AccessTools.Field(typeof(UIItemPicker), "currentType");
    private static readonly FieldInfo? ItemPickerIndexBufferField = AccessTools.Field(typeof(UIItemPicker), "indexBuffer");
    private static readonly FieldInfo? ItemPickerIconMatField = AccessTools.Field(typeof(UIItemPicker), "iconMat");
    private static readonly FieldInfo? ItemPickerMouseInBoxField = AccessTools.Field(typeof(UIItemPicker), "mouseInBox");
    private static readonly FieldInfo? ItemPickerHoveredIndexField = AccessTools.Field(typeof(UIItemPicker), "hoveredIndex");
    private static readonly FieldInfo? RecipePickerIndexArrayField = AccessTools.Field(typeof(UIRecipePicker), "indexArray");
    private static readonly FieldInfo? RecipePickerProtoArrayField = AccessTools.Field(typeof(UIRecipePicker), "protoArray");
    private static readonly FieldInfo? RecipePickerCurrentTypeField = AccessTools.Field(typeof(UIRecipePicker), "currentType");
    private static readonly FieldInfo? RecipePickerFilterField = AccessTools.Field(typeof(UIRecipePicker), "filter");
    private static readonly FieldInfo? RecipePickerIndexBufferField = AccessTools.Field(typeof(UIRecipePicker), "indexBuffer");
    private static readonly FieldInfo? RecipePickerIconMatField = AccessTools.Field(typeof(UIRecipePicker), "iconMat");
    private static readonly FieldInfo? RecipePickerMouseInBoxField = AccessTools.Field(typeof(UIRecipePicker), "mouseInBox");
    private static readonly FieldInfo? RecipePickerHoveredIndexField = AccessTools.Field(typeof(UIRecipePicker), "hoveredIndex");
    private static readonly FieldInfo? SignalPickerIndexArrayField = AccessTools.Field(typeof(UISignalPicker), "indexArray");
    private static readonly FieldInfo? SignalPickerSignalArrayField = AccessTools.Field(typeof(UISignalPicker), "signalArray");
    private static readonly FieldInfo? SignalPickerCurrentTypeField = AccessTools.Field(typeof(UISignalPicker), "currentType");
    private static readonly FieldInfo? SignalPickerIndexBufferField = AccessTools.Field(typeof(UISignalPicker), "indexBuffer");
    private static readonly FieldInfo? SignalPickerIconMatField = AccessTools.Field(typeof(UISignalPicker), "iconMat");
    private static readonly FieldInfo? SignalPickerMouseInBoxField = AccessTools.Field(typeof(UISignalPicker), "mouseInBox");
    private static readonly FieldInfo? SignalPickerHoveredIndexField = AccessTools.Field(typeof(UISignalPicker), "hoveredIndex");
    private static readonly FieldInfo? SignalTagPickerIndexArrayField = AccessTools.Field(typeof(UISignalTagPicker), "indexArray");
    private static readonly FieldInfo? SignalTagPickerSignalArrayField = AccessTools.Field(typeof(UISignalTagPicker), "signalArray");
    private static readonly FieldInfo? SignalTagPickerCurrentTypeField = AccessTools.Field(typeof(UISignalTagPicker), "currentType");
    private static readonly FieldInfo? SignalTagPickerShowUnlockField = AccessTools.Field(typeof(UISignalTagPicker), "showUnlock");
    private static readonly FieldInfo? SignalTagPickerIndexBufferField = AccessTools.Field(typeof(UISignalTagPicker), "indexBuffer");
    private static readonly FieldInfo? SignalTagPickerIconMatField = AccessTools.Field(typeof(UISignalTagPicker), "iconMat");
    private static readonly FieldInfo? SignalTagPickerMouseInBoxField = AccessTools.Field(typeof(UISignalTagPicker), "mouseInBox");
    private static readonly FieldInfo? SignalTagPickerHoveredIndexField = AccessTools.Field(typeof(UISignalTagPicker), "hoveredIndex");
    private static readonly MethodInfo GetRuntimeColumnCountMethod = AccessTools.Method(typeof(PickerRuntime), nameof(GetRuntimeColumnCount));
    private static readonly MethodInfo GetRuntimeRowCountMethod = AccessTools.Method(typeof(PickerRuntime), nameof(GetRuntimeRowCount));
    private static readonly Dictionary<object, PickerGridMetrics> BaselineMetrics = new();
    private static readonly Dictionary<object, PickerGridMetrics> LayoutMetrics = new();

    private static PickerRequest? activeItemRequest;
    private static PickerRequest? activeRecipeRequest;
    private static PickerRequest? activeSignalRequest;

    public static void Update()
    {
        foreach (var request in DspCore.Pickers.ConsumeAll())
        {
            Open(request);
        }
    }

    private static void Open(PickerRequest request)
    {
        var position = new Vector2(0f, 0f);
        try
        {
            switch (request.Kind)
            {
                case PickerKind.Item:
                    activeItemRequest = request;
                    UIItemPicker.showAll = request.ShowAll || request.ShowLocked;
                    UIItemPicker.Popup(position, item =>
                    {
                        activeItemRequest = null;
                        if (item != null && request.Filter != null && !request.Filter(item))
                        {
                            request.OnReturn?.Invoke(null);
                            return;
                        }

                        request.OnReturn?.Invoke(item);
                    });
                    break;
                case PickerKind.Recipe:
                    activeRecipeRequest = request;
                    UIRecipePicker.Popup(position, recipe =>
                    {
                        activeRecipeRequest = null;
                        if (recipe != null && request.Filter != null && !request.Filter(recipe))
                        {
                            request.OnReturn?.Invoke(null);
                            return;
                        }

                        request.OnReturn?.Invoke(recipe);
                    });
                    break;
                case PickerKind.Signal:
                    activeSignalRequest = request;
                    UISignalPicker.Popup(position, signalId =>
                    {
                        activeSignalRequest = null;
                        if (signalId != 0 && request.Filter != null && !request.Filter(signalId))
                        {
                            request.OnReturn?.Invoke(null);
                            return;
                        }

                        request.OnReturn?.Invoke(signalId);
                    });
                    break;
            }
        }
        catch (Exception ex)
        {
            ClearActiveRequest(request.Kind);
            DspCore.Errors.ReportException(request.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"Picker request {request.Kind} from {request.OwnerModGuid} failed: {ex}");
            request.OnReturn?.Invoke(null);
        }
    }

    public static void RefreshItemPicker(UIItemPicker picker)
    {
        if (picker == null ||
            ItemPickerIndexArrayField?.GetValue(picker) is not uint[] indexArray ||
            ItemPickerProtoArrayField?.GetValue(picker) is not ItemProto[] protoArray ||
            ItemPickerCurrentTypeField?.GetValue(picker) is not int currentType ||
            GameMain.iconSet == null ||
            LDB.items?.dataArray == null)
        {
            return;
        }

        var entries = new List<PickerLayoutEntry<ItemProto>>();
        var seenItems = new HashSet<int>();
        foreach (var item in LDB.items.dataArray)
        {
            if (!ShouldShowItem(item, currentType))
            {
                continue;
            }

            if (activeItemRequest?.Filter != null && !activeItemRequest.Filter(item))
            {
                continue;
            }

            if (!seenItems.Add(item.ID))
            {
                continue;
            }

            entries.Add(new PickerLayoutEntry<ItemProto>(item.GridIndex, GameMain.iconSet.itemIconIndex[item.ID], item));
        }

        var baseline = MeasureItemGridMetrics(GetOrCaptureBaselineMetrics(picker, picker.iconImage, DefaultItemPickerColumns, DefaultItemPickerRows, LargePickerGridSize));
        var layout = PickerLayoutPlanner.Plan(entries, baseline.Columns, baseline.Rows);
        EnsureItemPickerCapacity(picker, layout.Metrics.Capacity, ref indexArray, ref protoArray);
        ClearPickerArrays(indexArray, protoArray);
        FillProtoLayout(layout, indexArray, protoArray);
        ApplyItemPickerLayout(picker, layout.Metrics);
    }

    public static void RefreshRecipePicker(UIRecipePicker picker)
    {
        if (picker == null ||
            RecipePickerIndexArrayField?.GetValue(picker) is not uint[] indexArray ||
            RecipePickerProtoArrayField?.GetValue(picker) is not RecipeProto[] protoArray ||
            RecipePickerCurrentTypeField?.GetValue(picker) is not int currentType ||
            RecipePickerFilterField?.GetValue(picker) is not ERecipeType filter ||
            GameMain.iconSet == null ||
            LDB.recipes?.dataArray == null)
        {
            return;
        }

        var entries = new List<PickerLayoutEntry<RecipeProto>>();
        var seenRecipes = new HashSet<int>();
        foreach (var recipe in LDB.recipes.dataArray)
        {
            if (!ShouldShowRecipe(recipe, currentType, filter))
            {
                continue;
            }

            if (activeRecipeRequest?.Filter != null && !activeRecipeRequest.Filter(recipe))
            {
                continue;
            }

            if (!RecipeTypeRuntime.CanCurrentAssemblerUseRecipe(recipe.ID))
            {
                continue;
            }

            if (!seenRecipes.Add(recipe.ID))
            {
                continue;
            }

            entries.Add(new PickerLayoutEntry<RecipeProto>(recipe.GridIndex, GameMain.iconSet.recipeIconIndex[recipe.ID], recipe));
        }

        var baseline = MeasureRecipeGridMetrics(GetOrCaptureBaselineMetrics(picker, picker.iconImage, DefaultRecipePickerColumns, DefaultRecipePickerRows, LargePickerGridSize));
        var layout = PickerLayoutPlanner.Plan(entries, baseline.Columns, baseline.Rows);
        EnsureRecipePickerCapacity(picker, layout.Metrics.Capacity, ref indexArray, ref protoArray);
        ClearPickerArrays(indexArray, protoArray);
        FillProtoLayout(layout, indexArray, protoArray);
        ApplyRecipePickerLayout(picker, layout.Metrics);
    }

    public static void RefreshSignalPicker(UISignalPicker picker)
    {
        if (picker == null ||
            SignalPickerIndexArrayField?.GetValue(picker) is not uint[] indexArray ||
            SignalPickerSignalArrayField?.GetValue(picker) is not int[] signalArray ||
            SignalPickerCurrentTypeField?.GetValue(picker) is not int currentType ||
            GameMain.iconSet == null ||
            LDB.items?.dataArray == null)
        {
            return;
        }

        var itemPage = GetSignalPickerItemPage(currentType);
        if (itemPage <= 0)
        {
            var metrics = MeasureSignalGridMetrics(GetOrCaptureBaselineMetrics(picker, picker.iconImage, DefaultSignalPickerColumns, DefaultSignalPickerRows, LargePickerGridSize));
            EnsureSignalPickerCapacity(picker, metrics.Capacity, ref indexArray, ref signalArray);
            ApplySignalRequestFilter(indexArray, signalArray);
            ApplySignalPickerLayout(picker, metrics);
            return;
        }

        var entries = new List<PickerLayoutEntry<int>>();
        var seenSignals = new HashSet<int>();
        foreach (var item in LDB.items.dataArray)
        {
            if (!ShouldShowSignalItem(item, itemPage))
            {
                continue;
            }

            var signalId = SignalProtoSet.SignalId(ESignalType.Item, item.ID);
            if (activeSignalRequest?.Filter != null && !activeSignalRequest.Filter(signalId))
            {
                continue;
            }

            if (!seenSignals.Add(signalId))
            {
                continue;
            }

            entries.Add(new PickerLayoutEntry<int>(item.GridIndex, GameMain.iconSet.signalIconIndex[signalId], signalId));
        }

        var baseline = MeasureSignalGridMetrics(GetOrCaptureBaselineMetrics(picker, picker.iconImage, DefaultSignalPickerColumns, DefaultSignalPickerRows, LargePickerGridSize));
        var layout = PickerLayoutPlanner.Plan(entries, baseline.Columns, baseline.Rows);
        EnsureSignalPickerCapacity(picker, layout.Metrics.Capacity, ref indexArray, ref signalArray);
        ClearPickerArrays(indexArray, signalArray);
        FillSignalLayout(layout, indexArray, signalArray);
        ApplySignalPickerLayout(picker, layout.Metrics);
    }

    public static void RefreshSignalTagPicker(UISignalTagPicker picker)
    {
        if (picker == null ||
            SignalTagPickerIndexArrayField?.GetValue(picker) is not uint[] indexArray ||
            SignalTagPickerSignalArrayField?.GetValue(picker) is not int[] signalArray ||
            SignalTagPickerCurrentTypeField?.GetValue(picker) is not { } currentTypeValue ||
            GameMain.iconSet == null ||
            LDB.items?.dataArray == null)
        {
            return;
        }

        var itemPage = GetSignalTagPickerItemPage(Convert.ToInt32(currentTypeValue));
        if (itemPage <= 0)
        {
            var metrics = MeasureSignalGridMetrics(GetOrCaptureBaselineMetrics(picker, picker.iconImage, DefaultSignalPickerColumns, DefaultSignalPickerRows, TagPickerGridSize));
            EnsureSignalTagPickerCapacity(picker, metrics.Capacity, ref indexArray, ref signalArray);
            ApplySignalTagPickerLayout(picker, metrics);
            return;
        }

        var showUnlock = SignalTagPickerShowUnlockField?.GetValue(null) is true;
        var entries = new List<PickerLayoutEntry<int>>();
        var seenSignals = new HashSet<int>();
        foreach (var item in LDB.items.dataArray)
        {
            if (!ShouldShowSignalTagItem(item, itemPage, showUnlock))
            {
                continue;
            }

            var signalId = SignalProtoSet.SignalId(ESignalType.Item, item.ID);
            if (!seenSignals.Add(signalId))
            {
                continue;
            }

            entries.Add(new PickerLayoutEntry<int>(item.GridIndex, GameMain.iconSet.signalIconIndex[signalId], signalId));
        }

        var baseline = MeasureSignalGridMetrics(GetOrCaptureBaselineMetrics(picker, picker.iconImage, DefaultSignalPickerColumns, DefaultSignalPickerRows, TagPickerGridSize));
        var layout = PickerLayoutPlanner.Plan(entries, baseline.Columns, baseline.Rows);
        EnsureSignalTagPickerCapacity(picker, layout.Metrics.Capacity, ref indexArray, ref signalArray);
        ClearPickerArrays(indexArray, signalArray);
        FillSignalLayout(layout, indexArray, signalArray);
        ApplySignalTagPickerLayout(picker, layout.Metrics);
    }

    public static void ClearActiveRequest(PickerKind kind)
    {
        switch (kind)
        {
            case PickerKind.Item:
                activeItemRequest = null;
                break;
            case PickerKind.Recipe:
                activeRecipeRequest = null;
                break;
            case PickerKind.Signal:
                activeSignalRequest = null;
                break;
        }
    }

    private static void ApplySignalRequestFilter(uint[] indexArray, int[] signalArray)
    {
        if (activeSignalRequest?.Filter == null)
        {
            return;
        }

        for (var i = 0; i < signalArray.Length; i++)
        {
            var signalId = signalArray[i];
            if (signalId != 0 && !activeSignalRequest.Filter(signalId))
            {
                signalArray[i] = 0;
                if (i < indexArray.Length)
                {
                    indexArray[i] = 0;
                }
            }
        }
    }

    private static bool ShouldShowItem(ItemProto? item, int currentPage)
    {
        if (item == null || !PickerLayoutPlanner.IsOnPage(item.GridIndex, currentPage))
        {
            return false;
        }

        if (UIItemPicker.showAll)
        {
            return true;
        }

        return GameMain.history != null && GameMain.history.ItemUnlocked(item.ID);
    }

    private static bool ShouldShowRecipe(RecipeProto? recipe, int currentPage, ERecipeType filter)
    {
        if (recipe == null || !PickerLayoutPlanner.IsOnPage(recipe.GridIndex, currentPage))
        {
            return false;
        }

        if (filter != ERecipeType.None && filter != recipe.Type)
        {
            return false;
        }

        if (activeRecipeRequest is { ShowAll: true } or { ShowLocked: true })
        {
            return true;
        }

        return GameMain.history != null && GameMain.history.RecipeUnlocked(recipe.ID);
    }

    private static bool ShouldShowSignalItem(ItemProto? item, int currentPage)
    {
        return item != null && PickerLayoutPlanner.IsOnPage(item.GridIndex, currentPage);
    }

    private static bool ShouldShowSignalTagItem(ItemProto? item, int currentPage, bool showUnlock)
    {
        if (item == null || !PickerLayoutPlanner.IsOnPage(item.GridIndex, currentPage))
        {
            return false;
        }

        return showUnlock || GameMain.history == null || GameMain.history.ItemUnlocked(item.ID);
    }

    private static int GetSignalPickerItemPage(int currentType)
    {
        if (currentType == 2)
        {
            return 1;
        }

        if (currentType == 3)
        {
            return 2;
        }

        return currentType > 8 ? currentType - 6 : 0;
    }

    private static int GetSignalTagPickerItemPage(int currentType)
    {
        if (currentType == 3)
        {
            return 1;
        }

        if (currentType == 4)
        {
            return 2;
        }

        return currentType > 8 ? currentType - 6 : 0;
    }

    public static void ApplyItemPickerLayout(UIItemPicker picker)
    {
        ApplyItemPickerLayout(picker, GetRuntimeMetrics(picker, picker?.iconImage, DefaultItemPickerColumns, DefaultItemPickerRows, LargePickerGridSize));
    }

    public static void ApplyRecipePickerLayout(UIRecipePicker picker)
    {
        ApplyRecipePickerLayout(picker, GetRuntimeMetrics(picker, picker?.iconImage, DefaultRecipePickerColumns, DefaultRecipePickerRows, LargePickerGridSize));
    }

    public static void ApplySignalPickerLayout(UISignalPicker picker)
    {
        ApplySignalPickerLayout(picker, GetRuntimeMetrics(picker, picker?.iconImage, DefaultSignalPickerColumns, DefaultSignalPickerRows, LargePickerGridSize));
    }

    public static void ApplySignalTagPickerLayout(UISignalTagPicker picker)
    {
        ApplySignalTagPickerLayout(picker, GetRuntimeMetrics(picker, picker?.iconImage, DefaultSignalPickerColumns, DefaultSignalPickerRows, TagPickerGridSize));
    }

    public static bool TestItemMouseIndex(UIItemPicker picker)
    {
        return TestMouseIndex(
            picker,
            picker.iconImage,
            picker.selImage,
            ItemPickerMouseInBoxField,
            ItemPickerHoveredIndexField,
            ItemPickerProtoArrayField,
            DefaultItemPickerColumns,
            DefaultItemPickerRows,
            LargePickerGridSize,
            -1f);
    }

    public static bool TestRecipeMouseIndex(UIRecipePicker picker)
    {
        return TestMouseIndex(
            picker,
            picker.iconImage,
            picker.selImage,
            RecipePickerMouseInBoxField,
            RecipePickerHoveredIndexField,
            RecipePickerProtoArrayField,
            DefaultRecipePickerColumns,
            DefaultRecipePickerRows,
            LargePickerGridSize,
            -1f);
    }

    public static bool TestSignalMouseIndex(UISignalPicker picker)
    {
        return TestMouseIndex(
            picker,
            picker.iconImage,
            picker.selImage,
            SignalPickerMouseInBoxField,
            SignalPickerHoveredIndexField,
            SignalPickerSignalArrayField,
            DefaultSignalPickerColumns,
            DefaultSignalPickerRows,
            LargePickerGridSize,
            -1f);
    }

    public static bool TestSignalTagMouseIndex(UISignalTagPicker picker)
    {
        return TestMouseIndex(
            picker,
            picker.iconImage,
            picker.selImage,
            SignalTagPickerMouseInBoxField,
            SignalTagPickerHoveredIndexField,
            SignalTagPickerSignalArrayField,
            DefaultSignalPickerColumns,
            DefaultSignalPickerRows,
            TagPickerGridSize,
            0f);
    }

    public static int GetRuntimeColumnCount(object picker)
    {
        return picker != null && LayoutMetrics.TryGetValue(picker, out var metrics) ? metrics.Columns : DefaultSignalPickerColumns;
    }

    public static int GetRuntimeRowCount(object picker)
    {
        return picker != null && LayoutMetrics.TryGetValue(picker, out var metrics) ? metrics.Rows : DefaultSignalPickerRows;
    }

    public static IEnumerable<CodeInstruction> ReplaceUpdateColumnConstants(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (IsIntConstant(instruction, DefaultSignalPickerColumns))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, GetRuntimeColumnCountMethod);
                continue;
            }

            yield return instruction;
        }
    }

    public static IEnumerable<CodeInstruction> ReplaceSignalTagUpdateGridConstants(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (IsIntConstant(instruction, DefaultSignalPickerColumns))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, GetRuntimeColumnCountMethod);
                continue;
            }

            if (IsIntConstant(instruction, DefaultSignalPickerRows))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, GetRuntimeRowCountMethod);
                continue;
            }

            yield return instruction;
        }
    }

    private static void EnsureItemPickerCapacity(UIItemPicker picker, int requiredCapacity, ref uint[] indexArray, ref ItemProto[] protoArray)
    {
        indexArray = EnsureArrayCapacity(picker, ItemPickerIndexArrayField, indexArray, requiredCapacity);
        protoArray = EnsureArrayCapacity(picker, ItemPickerProtoArrayField, protoArray, requiredCapacity);
        EnsureIndexBuffer(picker, ItemPickerIndexBufferField, indexArray.Length);
    }

    private static void EnsureRecipePickerCapacity(UIRecipePicker picker, int requiredCapacity, ref uint[] indexArray, ref RecipeProto[] protoArray)
    {
        indexArray = EnsureArrayCapacity(picker, RecipePickerIndexArrayField, indexArray, requiredCapacity);
        protoArray = EnsureArrayCapacity(picker, RecipePickerProtoArrayField, protoArray, requiredCapacity);
        EnsureIndexBuffer(picker, RecipePickerIndexBufferField, indexArray.Length);
    }

    private static void EnsureSignalPickerCapacity(UISignalPicker picker, int requiredCapacity, ref uint[] indexArray, ref int[] signalArray)
    {
        indexArray = EnsureArrayCapacity(picker, SignalPickerIndexArrayField, indexArray, requiredCapacity);
        signalArray = EnsureArrayCapacity(picker, SignalPickerSignalArrayField, signalArray, requiredCapacity);
        EnsureIndexBuffer(picker, SignalPickerIndexBufferField, indexArray.Length);
    }

    private static void EnsureSignalTagPickerCapacity(UISignalTagPicker picker, int requiredCapacity, ref uint[] indexArray, ref int[] signalArray)
    {
        indexArray = EnsureArrayCapacity(picker, SignalTagPickerIndexArrayField, indexArray, requiredCapacity);
        signalArray = EnsureArrayCapacity(picker, SignalTagPickerSignalArrayField, signalArray, requiredCapacity);
        EnsureIndexBuffer(picker, SignalTagPickerIndexBufferField, indexArray.Length);
    }

    private static T[] EnsureArrayCapacity<T>(object owner, FieldInfo? field, T[] current, int requiredCapacity)
    {
        if (current.Length >= requiredCapacity)
        {
            return current;
        }

        var next = new T[requiredCapacity];
        Array.Copy(current, next, current.Length);
        field?.SetValue(owner, next);
        return next;
    }

    private static void EnsureIndexBuffer(object owner, FieldInfo? field, int requiredCapacity)
    {
        var current = field?.GetValue(owner) as ComputeBuffer;
        if (current != null && current.count >= requiredCapacity)
        {
            return;
        }

        try
        {
            current?.Release();
        }
        catch (Exception ex)
        {
            DspCore.Logger?.LogWarning($"Failed to release old picker index buffer: {ex.Message}");
        }

        field?.SetValue(owner, new ComputeBuffer(requiredCapacity, 4));
    }

    private static void ClearPickerArrays<TValue>(uint[] indexArray, TValue[] valueArray)
    {
        Array.Clear(indexArray, 0, indexArray.Length);
        Array.Clear(valueArray, 0, valueArray.Length);
    }

    private static void FillProtoLayout<TProto>(PickerLayoutResult<TProto> layout, uint[] indexArray, TProto[] protoArray)
        where TProto : class
    {
        foreach (var entry in layout.Entries)
        {
            if (entry.Slot < 0 || entry.Slot >= indexArray.Length || entry.Slot >= protoArray.Length)
            {
                continue;
            }

            indexArray[entry.Slot] = entry.IconIndex;
            protoArray[entry.Slot] = entry.Value;
        }
    }

    private static void FillSignalLayout(PickerLayoutResult<int> layout, uint[] indexArray, int[] signalArray)
    {
        foreach (var entry in layout.Entries)
        {
            if (entry.Slot < 0 || entry.Slot >= indexArray.Length || entry.Slot >= signalArray.Length)
            {
                continue;
            }

            indexArray[entry.Slot] = entry.IconIndex;
            signalArray[entry.Slot] = entry.Value;
        }
    }

    private static PickerGridMetrics ResolveBaselineMetrics(RawImage? iconImage, int fallbackColumns, int fallbackRows, float gridSize)
    {
        if (iconImage == null)
        {
            return new PickerGridMetrics(fallbackColumns, fallbackRows);
        }

        var rectTransform = iconImage.rectTransform;
        var size = rectTransform.rect.size;
        if (size.x <= 1f || size.y <= 1f)
        {
            size = rectTransform.sizeDelta;
        }

        var columns = size.x > 1f ? Math.Max(1, Mathf.RoundToInt(size.x / gridSize)) : fallbackColumns;
        var rows = size.y > 1f ? Math.Max(1, Mathf.RoundToInt(size.y / gridSize)) : fallbackRows;
        return new PickerGridMetrics(columns, rows);
    }

    private static PickerGridMetrics GetOrCaptureBaselineMetrics(object picker, RawImage? iconImage, int fallbackColumns, int fallbackRows, float gridSize)
    {
        if (picker != null && BaselineMetrics.TryGetValue(picker, out var metrics))
        {
            return metrics;
        }

        metrics = ResolveBaselineMetrics(iconImage, fallbackColumns, fallbackRows, gridSize);
        if (picker != null)
        {
            BaselineMetrics[picker] = metrics;
        }

        return metrics;
    }

    private static PickerGridMetrics MeasureItemGridMetrics(PickerGridMetrics baseline)
    {
        var columns = baseline.Columns;
        var rows = baseline.Rows;
        if (LDB.items?.dataArray != null)
        {
            foreach (var item in LDB.items.dataArray)
            {
                IncludeGridIndex(item?.GridIndex ?? 0, ref columns, ref rows);
            }
        }

        return new PickerGridMetrics(columns, rows);
    }

    private static PickerGridMetrics MeasureRecipeGridMetrics(PickerGridMetrics baseline)
    {
        var columns = baseline.Columns;
        var rows = baseline.Rows;
        if (LDB.recipes?.dataArray != null)
        {
            foreach (var recipe in LDB.recipes.dataArray)
            {
                IncludeGridIndex(recipe?.GridIndex ?? 0, ref columns, ref rows);
            }
        }

        return new PickerGridMetrics(columns, rows);
    }

    private static PickerGridMetrics MeasureSignalGridMetrics(PickerGridMetrics baseline)
    {
        var columns = baseline.Columns;
        var rows = baseline.Rows;
        if (LDB.items?.dataArray != null)
        {
            foreach (var item in LDB.items.dataArray)
            {
                IncludeGridIndex(item?.GridIndex ?? 0, ref columns, ref rows);
            }
        }

        if (LDB.signals?.dataArray != null)
        {
            foreach (var signal in LDB.signals.dataArray)
            {
                IncludeGridIndex(signal?.GridIndex ?? 0, ref columns, ref rows);
            }
        }

        return new PickerGridMetrics(columns, rows);
    }

    private static void IncludeGridIndex(int gridIndex, ref int columns, ref int rows)
    {
        if (!PickerLayoutPlanner.TryGetCell(gridIndex, out var row, out var column))
        {
            return;
        }

        rows = Math.Max(rows, row + 1);
        columns = Math.Max(columns, column + 1);
    }

    private static PickerGridMetrics GetRuntimeMetrics(object picker, RawImage? iconImage, int fallbackColumns, int fallbackRows, float gridSize)
    {
        if (picker != null && LayoutMetrics.TryGetValue(picker, out var metrics))
        {
            return metrics;
        }

        return ResolveBaselineMetrics(iconImage, fallbackColumns, fallbackRows, gridSize);
    }

    private static void ApplyItemPickerLayout(UIItemPicker picker, PickerGridMetrics metrics)
    {
        ApplyGridLayout(picker, picker.iconImage, picker.pickerTrans, ItemPickerIndexBufferField, ItemPickerIconMatField, metrics, LargePickerGridSize, 3f, 1.15f, false);
    }

    private static void ApplyRecipePickerLayout(UIRecipePicker picker, PickerGridMetrics metrics)
    {
        ApplyGridLayout(picker, picker.iconImage, picker.pickerTrans, RecipePickerIndexBufferField, RecipePickerIconMatField, metrics, LargePickerGridSize, 3f, 1.15f, false);
    }

    private static void ApplySignalPickerLayout(UISignalPicker picker, PickerGridMetrics metrics)
    {
        ApplyGridLayout(picker, picker.iconImage, picker.pickerTrans, SignalPickerIndexBufferField, SignalPickerIconMatField, metrics, LargePickerGridSize, 3f, 1.15f, false);
    }

    private static void ApplySignalTagPickerLayout(UISignalTagPicker picker, PickerGridMetrics metrics)
    {
        ApplyGridLayout(picker, picker.iconImage, picker.pickerTrans, SignalTagPickerIndexBufferField, SignalTagPickerIconMatField, metrics, TagPickerGridSize, 1f, 1.0833334f, true);
    }

    private static void ApplyGridLayout(
        object picker,
        RawImage iconImage,
        RectTransform pickerTrans,
        FieldInfo? indexBufferField,
        FieldInfo? iconMatField,
        PickerGridMetrics metrics,
        float gridSize,
        float padding,
        float iconScale,
        bool setMipmap)
    {
        if (picker == null || iconImage == null)
        {
            return;
        }

        LayoutMetrics[picker] = metrics;
        ResizeGrid(picker, iconImage.rectTransform, pickerTrans, metrics, gridSize);

        if (iconMatField?.GetValue(picker) is not Material iconMat)
        {
            return;
        }

        if (setMipmap)
        {
            iconMat.SetFloat("_MipmapLevelPlusOne", 2f);
        }

        if (indexBufferField?.GetValue(picker) is ComputeBuffer indexBuffer)
        {
            iconMat.SetBuffer("_IndexBuffer", indexBuffer);
        }

        var rectPadding = padding / gridSize;
        iconMat.SetVector("_Grid", new Vector4(metrics.Columns, metrics.Rows, 0.04f, 0.04f));
        iconMat.SetVector("_Rect", new Vector4(rectPadding, rectPadding, iconScale, iconScale));
    }

    private static void ResizeGrid(object picker, RectTransform iconRect, RectTransform pickerTrans, PickerGridMetrics metrics, float gridSize)
    {
        var targetSize = new Vector2(metrics.Columns * gridSize, metrics.Rows * gridSize);
        var currentSize = iconRect.sizeDelta;
        if (currentSize.x <= 1f || currentSize.y <= 1f)
        {
            currentSize = iconRect.rect.size;
        }

        iconRect.sizeDelta = targetSize;
        if (picker is Component component)
        {
            var content = component.transform.Find("content") as RectTransform;
            if (content != null)
            {
                content.sizeDelta = targetSize;
            }
        }

        if (pickerTrans == null)
        {
            return;
        }

        var deltaX = Math.Max(0f, targetSize.x - Math.Max(1f, currentSize.x));
        var deltaY = Math.Max(0f, targetSize.y - Math.Max(1f, currentSize.y));
        if (deltaX <= 0f && deltaY <= 0f)
        {
            return;
        }

        pickerTrans.sizeDelta = new Vector2(pickerTrans.sizeDelta.x + deltaX, pickerTrans.sizeDelta.y + deltaY);
    }

    private static bool TestMouseIndex(
        object picker,
        RawImage iconImage,
        Image selImage,
        FieldInfo? mouseInBoxField,
        FieldInfo? hoveredIndexField,
        FieldInfo? valueArrayField,
        int fallbackColumns,
        int fallbackRows,
        float gridSize,
        float selectionXOffset)
    {
        var hoveredIndex = -1;
        var metrics = GetRuntimeMetrics(picker, iconImage, fallbackColumns, fallbackRows, gridSize);
        if (mouseInBoxField?.GetValue(picker) is true &&
            UIRoot.ScreenPointIntoRect(Input.mousePosition, iconImage.rectTransform, out var rectPoint))
        {
            var column = Mathf.FloorToInt(rectPoint.x / gridSize);
            var row = Mathf.FloorToInt((0f - rectPoint.y) / gridSize);
            if (column >= 0 && row >= 0 && column < metrics.Columns && row < metrics.Rows)
            {
                hoveredIndex = column + row * metrics.Columns;
            }
        }

        if (!SlotHasValue(valueArrayField?.GetValue(picker), hoveredIndex))
        {
            hoveredIndex = -1;
        }

        hoveredIndexField?.SetValue(picker, hoveredIndex);
        if (hoveredIndex >= 0)
        {
            var column = hoveredIndex % metrics.Columns;
            var row = hoveredIndex / metrics.Columns;
            selImage.rectTransform.anchoredPosition = new Vector2(column * gridSize + selectionXOffset, -row * gridSize + 1f);
            selImage.gameObject.SetActive(true);
        }
        else
        {
            selImage.rectTransform.anchoredPosition = new Vector2(-1f, 1f);
            selImage.gameObject.SetActive(false);
        }

        return false;
    }

    private static bool SlotHasValue(object? valueArray, int slot)
    {
        if (slot < 0)
        {
            return false;
        }

        return valueArray switch
        {
            ItemProto[] items => slot < items.Length && items[slot] != null,
            RecipeProto[] recipes => slot < recipes.Length && recipes[slot] != null,
            int[] signals => slot < signals.Length && signals[slot] != 0,
            _ => false
        };
    }

    private static bool IsIntConstant(CodeInstruction instruction, int value)
    {
        if (instruction.opcode == OpCodes.Ldc_I4 && instruction.operand is int intValue)
        {
            return intValue == value;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte sbyteValue)
        {
            return sbyteValue == value;
        }

        if (value == 8 && instruction.opcode == OpCodes.Ldc_I4_8)
        {
            return true;
        }

        return false;
    }
}

internal static class PickerRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIItemPicker), "RefreshIcons")]
    private static void ItemPickerRefresh(UIItemPicker __instance)
    {
        PickerRuntime.RefreshItemPicker(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIItemPicker), "SetMaterialProps")]
    private static void ItemPickerSetMaterialProps(UIItemPicker __instance)
    {
        PickerRuntime.ApplyItemPickerLayout(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIItemPicker), "TestMouseIndex")]
    private static bool ItemPickerTestMouseIndex(UIItemPicker __instance)
    {
        return PickerRuntime.TestItemMouseIndex(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIItemPicker), "_OnUpdate")]
    private static IEnumerable<CodeInstruction> ItemPickerUpdate(IEnumerable<CodeInstruction> instructions)
    {
        return PickerRuntime.ReplaceUpdateColumnConstants(instructions);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIItemPicker), "_OnClose")]
    private static void ItemPickerClose()
    {
        UIItemPicker.showAll = false;
        PickerRuntime.ClearActiveRequest(PickerKind.Item);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIRecipePicker), "RefreshIcons")]
    private static void RecipePickerRefresh(UIRecipePicker __instance)
    {
        PickerRuntime.RefreshRecipePicker(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIRecipePicker), "SetMaterialProps")]
    private static void RecipePickerSetMaterialProps(UIRecipePicker __instance)
    {
        PickerRuntime.ApplyRecipePickerLayout(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIRecipePicker), "TestMouseIndex")]
    private static bool RecipePickerTestMouseIndex(UIRecipePicker __instance)
    {
        return PickerRuntime.TestRecipeMouseIndex(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIRecipePicker), "_OnUpdate")]
    private static IEnumerable<CodeInstruction> RecipePickerUpdate(IEnumerable<CodeInstruction> instructions)
    {
        return PickerRuntime.ReplaceUpdateColumnConstants(instructions);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UIRecipePicker), "_OnClose")]
    private static void RecipePickerClose()
    {
        PickerRuntime.ClearActiveRequest(PickerKind.Recipe);
        RecipeTypeRuntime.ClearCurrentAssembler();
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalPicker), "RefreshIcons")]
    private static void SignalPickerRefresh(UISignalPicker __instance)
    {
        PickerRuntime.RefreshSignalPicker(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalPicker), "SetMaterialProps")]
    private static void SignalPickerSetMaterialProps(UISignalPicker __instance)
    {
        PickerRuntime.ApplySignalPickerLayout(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalPicker), "TestMouseIndex")]
    private static bool SignalPickerTestMouseIndex(UISignalPicker __instance)
    {
        return PickerRuntime.TestSignalMouseIndex(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalPicker), "_OnUpdate")]
    private static IEnumerable<CodeInstruction> SignalPickerUpdate(IEnumerable<CodeInstruction> instructions)
    {
        return PickerRuntime.ReplaceUpdateColumnConstants(instructions);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalPicker), "_OnClose")]
    private static void SignalPickerClose()
    {
        PickerRuntime.ClearActiveRequest(PickerKind.Signal);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalTagPicker), "RefreshIcons")]
    private static void SignalTagPickerRefresh(UISignalTagPicker __instance)
    {
        PickerRuntime.RefreshSignalTagPicker(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalTagPicker), "SetMaterialProps")]
    private static void SignalTagPickerSetMaterialProps(UISignalTagPicker __instance)
    {
        PickerRuntime.ApplySignalTagPickerLayout(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalTagPicker), "TestMouseIndex")]
    private static bool SignalTagPickerTestMouseIndex(UISignalTagPicker __instance)
    {
        return PickerRuntime.TestSignalTagMouseIndex(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(UISignalTagPicker), "_OnUpdate")]
    private static IEnumerable<CodeInstruction> SignalTagPickerUpdate(IEnumerable<CodeInstruction> instructions)
    {
        return PickerRuntime.ReplaceSignalTagUpdateGridConstants(instructions);
    }
}
