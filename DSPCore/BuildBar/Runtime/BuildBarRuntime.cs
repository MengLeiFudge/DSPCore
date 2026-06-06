using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DSPCore;

internal static class BuildBarRuntime
{
    private static readonly System.Reflection.FieldInfo? ProtosField = AccessTools.Field(typeof(UIBuildMenu), "protos");
    private static readonly System.Reflection.FieldInfo? StaticLoadedField = AccessTools.Field(typeof(UIBuildMenu), "staticLoaded");
    private static readonly System.Reflection.FieldInfo? CurrentCategoryField = AccessTools.Field(typeof(UIBuildMenu), "currentCategory");
    private static readonly System.Reflection.MethodInfo? OnChildButtonClickMethod = AccessTools.Method(typeof(UIBuildMenu), "OnChildButtonClick");
    private static readonly Dictionary<RowButtonKey, UIButton> ExtendedButtons = new();
    private static readonly Dictionary<RowButtonKey, Image> ExtendedIcons = new();
    private static readonly Dictionary<RowButtonKey, Text> ExtendedCounts = new();
    private static UIBuildMenu? currentMenu;

    public static void Initialize()
    {
        DspCore.Saves.Register("DSPCore.BuildBar", new BuildBarSaveHandler(), CoreLoadOrder.Postload);
    }

    public static void Apply()
    {
        if (ProtosField?.GetValue(null) is not ItemProto[,] protos)
        {
            DspCore.Logger?.LogWarning("UIBuildMenu.protos is not accessible; build bar runtime bridge skipped.");
            return;
        }

        foreach (var binding in DspCore.BuildBar.GetEffectiveBindings())
        {
            var slot = binding.Key;
            var itemId = binding.Value;
            if (slot.Row != 1)
            {
                continue;
            }

            if (slot.Tab > 15 || slot.Index > 12)
            {
                DspCore.Logger?.LogWarning($"Build bar slot tab {slot.Tab}, row {slot.Row}, index {slot.Index} is outside vanilla UIBuildMenu bounds.");
                continue;
            }

            var item = LDB.items.Select(itemId);
            if (item == null)
            {
                DspCore.Logger?.LogWarning($"Build bar item {itemId} does not exist in LDB.items.");
                continue;
            }

            item.BuildIndex = slot.Tab * 100 + slot.Index;
            protos[slot.Tab, slot.Index] = item;
            StaticLoadedField?.SetValue(null, true);
            DspCore.Logger?.LogInfo($"Applied build bar slot tab {slot.Tab}, row {slot.Row}, index {slot.Index} -> item {itemId}.");
        }
    }

    public static void EnsureExtendedRows(UIBuildMenu menu)
    {
        currentMenu = menu;
        var maxRow = DspCore.BuildBar.GetEffectiveBindings().Keys.Select(item => item.Row).DefaultIfEmpty(1).Max();
        if (maxRow <= 1)
        {
            return;
        }

        var childGroup = GameObject.Find("UI Root/Overlay Canvas/In Game/Function Panel/Build Menu/child-group");
        var baseButton = GameObject.Find("UI Root/Overlay Canvas/In Game/Function Panel/Build Menu/child-group/button-1");
        if (childGroup == null || baseButton == null)
        {
            return;
        }

        var parent = childGroup.transform;
        var basePosition = baseButton.transform.localPosition;
        for (var row = 2; row <= maxRow; row++)
        {
            for (var index = 1; index <= 12; index++)
            {
                var key = new RowButtonKey(row, index);
                if (ExtendedButtons.ContainsKey(key))
                {
                    continue;
                }

                var buttonObject = UnityEngine.Object.Instantiate(baseButton, parent, false);
                buttonObject.name = $"dspcore-row-{row}-button-{index}";
                buttonObject.transform.localPosition = new Vector3(basePosition.x + (index - 1) * 52f, basePosition.y + (row - 1) * 60f, 0f);
                var button = buttonObject.GetComponent<Button>() ?? buttonObject.AddComponent<Button>();
                button.onClick.RemoveAllListeners();
                var uiButton = buttonObject.GetComponent<UIButton>();
                if (uiButton == null)
                {
                    UnityEngine.Object.Destroy(buttonObject);
                    continue;
                }

                uiButton.button = button;

                var capturedRow = row;
                var capturedIndex = index;
                button.onClick.AddListener((UnityAction)(() => OnExtendedButtonClick(capturedRow, capturedIndex)));

                ExtendedButtons[key] = uiButton;
                if (buttonObject.transform.Find("icon")?.GetComponent<Image>() is { } icon)
                {
                    ExtendedIcons[key] = icon;
                }

                if (buttonObject.transform.Find("count")?.GetComponent<Text>() is { } countText)
                {
                    ExtendedCounts[key] = countText;
                }

                buttonObject.SetActive(false);
            }
        }
    }

    public static void RefreshExtendedRows(UIBuildMenu menu)
    {
        currentMenu = menu;
        EnsureExtendedRows(menu);

        if (ExtendedButtons.Count == 0)
        {
            return;
        }

        var category = GetCurrentCategory(menu);
        var bindings = DspCore.BuildBar.GetEffectiveBindings();
        foreach (var pair in ExtendedButtons)
        {
            var key = pair.Key;
            var uiButton = pair.Value;
            var slot = new BuildBarSlot(category, key.Row, key.Index);
            var visible = bindings.TryGetValue(slot, out var itemId);
            var item = visible ? LDB.items.Select(itemId) : null;
            visible = visible && item != null;

            uiButton.gameObject.SetActive(visible);
            if (!visible || item == null || uiButton.button == null)
            {
                continue;
            }

            var unlocked = GameMain.history != null && GameMain.history.ItemUnlocked(item.ID);
            uiButton.button.interactable = unlocked || IsSandboxInstantItem();
            uiButton.tips.itemId = item.ID;
            uiButton.tips.itemInc = 0;
            uiButton.tips.itemCount = 0;
            uiButton.tips.corner = 8;
            uiButton.tips.delay = 0.2f;
            uiButton.tips.type = UIButton.ItemTipType.Other;

            if (ExtendedIcons.TryGetValue(key, out var icon))
            {
                icon.sprite = item.iconSprite;
            }

            if (ExtendedCounts.TryGetValue(key, out var countText))
            {
                var count = GetItemCount(item.ID);
                countText.text = count > 0 ? count.ToString() : string.Empty;
            }
        }
    }

    private static void OnExtendedButtonClick(int row, int index)
    {
        if (currentMenu == null || ProtosField?.GetValue(null) is not ItemProto[,] protos || OnChildButtonClickMethod == null)
        {
            return;
        }

        var category = GetCurrentCategory(currentMenu);
        if (!IsInVanillaBounds(protos, category, index))
        {
            return;
        }

        if (!DspCore.BuildBar.GetEffectiveBindings().TryGetValue(new BuildBarSlot(category, row, index), out var itemId))
        {
            return;
        }

        var item = LDB.items.Select(itemId);
        if (item == null)
        {
            return;
        }

        var original = protos[category, index];
        try
        {
            protos[category, index] = item;
            OnChildButtonClickMethod.Invoke(currentMenu, new object[] { index });
        }
        finally
        {
            protos[category, index] = original;
        }
    }

    private static int GetCurrentCategory(UIBuildMenu menu)
    {
        return CurrentCategoryField?.GetValue(menu) is int category ? category : 0;
    }

    private static bool IsInVanillaBounds(ItemProto[,] protos, int category, int index)
    {
        return category > 0 &&
            index > 0 &&
            category < protos.GetLength(0) &&
            index < protos.GetLength(1);
    }

    private static bool IsSandboxInstantItem()
    {
        try
        {
            return GameMain.data?.gameDesc?.isSandboxMode == true && UIRoot.instance?.uiGame?.replicator?.isInstantItem == true;
        }
        catch
        {
            return false;
        }
    }

    private static int GetItemCount(int itemId)
    {
        try
        {
            return GameMain.mainPlayer?.package?.GetItemCount(itemId) ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    private readonly struct RowButtonKey : IEquatable<RowButtonKey>
    {
        public RowButtonKey(int row, int index)
        {
            Row = row;
            Index = index;
        }

        public int Row { get; }

        public int Index { get; }

        public bool Equals(RowButtonKey other)
        {
            return Row == other.Row && Index == other.Index;
        }

        public override bool Equals(object? obj)
        {
            return obj is RowButtonKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Row * 397) ^ Index;
        }
    }

    private sealed class BuildBarSaveHandler : ICoreSaveHandler
    {
        public void Export(BinaryWriter writer)
        {
            var overrides = DspCore.BuildBar.GetPlayerOverrides();
            writer.Write(1);
            writer.Write(overrides.Count);
            foreach (var pair in overrides)
            {
                writer.Write(pair.Key.Tab);
                writer.Write(pair.Key.Row);
                writer.Write(pair.Key.Index);
                writer.Write(pair.Value);
            }
        }

        public void Import(BinaryReader reader)
        {
            DspCore.BuildBar.ClearPlayerOverrides();
            var version = reader.ReadInt32();
            if (version > 1)
            {
                DspCore.Logger?.LogWarning($"BuildBar save version {version} is newer than runtime version 1.");
            }

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var slot = new BuildBarSlot(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                var itemId = reader.ReadInt32();
                DspCore.BuildBar.SetPlayerOverride(slot, itemId);
            }

            Apply();
        }

        public void IntoOtherSave()
        {
            DspCore.BuildBar.ClearPlayerOverrides();
            Apply();
        }
    }
}

[HarmonyPatch(typeof(UIBuildMenu), nameof(UIBuildMenu.StaticLoad))]
internal static class BuildBarRuntimePatches
{
    private static void Postfix()
    {
        BuildBarRuntime.Apply();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIBuildMenu), "_OnOpen")]
    private static void OnOpen(UIBuildMenu __instance)
    {
        BuildBarRuntime.EnsureExtendedRows(__instance);
        BuildBarRuntime.RefreshExtendedRows(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIBuildMenu), "_OnUpdate")]
    private static void OnUpdate(UIBuildMenu __instance)
    {
        BuildBarRuntime.RefreshExtendedRows(__instance);
    }
}
