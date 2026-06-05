# Key Bind / 按键

Key binds declare player-rebindable controls and optional runtime callbacks. The current runtime supports single keys plus simple `Ctrl`, `Alt`, and `Shift` modifier combinations in `DefaultKey`.

按键声明用于玩家可重绑定控制，也可以提供运行时回调。当前运行时支持单键，以及 `DefaultKey` 中简单的 `Ctrl`、`Alt` 和 `Shift` 修饰键组合。

```csharp
using DSPCore;

DspCore.KeyBinds.Register(new KeyBindDescriptor(
    Id: "example.toggle-panel",
    OwnerModGuid: "com.example.my-mod",
    DisplayName: "Toggle Example Panel",
    DefaultKey: "Ctrl+E",
    Action: CoreKeyAction.Press,
    ConflictGroup: 2,
    CanOverride: true,
    Callback: () => ExamplePanel.Toggle()));
```
