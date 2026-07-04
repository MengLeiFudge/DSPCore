using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace DSPCore;

internal static class KeyBindRuntime
{
    private const int VanillaKeyLimit = 256;
    private const int OverrideKeyCapacity = 512;
    private const string SaveDirectoryName = "DSPCore";
    private const string SaveFileName = "keybinds.dat";
    private static readonly System.Reflection.FieldInfo? KeyEntriesField = AccessTools.Field(typeof(UIOptionWindow), "keyEntries");
    private static readonly Dictionary<string, BuiltinKey> RegisteredKeys = new(StringComparer.Ordinal);
    private static readonly HashSet<string> InvalidKeyWarnings = new(StringComparer.Ordinal);
    private static bool vanillaKeysExpanded;
    private static bool customKeysLoaded;

    public static void Update()
    {
        EnsureRegisteredToVanilla();
        EnsureRuntimeArrays();
        foreach (var descriptor in DspCore.KeyBinds.GetAll())
        {
            if (descriptor.Callback == null || !TryGetCombineKey(descriptor, out var key))
            {
                continue;
            }

            var triggered = descriptor.Action switch
            {
                CoreKeyAction.Press => key.GetKeyDown(),
                CoreKeyAction.Hold => key.GetKey(),
                CoreKeyAction.Release => key.GetKeyUp(),
                _ => false
            };

            if (!triggered)
            {
                continue;
            }

            try
            {
                descriptor.Callback();
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
                DspCore.Logger?.LogError($"Key binding {descriptor.Id} from {descriptor.OwnerModGuid} failed: {ex}");
            }
        }
    }

    public static void EnsureRegisteredToVanilla()
    {
        if (vanillaKeysExpanded && RegisteredKeys.Count == DspCore.KeyBinds.GetAll().Count(item => item.CanOverride))
        {
            return;
        }

        if (DSPGame.key?.builtinKeys == null)
        {
            return;
        }

        var keys = DSPGame.key.builtinKeys.ToList();
        var nextId = Math.Max(VanillaKeyLimit, keys.Select(item => item.id).DefaultIfEmpty(VanillaKeyLimit - 1).Max() + 1);
        foreach (var descriptor in DspCore.KeyBinds.GetAll().Where(item => item.CanOverride))
        {
            var keyName = BuildKeyName(descriptor);
            var descriptorKey = DescriptorKey(descriptor);
            if (RegisteredKeys.ContainsKey(descriptorKey))
            {
                continue;
            }

            var existingKey = keys.FirstOrDefault(item => item.name.Equals(keyName, StringComparison.Ordinal));
            if (existingKey.name != null)
            {
                RegisteredKeys[descriptorKey] = existingKey;
                continue;
            }

            if (!TryParseCombineKey(descriptor.DefaultKey, descriptor.Action, out var defaultKey))
            {
                WarnInvalidKeyOnce(descriptor, descriptor.DefaultKey);
                continue;
            }

            var builtInKey = new BuiltinKey(
                keyName,
                nextId++,
                defaultKey.keyCode,
                defaultKey.modifier,
                defaultKey.action,
                defaultKey.noneKey,
                true,
                descriptor.ConflictGroup);
            keys.Add(builtInKey);
            RegisteredKeys[descriptorKey] = builtInKey;
            DspCore.Resources.RegisterLocalization(new LocalizationEntry("KEY" + keyName, "zhCN", descriptor.DisplayName, descriptor.OwnerModGuid));
            DspCore.Resources.RegisterLocalization(new LocalizationEntry("KEY" + keyName, "enUS", descriptor.DisplayName, descriptor.OwnerModGuid));
        }

        DSPGame.key.builtinKeys = keys.ToArray();
        vanillaKeysExpanded = true;
        EnsureRuntimeArrays();
    }

    public static void EnsureRuntimeArrays()
    {
        EnsureRegisteredToVanilla();
        Resize(ref VFInput.override_keys);
        Resize(ref DSPGame.globalOption.overrideKeys);
        Resize(ref DSPGame.globalOption.overrideKeysChanged);

        LoadCustomKeysOnce();
    }

    public static void SaveCustomKeys()
    {
        EnsureRegisteredToVanilla();
        EnsureRuntimeArrays();
        if (DSPGame.globalOption.overrideKeys == null)
        {
            return;
        }

        var directory = Path.Combine(Paths.ConfigPath, SaveDirectoryName);
        Directory.CreateDirectory(directory);
        using var stream = File.Create(Path.Combine(directory, SaveFileName));
        using var writer = new BinaryWriter(stream);
        writer.Write(1);
        var keys = RegisteredKeys.Values.OrderBy(item => item.id).ToArray();
        writer.Write(keys.Length);
        foreach (var key in keys)
        {
            writer.Write(key.name);
            writer.Write(key.id);
            WriteCombineKey(writer, DSPGame.globalOption.overrideKeys[key.id]);
        }
    }

    public static void EnsureOptionWindowKeyEntries(UIOptionWindow window)
    {
        EnsureRegisteredToVanilla();
        EnsureRuntimeArrays();
        if (!window || DSPGame.key?.builtinKeys == null || window.entryPrefab == null || KeyEntriesField == null)
        {
            return;
        }

        var builtinKeys = DSPGame.key.builtinKeys;
        var existingEntries = KeyEntriesField.GetValue(window) as UIKeyEntry[] ?? Array.Empty<UIKeyEntry>();
        if (existingEntries.Length >= builtinKeys.Length)
        {
            ResizeKeyScrollContent(window, existingEntries.Length);
            return;
        }

        var expandedEntries = new UIKeyEntry[builtinKeys.Length];
        Array.Copy(existingEntries, expandedEntries, existingEntries.Length);
        for (var i = existingEntries.Length; i < builtinKeys.Length; i++)
        {
            expandedEntries[i] = window.CreateKeyEntry(i, builtinKeys[i]);
        }

        KeyEntriesField.SetValue(window, expandedEntries);
        ResizeKeyScrollContent(window, expandedEntries.Length);
    }

    internal static bool TryNormalizeKeyText(string keyText, out string normalized)
    {
        if (!TryParseCombineKey(keyText, CoreKeyAction.Press, out var key))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = key.ToTokenString(0);
        return true;
    }

    internal static bool IsHeld(string id, KeyCode fallback)
    {
        foreach (var descriptor in DspCore.KeyBinds.GetAll())
        {
            if (descriptor.Id.Equals(id, StringComparison.Ordinal) && TryGetCombineKey(descriptor, out var key))
            {
                return key.GetKey();
            }
        }

        return Input.GetKey(fallback);
    }

    private static bool TryGetCombineKey(KeyBindDescriptor descriptor, out CombineKey key)
    {
        if (descriptor.CanOverride && RegisteredKeys.TryGetValue(DescriptorKey(descriptor), out var builtInKey))
        {
            var overrides = VFInput.override_keys ?? DSPGame.globalOption.overrideKeys;
            if (overrides != null && builtInKey.id >= 0 && builtInKey.id < overrides.Length && !overrides[builtInKey.id].IsNull())
            {
                key = overrides[builtInKey.id];
                key.modifierExact = builtInKey.key.modifierExact;
                return true;
            }

            key = builtInKey.key;
            return true;
        }

        return TryParseCombineKey(descriptor.DefaultKey, descriptor.Action, out key);
    }

    private static void ResizeKeyScrollContent(UIOptionWindow window, int keyEntryCount)
    {
        if (window.keyScrollContentRect == null)
        {
            return;
        }

        window.keyScrollContentRect.sizeDelta = new Vector2(window.keyScrollContentRect.sizeDelta.x, 36 * keyEntryCount + 8);
    }

    private static void LoadCustomKeysOnce()
    {
        if (customKeysLoaded || DSPGame.globalOption.overrideKeys == null)
        {
            return;
        }

        customKeysLoaded = true;
        var path = Path.Combine(Paths.ConfigPath, SaveDirectoryName, SaveFileName);
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);
            var version = reader.ReadInt32();
            if (version > 1)
            {
                DspCore.Logger?.LogWarning($"DSPCore key bind save version {version} is newer than runtime version 1.");
            }

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var keyName = reader.ReadString();
                var savedId = reader.ReadInt32();
                var key = ReadCombineKey(reader);
                var builtInKey = RegisteredKeys.Values.FirstOrDefault(item => item.name.Equals(keyName, StringComparison.Ordinal));
                if (builtInKey.name == null || builtInKey.id <= 0)
                {
                    continue;
                }

                DSPGame.globalOption.overrideKeys[builtInKey.id] = key;
                if (savedId != builtInKey.id)
                {
                    DspCore.Logger?.LogInfo($"Migrated DSPCore key bind id {savedId} -> {builtInKey.id} for {keyName}.");
                }
            }
        }
        catch (Exception ex)
        {
            DspCore.Logger?.LogWarning($"Failed to load DSPCore key binds: {ex.Message}");
        }
    }

    private static string BuildKeyName(KeyBindDescriptor descriptor)
    {
        return "DSPCore." + descriptor.OwnerModGuid + "." + descriptor.Id;
    }

    private static string DescriptorKey(KeyBindDescriptor descriptor)
    {
        return descriptor.OwnerModGuid + "\u001f" + descriptor.Id;
    }

    private static bool TryParseCombineKey(string keyText, CoreKeyAction action, out CombineKey key)
    {
        var primaryKeyText = keyText;
        byte modifiers = 0;
        foreach (var part in keyText.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var token = part.Trim();
            if (token.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) || token.Equals("Control", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= 2;
                continue;
            }

            if (token.Equals("Alt", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= 4;
                continue;
            }

            if (token.Equals("Shift", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= 1;
                continue;
            }

            keyText = token;
        }

        if (primaryKeyText.Trim().Length == 0 || !Enum.TryParse(keyText, true, out KeyCode keyCode))
        {
            key = default;
            return false;
        }

        key = new CombineKey((int)keyCode, modifiers, ToCombineAction(action), false);
        return true;
    }

    private static ECombineKeyAction ToCombineAction(CoreKeyAction action)
    {
        return action == CoreKeyAction.Hold ? ECombineKeyAction.LongPress : ECombineKeyAction.OnceClick;
    }

    private static void Resize(ref CombineKey[]? keys)
    {
        if (keys == null)
        {
            keys = new CombineKey[OverrideKeyCapacity];
            return;
        }

        if (keys.Length < OverrideKeyCapacity)
        {
            Array.Resize(ref keys, OverrideKeyCapacity);
        }
    }

    private static void Resize(ref bool[]? keys)
    {
        if (keys == null)
        {
            keys = new bool[OverrideKeyCapacity];
            return;
        }

        if (keys.Length < OverrideKeyCapacity)
        {
            Array.Resize(ref keys, OverrideKeyCapacity);
        }
    }

    private static void WriteCombineKey(BinaryWriter writer, CombineKey key)
    {
        writer.Write(key.keyCode);
        writer.Write(key.modifier);
        writer.Write(key.modifierExact);
        writer.Write((int)key.action);
        writer.Write(key.noneKey);
    }

    private static CombineKey ReadCombineKey(BinaryReader reader)
    {
        var keyCode = reader.ReadInt32();
        var modifier = reader.ReadByte();
        var modifierExact = reader.ReadBoolean();
        var action = (ECombineKeyAction)reader.ReadInt32();
        var noneKey = reader.ReadBoolean();
        var key = new CombineKey(keyCode, modifier, action, noneKey)
        {
            modifierExact = modifierExact
        };
        return key;
    }

    private static void WarnInvalidKeyOnce(KeyBindDescriptor descriptor, string keyText)
    {
        var warningKey = DescriptorKey(descriptor) + "\u001f" + keyText;
        if (!InvalidKeyWarnings.Add(warningKey))
        {
            return;
        }

        DspCore.Logger?.LogWarning($"Key binding {descriptor.Id} from {descriptor.OwnerModGuid} has invalid key text '{keyText}'.");
    }
}

internal static class KeyBindRuntimePatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
    private static void BeforeOptionWindowOpen(UIOptionWindow __instance)
    {
        KeyBindRuntime.EnsureOptionWindowKeyEntries(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.ImportXML))]
    private static void OnImportXml()
    {
        KeyBindRuntime.EnsureRuntimeArrays();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.Apply))]
    private static void OnApply()
    {
        KeyBindRuntime.EnsureRuntimeArrays();
        KeyBindRuntime.SaveCustomKeys();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.InitKeys))]
    private static IEnumerable<CodeInstruction> PatchInitKeys(IEnumerable<CodeInstruction> instructions)
    {
        return ReplaceFirstI4(instructions, 256, 512);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.SetDefaultKeys))]
    private static IEnumerable<CodeInstruction> PatchSetDefaultKeys(IEnumerable<CodeInstruction> instructions)
    {
        return ReplaceFirstI4(instructions, 256, 512);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.FreeKeys))]
    private static IEnumerable<CodeInstruction> PatchFreeKeys(IEnumerable<CodeInstruction> instructions)
    {
        return ReplaceFirstI4(instructions, 256, 512);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.Apply))]
    private static IEnumerable<CodeInstruction> PatchApply(IEnumerable<CodeInstruction> instructions)
    {
        return ReplaceFirstI4(instructions, 256, 512);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.ImportXML))]
    private static IEnumerable<CodeInstruction> PatchImportXml(IEnumerable<CodeInstruction> instructions)
    {
        return ReplaceFirstI4(instructions, 256, 512);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(UIOptionWindow), "_OnOpen")]
    private static IEnumerable<CodeInstruction> PatchOptionWindowOpen(IEnumerable<CodeInstruction> instructions)
    {
        return ReplaceFirstI4(instructions, 256, 512);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(GameOption), nameof(GameOption.ExportXML))]
    private static IEnumerable<CodeInstruction> PatchExportXml(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var matcher = new CodeMatcher(instructions, generator)
            .MatchForward(true,
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(GameOption), nameof(GameOption.overrideKeys))),
                new CodeMatch(OpCodes.Ldlen),
                new CodeMatch(OpCodes.Conv_I4),
                new CodeMatch(ci => ci.IsStloc()));

        if (!matcher.IsInvalid)
        {
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldc_I4, 256),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Math), nameof(Math.Min), new[] { typeof(int), typeof(int) })));
        }

        return matcher.InstructionEnumeration();
    }

    private static IEnumerable<CodeInstruction> ReplaceFirstI4(IEnumerable<CodeInstruction> instructions, int oldValue, int newValue)
    {
        var replaced = false;
        foreach (var instruction in instructions)
        {
            if (!replaced && instruction.opcode == OpCodes.Ldc_I4 && instruction.operand is int value && value == oldValue)
            {
                yield return new CodeInstruction(OpCodes.Ldc_I4, newValue);
                replaced = true;
                continue;
            }

            if (!replaced && instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte shortValue && shortValue == oldValue)
            {
                yield return new CodeInstruction(OpCodes.Ldc_I4, newValue);
                replaced = true;
                continue;
            }

            yield return instruction;
        }
    }
}
