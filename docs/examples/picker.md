# Picker / 选择器

Register a picker request when a feature needs the vanilla item, recipe, or signal picker. DSPCore opens the popup from the runtime update loop and calls `OnReturn`.

当某个功能需要原版物品、配方或信号选择器时，注册一个 picker request。DSPCore 会在运行时 update 循环中打开弹窗，并调用 `OnReturn`。

```csharp
using DSPCore;

DspCore.Pickers.Register(new PickerRequest(
    Kind: PickerKind.Item,
    OwnerModGuid: "com.example.my-mod",
    Filter: value => value is ItemProto item && item.ID >= 9000,
    ShowLocked: true,
    ShowAll: true,
    OnReturn: value =>
    {
        var selectedItem = value as ItemProto;
        ExamplePanel.SetItem(selectedItem?.ID ?? 0);
    }));
```

Current runtime note: filters are applied when the picker returns. They do not yet hide invalid entries inside the live picker grid.

当前运行时说明：过滤器会在选择器返回时执行，尚未在实时选择器网格中隐藏无效条目。
