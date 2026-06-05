using System;
using System.Collections.Generic;
using UnityEngine;

namespace DSPCore;

internal static class KeyBindRuntime
{
    private static readonly Dictionary<string, KeyBinding> KeyCache = new(StringComparer.Ordinal);

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
        if (KeyCache.TryGetValue(descriptor.Id, out keyBinding))
        {
            return true;
        }

        var keyText = descriptor.DefaultKey;
        var modifiers = KeyModifiers.None;
        foreach (var part in descriptor.DefaultKey.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
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

        if (!Enum.TryParse(keyText, true, out KeyCode keyCode))
        {
            DspCore.Logger?.LogWarning($"Key binding {descriptor.Id} has invalid DefaultKey '{descriptor.DefaultKey}'.");
            return false;
        }

        keyBinding = new KeyBinding(keyCode, modifiers);
        KeyCache[descriptor.Id] = keyBinding;
        return true;
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

        private bool HasModifier(KeyModifiers modifier, KeyCode left, KeyCode right)
        {
            return (Modifiers & modifier) == 0 || Input.GetKey(left) || Input.GetKey(right);
        }
    }
}
