# 按键

KeyBinds 模块让模组声明默认快捷键、触发时机和回调，由 DSPCore 在运行时轮询并调用你的逻辑。普通快捷键用 `KeyBinds.Register(id, ownerModGuid, displayName, defaultKey, callback, ...)`；需要传递完整描述对象时再使用 `KeyBindDescriptor`。

## 这个模块带来什么便利

- 你不需要每个模组自己写 `Input.GetKeyDown` / `GetKey` / `GetKeyUp` 轮询。
- 默认按键和 `Ctrl` / `Alt` / `Shift` 修饰键解析集中在 DSPCore。
- `CanOverride=true` 的按键会自动进入 DSPCore 统一设置窗口，玩家可以点击 Capture 后按键，或直接用文本方式改成新的按键组合。
- 作者可以给按键声明 `displayPageId`，让它跟随模组自己的设置页显示；玩家可以在 DSPCore 设置页的 `Key Bind Display` 中选择只显示在模组设置页、只显示在统一按键页，或两处都显示。
- 同一 `ConflictGroup` 内配置成相同按键时，统一设置窗口会提示冲突对象，减少多个模组抢同一快捷键时的排查成本。
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

`DefaultKey` 使用 Unity `KeyCode` 名称，可带简单 `Ctrl`、`Alt`、`Shift` 修饰。`Action` 可以是 `Press`、`Hold` 或 `Release`。`ConflictGroup` 为 0 时不参与冲突检测；非 0 时，同组按键会在统一设置窗口中按规范化后的按键文本检测冲突。`displayPageId` 为空时按键只具备统一按键页显示位置；如果你希望按键跟随模组设置页，需要先用 `Options.Page(...)` 注册同一个页面 ID。需要先构造或缓存完整声明时，仍可使用 `KeyBinds.Register(new KeyBindDescriptor(...))`。

## 功能：玩家选择按键显示位置

DSPCore 会注册自己的设置页和 `Key Bind Display` 枚举项，默认值是 `Both`：

- `ModSettings`：只在作者声明的模组设置页显示；没有 `displayPageId` 的按键仍回落到统一按键页，避免消失。
- `KeyBindings`：只在 DSPCore 的统一 `Key Bindings` 页显示。
- `Both`：同时显示在模组设置页和统一按键页；两个位置读写同一个底层配置项。

## 调用后 DSPCore 会怎么处理

- 注册阶段保存 descriptor；同一个 `Id` 后一次注册会覆盖前一次。
- `CanOverride=true` 时，DSPCore 会把按键写入统一设置窗口对应的配置项。
- 打开统一设置窗口时，DSPCore 会按玩家的 `Key Bind Display` 策略决定按键行显示在哪些页面；多处显示时不会复制底层配置项。
- 每帧 update 时，DSPCore 会优先读取玩家配置的按键文本；配置为空或非法时回落 `DefaultKey`。
- 打开统一设置窗口时，DSPCore 会扫描同一 `ConflictGroup` 内的有效按键文本；如果当前按键与其他声明相同，会在当前设置行显示冲突对象。
- 修饰键未按下时不会触发回调。
- 根据 `Action` 调用 `Input.GetKeyDown`、`Input.GetKey` 或 `Input.GetKeyUp`。
- 回调抛异常时，DSPCore 会按 `OwnerModGuid` 记录错误报告。

## 这个模块不负责什么

- Capture 会捕获下一次按下的非修饰键，并附带当前按住的 `Ctrl` / `Alt` / `Shift`；单独按修饰键不会完成捕获。
- `ConflictGroup` 只负责同组同键提示，不会自动改键、禁用回调或决定哪个模组优先。
- 当前显示位置策略只作用于 DSPCore 自有统一设置窗口；原版按键页末尾只提供“DSPCore 按键设置”入口，不会把按键行注入原版 `BuiltinKey` / `overrideKeys` 数据模型。
- 回调应保持短小；昂贵扫描或复杂状态机应交给你的模组后续 update 处理。
- 无效玩家配置会写 warning 并回落默认键；默认键也无效时会跳过该绑定。

## 示例

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`
