using DSPCore;
using static DSPCore.DspCore;

namespace ExampleMod;

public static class KeyBindExample
{
    public static void Register()
    {
        KeyBinds.Register(new KeyBindDescriptor(
            Id: "example.toggle-panel",
            OwnerModGuid: "com.example.my-mod",
            DisplayName: "Toggle Example Panel",
            DefaultKey: "Ctrl+E",
            Action: CoreKeyAction.Press,
            ConflictGroup: 2,
            CanOverride: true,
            Callback: ToggleExamplePanel));
    }

    private static void ToggleExamplePanel()
    {
    }
}
