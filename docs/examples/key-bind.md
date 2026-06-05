# Key Bind / 按键

Key binds declare player-rebindable controls.

按键声明用于玩家可重绑定控制。

```csharp
using DSPCore;

DspCore.KeyBinds.Register(new KeyBindDescriptor(
    Id: "example.toggle-panel",
    OwnerModGuid: "com.example.my-mod",
    DisplayName: "Toggle Example Panel",
    DefaultKey: "Ctrl+E",
    Action: CoreKeyAction.Press,
    ConflictGroup: 2,
    CanOverride: true));
```
