using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Pickers 用于请求打开原版物品、配方或信号选择器。
// - 请求会进入队列，由 DSPCore 运行时在合适的 UI update 时机打开弹窗。
// - 当前过滤器在返回时兜底检查，尚未在实时网格里隐藏无效项。
//
// Usage:
// - Call OpenItemPicker from your UI button callback.
// - OnReturn receives the selected object; always null-check and type-check.
public static class PickerExample
{
    public static void OpenItemPicker()
    {
        Pickers.Register(new PickerRequest(
            Kind: PickerKind.Item,
            OwnerModGuid: "com.example.my-mod",

            // 这里限制只能选 ID >= 9000 的物品。
            // This keeps only item protos whose id is at least 9000.
            Filter: value => value is ItemProto item && item.ID >= 9000,
            ShowLocked: true,
            ShowAll: true,
            OnReturn: OnItemSelected));
    }

    private static void OnItemSelected(object? value)
    {
        var selectedItem = value as ItemProto;
        int itemId = selectedItem?.ID ?? 0;

        // 把 itemId 保存到你的配置、UI 状态或功能声明中。
        // Store itemId in your config, UI state, or feature declaration.
    }
}
