using System;
using System.Collections.Generic;
using UnityEngine;

namespace DSPCore;

internal static class KeyBindRuntime
{
    private static readonly Dictionary<string, CachedKeyBinding> KeyCache = new(StringComparer.Ordinal);
    private static readonly HashSet<string> InvalidKeyWarnings = new(StringComparer.Ordinal);

    public static void Update()
    {
        foreach (var descriptor in DspCore.KeyBinds.GetAll())
        {
            if (descriptor.Callback == null || !TryGetKeyBinding(descriptor, out var keyBinding))
            {
                continue;
            }

            if (!keyBinding.ModifiersPressed())
            {
                continue;
            }

            var triggered = descriptor.Action switch
            {
                CoreKeyAction.Press => Input.GetKeyDown(keyBinding.KeyCode),
                CoreKeyAction.Hold => Input.GetKey(keyBinding.KeyCode),
                CoreKeyAction.Release => Input.GetKeyUp(keyBinding.KeyCode),
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

    private static bool TryGetKeyBinding(KeyBindDescriptor descriptor, out KeyBinding keyBinding)
    {
        var keyText = descriptor.CanOverride ? KeyBinds.GetConfiguredKeyText(descriptor) : descriptor.DefaultKey;
        if (KeyCache.TryGetValue(descriptor.Id, out var cached) && cached.KeyText.Equals(keyText, StringComparison.Ordinal))
        {
            keyBinding = cached.Binding;
            return true;
        }

        if (!TryParseKeyBinding(keyText, out keyBinding))
        {
            WarnInvalidKeyOnce(descriptor, keyText);
            if (keyText.Equals(descriptor.DefaultKey, StringComparison.OrdinalIgnoreCase) ||
                !TryParseKeyBinding(descriptor.DefaultKey, out keyBinding))
            {
                return false;
            }

            keyText = descriptor.DefaultKey;
        }

        KeyCache[descriptor.Id] = new CachedKeyBinding(keyText, keyBinding);
        return true;
    }

    internal static bool IsValidKeyText(string keyText)
    {
        return TryParseKeyBinding(keyText, out _);
    }

    internal static bool TryNormalizeKeyText(string keyText, out string normalized)
    {
        if (!TryParseKeyBinding(keyText, out var keyBinding))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = keyBinding.ToDisplayText();
        return true;
    }

    private static bool TryParseKeyBinding(string keyText, out KeyBinding keyBinding)
    {
        var primaryKeyText = keyText;
        var modifiers = KeyModifiers.None;
        foreach (var part in keyText.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var token = part.Trim();
            if (token.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) || token.Equals("Control", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= KeyModifiers.Control;
                continue;
            }

            if (token.Equals("Alt", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= KeyModifiers.Alt;
                continue;
            }

            if (token.Equals("Shift", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= KeyModifiers.Shift;
                continue;
            }

            keyText = token;
        }

        if (primaryKeyText.Trim().Length == 0 || !Enum.TryParse(keyText, true, out KeyCode keyCode))
        {
            keyBinding = default;
            return false;
        }

        keyBinding = new KeyBinding(keyCode, modifiers);
        return true;
    }

    private static void WarnInvalidKeyOnce(KeyBindDescriptor descriptor, string keyText)
    {
        var warningKey = descriptor.Id + "\u001f" + keyText;
        if (!InvalidKeyWarnings.Add(warningKey))
        {
            return;
        }

        DspCore.Logger?.LogWarning($"Key binding {descriptor.Id} from {descriptor.OwnerModGuid} has invalid key text '{keyText}'. Falling back to '{descriptor.DefaultKey}'.");
    }

    [Flags]
    private enum KeyModifiers
    {
        None = 0,
        Control = 1,
        Alt = 2,
        Shift = 4
    }

    private readonly struct KeyBinding
    {
        public KeyBinding(KeyCode keyCode, KeyModifiers modifiers)
        {
            KeyCode = keyCode;
            Modifiers = modifiers;
        }

        public KeyCode KeyCode { get; }

        private KeyModifiers Modifiers { get; }

        public bool ModifiersPressed()
        {
            return HasModifier(KeyModifiers.Control, KeyCode.LeftControl, KeyCode.RightControl) &&
                HasModifier(KeyModifiers.Alt, KeyCode.LeftAlt, KeyCode.RightAlt) &&
                HasModifier(KeyModifiers.Shift, KeyCode.LeftShift, KeyCode.RightShift);
        }

        public string ToDisplayText()
        {
            var parts = new List<string>();
            if ((Modifiers & KeyModifiers.Control) != 0)
            {
                parts.Add("Ctrl");
            }

            if ((Modifiers & KeyModifiers.Alt) != 0)
            {
                parts.Add("Alt");
            }

            if ((Modifiers & KeyModifiers.Shift) != 0)
            {
                parts.Add("Shift");
            }

            parts.Add(KeyCode.ToString());
            return string.Join("+", parts);
        }

        private bool HasModifier(KeyModifiers modifier, KeyCode left, KeyCode right)
        {
            return (Modifiers & modifier) == 0 || Input.GetKey(left) || Input.GetKey(right);
        }
    }

    private readonly struct CachedKeyBinding
    {
        public CachedKeyBinding(string keyText, KeyBinding binding)
        {
            KeyText = keyText;
            Binding = binding;
        }

        public string KeyText { get; }

        public KeyBinding Binding { get; }
    }
}
