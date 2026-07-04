# 按键

KeyBinds 模块让模组声明默认快捷键、触发时机和回调，由 DSPCore 在运行时轮询并调用你的逻辑。普通快捷键用 `KeyBinds.Register(id, ownerModGuid, displayName, defaultKey, callback, ...)`；需要传递完整描述对象时再使用 `KeyBindDescriptor`。

## 这个模块带来什么便利

- 你不需要每个模组自己写 `Input.GetKeyDown` / `GetKey` / `GetKeyUp` 轮询。
- 默认按键和 `Ctrl` / `Alt` / `Shift` 修饰键解析集中在 DSPCore。
- `CanOverride=true` 的按键会注入原版 `BuiltinKey` / `overrideKeys` 数据模型，直接出现在原版按键页。
- 玩家改键、恢复默认和冲突提示都复用原版 `UIKeyEntry` 行为。
- 回调异常会记录到 ErrorWindow 系统，不会静默吞掉或直接打断整条 update 流程。
- 同一套 descriptor 能表达按下、按住、释放三种触发时机。

## 功能：注册按键绑定

```csharp
KeyBinds.Register(
    id: "example-toggle",
    ownerModGuid: "com.example.my-mod",
    displayName: "ExampleToggle",
    defaultKey: "Ctrl+K",
    callback: TogglePanel,
    action: CoreKeyAction.Press,
    conflictGroup: 100,
    displayPageId: "com.example.settings");
```

`DefaultKey` 使用 Unity `KeyCode` 名称，可带简单 `Ctrl`、`Alt`、`Shift` 修饰。`Action` 可以是 `Press`、`Hold` 或 `Release`。`ConflictGroup` 会写入原版 `BuiltinKey.conflictGroup`，冲突判断交给原版按键页。`displayPageId` 只作为旧 API 兼容参数保留，不再控制显示位置。需要先构造或缓存完整声明时，仍可使用 `KeyBinds.Register(new KeyBindDescriptor(...))`。

## 调用后 DSPCore 会怎么处理

- 注册阶段保存 descriptor；同一个 `Id` 后一次注册会覆盖前一次。
- `CanOverride=true` 时，DSPCore 会在原版按键列表里追加一个 `BuiltinKey`，并扩展 `GameOption.overrideKeys` / `VFInput.override_keys` 容量。
- 原版按键页会创建对应 `UIKeyEntry`，玩家改键后 DSPCore 把自定义键保存到 `BepInEx/config/DSPCore/keybinds.dat`，不写入原版 options.xml 的 256 以内键区。
- 每帧 update 时，DSPCore 会优先读取原版 overrideKeys 中的玩家覆盖值；未覆盖时回落 `DefaultKey`。
- 修饰键未按下时不会触发回调。
- 根据 `Action` 调用 `Input.GetKeyDown`、`Input.GetKey` 或 `Input.GetKeyUp`。
- 回调抛异常时，DSPCore 会按 `OwnerModGuid` 记录错误报告。

## 这个模块不负责什么

- `ConflictGroup` 交给原版 UI 做同组冲突提示，不会自动改键、禁用回调或决定哪个模组优先。
- `displayPageId` 不再影响玩家 UI；按键绑定统一显示在原版按键页。
- 回调应保持短小；昂贵扫描或复杂状态机应交给你的模组后续 update 处理。
- 无效玩家配置会写 warning 并回落默认键；默认键也无效时会跳过该绑定。

## 示例

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`
