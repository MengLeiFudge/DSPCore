using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace DSPCore;

internal static class PickerRuntime
{
    private const int ItemPickerColumns = 14;
    private const int ItemPickerRows = 8;
    private const int RecipePickerColumns = 14;
    private const int RecipePickerRows = 8;
    private const int SignalPickerColumns = 14;
    private const int SignalPickerRows = 10;

    private static readonly FieldInfo? ItemPickerIndexArrayField = AccessTools.Field(typeof(UIItemPicker), "indexArray");
    private static readonly FieldInfo? ItemPickerProtoArrayField = AccessTools.Field(typeof(UIItemPicker), "protoArray");
    private static readonly FieldInfo? ItemPickerCurrentTypeField = AccessTools.Field(typeof(UIItemPicker), "currentType");
    private static readonly FieldInfo? RecipePickerIndexArrayField = AccessTools.Field(typeof(UIRecipePicker), "indexArray");
    private static readonly FieldInfo? RecipePickerProtoArrayField = AccessTools.Field(typeof(UIRecipePicker), "protoArray");
    private static readonly FieldInfo? RecipePickerCurrentTypeField = AccessTools.Field(typeof(UIRecipePicker), "currentType");
    private static readonly FieldInfo? RecipePickerFilterField = AccessTools.Field(typeof(UIRecipePicker), "filter");
    private static readonly FieldInfo? SignalPickerIndexArrayField = AccessTools.Field(typeof(UISignalPicker), "indexArray");
    private static readonly FieldInfo? SignalPickerSignalArrayField = AccessTools.Field(typeof(UISignalPicker), "signalArray");
    private static readonly FieldInfo? SignalPickerCurrentTypeField = AccessTools.Field(typeof(UISignalPicker), "currentType");
    private static readonly FieldInfo? SignalTagPickerIndexArrayField = AccessTools.Field(typeof(UISignalTagPicker), "indexArray");
    private static readonly FieldInfo? SignalTagPickerSignalArrayField = AccessTools.Field(typeof(UISignalTagPicker), "signalArray");
    private static readonly FieldInfo? SignalTagPickerCurrentTypeField = AccessTools.Field(typeof(UISignalTagPicker), "currentType");
    private static readonly FieldInfo? SignalTagPickerShowUnlockField = AccessTools.Field(typeof(UISignalTagPicker), "showUnlock");

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

        Array.Clear(indexArray, 0, indexArray.Length);
        Array.Clear(protoArray, 0, protoArray.Length);

        var visibleSlotCount = Math.Min(ItemPickerColumns * ItemPickerRows, Math.Min(indexArray.Length, protoArray.Length));
        var occupied = new bool[visibleSlotCount];
        foreach (var item in LDB.items.dataArray)
        {
            if (!ShouldShowItem(item, currentType, ItemPickerRows, ItemPickerColumns))
            {
                continue;
            }

            if (activeItemRequest?.Filter != null && !activeItemRequest.Filter(item))
            {
                continue;
            }

            var preferredSlot = GetGridSlot(item.GridIndex, ItemPickerColumns);
            var slot = FindVisibleSlot(occupied, preferredSlot);
            if (slot < 0 || IsItemAlreadyVisible(protoArray, item.ID))
            {
                continue;
            }

            indexArray[slot] = GameMain.iconSet.itemIconIndex[item.ID];
            protoArray[slot] = item;
            occupied[slot] = true;
        }
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

        Array.Clear(indexArray, 0, indexArray.Length);
        Array.Clear(protoArray, 0, protoArray.Length);

        var visibleSlotCount = Math.Min(RecipePickerColumns * RecipePickerRows, Math.Min(indexArray.Length, protoArray.Length));
        var occupied = new bool[visibleSlotCount];
        foreach (var recipe in LDB.recipes.dataArray)
        {
            if (!ShouldShowRecipe(recipe, currentType, filter, RecipePickerRows, RecipePickerColumns))
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

            var preferredSlot = GetGridSlot(recipe.GridIndex, RecipePickerColumns);
            var slot = FindVisibleSlot(occupied, preferredSlot);
            if (slot < 0 || IsRecipeAlreadyVisible(protoArray, recipe.ID))
            {
                continue;
            }

            indexArray[slot] = GameMain.iconSet.recipeIconIndex[recipe.ID];
            protoArray[slot] = recipe;
            occupied[slot] = true;
        }
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
            ApplySignalRequestFilter(indexArray, signalArray);
            return;
        }

        Array.Clear(indexArray, 0, indexArray.Length);
        Array.Clear(signalArray, 0, signalArray.Length);

        var visibleSlotCount = Math.Min(SignalPickerColumns * SignalPickerRows, Math.Min(indexArray.Length, signalArray.Length));
        var occupied = new bool[visibleSlotCount];
        foreach (var item in LDB.items.dataArray)
        {
            if (!ShouldShowSignalItem(item, itemPage, SignalPickerRows, SignalPickerColumns))
            {
                continue;
            }

            var signalId = SignalProtoSet.SignalId(ESignalType.Item, item.ID);
            if (activeSignalRequest?.Filter != null && !activeSignalRequest.Filter(signalId))
            {
                continue;
            }

            var preferredSlot = GetGridSlot(item.GridIndex, SignalPickerColumns);
            var slot = FindVisibleSlot(occupied, preferredSlot);
            if (slot < 0 || IsSignalAlreadyVisible(signalArray, signalId))
            {
                continue;
            }

            indexArray[slot] = GameMain.iconSet.signalIconIndex[signalId];
            signalArray[slot] = signalId;
            occupied[slot] = true;
        }
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
            return;
        }

        var showUnlock = SignalTagPickerShowUnlockField?.GetValue(null) is true;
        Array.Clear(indexArray, 0, indexArray.Length);
        Array.Clear(signalArray, 0, signalArray.Length);

        var visibleSlotCount = Math.Min(SignalPickerColumns * SignalPickerRows, Math.Min(indexArray.Length, signalArray.Length));
        var occupied = new bool[visibleSlotCount];
        foreach (var item in LDB.items.dataArray)
        {
            if (!ShouldShowSignalTagItem(item, itemPage, showUnlock, SignalPickerRows, SignalPickerColumns))
            {
                continue;
            }

            var signalId = SignalProtoSet.SignalId(ESignalType.Item, item.ID);
            var preferredSlot = GetGridSlot(item.GridIndex, SignalPickerColumns);
            var slot = FindVisibleSlot(occupied, preferredSlot);
            if (slot < 0 || IsSignalAlreadyVisible(signalArray, signalId))
            {
                continue;
            }

            indexArray[slot] = GameMain.iconSet.signalIconIndex[signalId];
            signalArray[slot] = signalId;
            occupied[slot] = true;
        }
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

    private static bool ShouldShowItem(ItemProto? item, int currentPage, int rowCount, int columnCount)
    {
        if (item == null || !IsGridVisible(item.GridIndex, currentPage, rowCount, columnCount))
        {
            return false;
        }

        if (UIItemPicker.showAll)
        {
            return true;
        }

        return GameMain.history != null && GameMain.history.ItemUnlocked(item.ID);
    }

    private static bool ShouldShowRecipe(RecipeProto? recipe, int currentPage, ERecipeType filter, int rowCount, int columnCount)
    {
        if (recipe == null || !IsGridVisible(recipe.GridIndex, currentPage, rowCount, columnCount))
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

    private static bool ShouldShowSignalItem(ItemProto? item, int currentPage, int rowCount, int columnCount)
    {
        return item != null && IsGridVisible(item.GridIndex, currentPage, rowCount, columnCount);
    }

    private static bool ShouldShowSignalTagItem(ItemProto? item, int currentPage, bool showUnlock, int rowCount, int columnCount)
    {
        if (item == null || !IsGridVisible(item.GridIndex, currentPage, rowCount, columnCount))
        {
            return false;
        }

        return showUnlock || GameMain.history == null || GameMain.history.ItemUnlocked(item.ID);
    }

    private static bool IsGridVisible(int gridIndex, int currentPage, int rowCount, int columnCount)
    {
        if (gridIndex < 1101)
        {
            return false;
        }

        var page = gridIndex / 1000;
        if (page != currentPage)
        {
            return false;
        }

        var row = GetGridRow(gridIndex, page);
        var column = GetGridColumn(gridIndex);
        return row >= 0 && row < rowCount && column >= 0 && column < columnCount;
    }

    private static int GetGridRow(int gridIndex, int page)
    {
        return (gridIndex - page * 1000) / 100 - 1;
    }

    private static int GetGridColumn(int gridIndex)
    {
        return gridIndex % 100 - 1;
    }

    private static int GetGridSlot(int gridIndex, int columnCount)
    {
        var page = gridIndex / 1000;
        return GetGridRow(gridIndex, page) * columnCount + GetGridColumn(gridIndex);
    }

    private static int FindVisibleSlot(bool[] occupied, int preferredSlot)
    {
        if (preferredSlot >= 0 && preferredSlot < occupied.Length && !occupied[preferredSlot])
        {
            return preferredSlot;
        }

        for (var i = 0; i < occupied.Length; i++)
        {
            if (!occupied[i])
            {
                return i;
            }
        }

        return -1;
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

    private static bool IsItemAlreadyVisible(ItemProto[] protoArray, int itemId)
    {
        foreach (var item in protoArray)
        {
            if (item?.ID == itemId)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsRecipeAlreadyVisible(RecipeProto[] protoArray, int recipeId)
    {
        foreach (var recipe in protoArray)
        {
            if (recipe?.ID == recipeId)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSignalAlreadyVisible(int[] signalArray, int signalId)
    {
        foreach (var current in signalArray)
        {
            if (current == signalId)
            {
                return true;
            }
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
}
