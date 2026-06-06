using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DSPCore;

internal static class TabRuntime
{
    private static readonly Dictionary<object, List<TabButtonBinding>> Bindings = new();

    public static void AttachToItemPicker(UIItemPicker picker)
    {
        Attach(picker, picker.pickerTrans, picker.typeButton2, type => InvokeTypeClick(picker, type), -54f, -75f);
    }

    public static void AttachToRecipePicker(UIRecipePicker picker)
    {
        Attach(picker, picker.pickerTrans, picker.typeButton2, type => InvokeTypeClick(picker, type), -54f, -75f);
    }

    public static void AttachToReplicator(UIReplicatorWindow window)
    {
        Attach(window, window.recipeGroup, window.typeButton2, type => InvokeTypeClick(window, type), -95f, 50f);
    }

    public static void Select(object owner, int type)
    {
        if (!Bindings.TryGetValue(owner, out var bindings))
        {
            return;
        }

        foreach (var binding in bindings)
        {
            binding.Button.button.interactable = type != binding.Type;
        }
    }

    private static void Attach(object owner, Transform parent, UIButton template, Action<int> onClick, float baseX, float y)
    {
        if (Bindings.ContainsKey(owner) || parent == null || template == null)
        {
            return;
        }

        var bindings = new List<TabButtonBinding>();
        var displayIndex = 3;
        foreach (var registration in DspCore.Tabs.GetRegistrations())
        {
            var tab = registration.Descriptor;
            var type = registration.Slot.Value;
            var buttonObject = UnityEngine.Object.Instantiate(template.gameObject, parent, false);
            buttonObject.name = "dspcore-tab-" + tab.Id;
            buttonObject.transform.localPosition = new Vector3(baseX + 70f * (displayIndex - 1), y, 0f);
            displayIndex++;
            var uiButton = buttonObject.GetComponent<UIButton>();
            if (uiButton == null)
            {
                UnityEngine.Object.Destroy(buttonObject);
                continue;
            }

            var button = buttonObject.GetComponent<Button>() ?? buttonObject.AddComponent<Button>();
            button.onClick.RemoveAllListeners();
            uiButton.button = button;
            button.onClick.AddListener((UnityAction)(() => onClick(type)));

            var label = buttonObject.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = tab.Title.Translate();
            }

            if (DspCore.Icons.TryGet(tab.IconId, out var icon))
            {
                var image = buttonObject.GetComponentInChildren<Image>();
                var sprite = IconRuntime.ResolveSprite(icon);
                if (image != null && sprite != null)
                {
                    image.sprite = sprite;
                }
            }

            bindings.Add(new TabButtonBinding(type, uiButton));
        }

        Bindings[owner] = bindings;
    }

    private static void InvokeTypeClick(object owner, int type)
    {
        AccessTools.Method(owner.GetType(), "OnTypeButtonClick")?.Invoke(owner, new object[] { type });
    }

    private readonly struct TabButtonBinding
    {
        public TabButtonBinding(int type, UIButton button)
        {
            Type = type;
            Button = button;
        }

        public int Type { get; }

        public UIButton Button { get; }
    }
}

internal static class TabRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIItemPicker), "_OnCreate")]
    private static void ItemPickerCreate(UIItemPicker __instance)
    {
        TabRuntime.AttachToItemPicker(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIItemPicker), "OnTypeButtonClick")]
    private static void ItemPickerType(UIItemPicker __instance, int type)
    {
        TabRuntime.Select(__instance, type);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIRecipePicker), "_OnCreate")]
    private static void RecipePickerCreate(UIRecipePicker __instance)
    {
        TabRuntime.AttachToRecipePicker(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIRecipePicker), "OnTypeButtonClick")]
    private static void RecipePickerType(UIRecipePicker __instance, int type)
    {
        TabRuntime.Select(__instance, type);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIReplicatorWindow), "_OnCreate")]
    private static void ReplicatorCreate(UIReplicatorWindow __instance)
    {
        TabRuntime.AttachToReplicator(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIReplicatorWindow), "OnTypeButtonClick")]
    private static void ReplicatorType(UIReplicatorWindow __instance, int type)
    {
        TabRuntime.Select(__instance, type);
    }
}
