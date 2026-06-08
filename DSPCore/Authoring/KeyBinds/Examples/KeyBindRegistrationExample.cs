using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - KeyBinds 用于声明可重绑定按键和触发回调。
// - 当前运行时支持单键以及简单 Ctrl/Alt/Shift 修饰键。
// - 完整玩家重绑定 UI 仍是后续功能；这里先声明默认键和冲突组。
//
// Usage:
// - Register once during plugin startup.
// - Keep callbacks short; expensive work should be deferred to your own update logic.
public static class KeyBindRegistrationExample
{
    public static void Register()
    {
        KeyBinds.Register(new KeyBindDescriptor(
            // Id 应稳定，通常使用 mod 前缀。
            // Use a stable id, normally prefixed by your mod id.
            Id: "example.toggle-panel",
            OwnerModGuid: "com.example.my-mod",
            DisplayName: "Toggle Example Panel",

            // 当前格式支持类似 Ctrl+E、Alt+E、Shift+E 或单键 E。
            // Current format supports Ctrl+E, Alt+E, Shift+E, or a single key.
            DefaultKey: "Ctrl+E",
            Action: CoreKeyAction.Press,
            ConflictGroup: 2,
            CanOverride: true,
            Callback: ToggleExamplePanel));
    }

    private static void ToggleExamplePanel()
    {
        // 在这里打开/关闭你的 UI，或者只设置状态让主逻辑稍后处理。
        // Toggle UI here, or set state and let your main logic process it later.
    }
}
