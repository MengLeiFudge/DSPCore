# 按键

KeyBinds 模块让模组声明默认快捷键、触发时机和回调，由 DSPCore 在运行时轮询并调用你的逻辑。

## 这个模块带来什么便利

- 你不需要每个模组自己写 `Input.GetKeyDown` / `GetKey` / `GetKeyUp` 轮询。
- 默认按键和 `Ctrl` / `Alt` / `Shift` 修饰键解析集中在 DSPCore。
- 回调异常会记录到 ErrorWindow 系统，不会静默吞掉或直接打断整条 update 流程。
- 同一套 descriptor 能表达按下、按住、释放三种触发时机。

## 功能：注册按键绑定

```csharp
KeyBinds.Register(new KeyBindDescriptor(
    Id: "example-toggle",
    OwnerModGuid: "com.example.my-mod",
    DisplayName: "ExampleToggle",
    DefaultKey: "Ctrl+K",
    Action: CoreKeyAction.Press,
    ConflictGroup: 100,
    Callback: TogglePanel));
```

`DefaultKey` 使用 Unity `KeyCode` 名称，可带简单 `Ctrl`、`Alt`、`Shift` 修饰。`Action` 可以是 `Press`、`Hold` 或 `Release`。`ConflictGroup` 当前是数字描述字段。

## 调用后 DSPCore 会怎么处理

- 注册阶段保存 descriptor；同一个 `Id` 后一次注册会覆盖前一次。
- 每帧 update 时，DSPCore 会解析并缓存 `DefaultKey`。
- 修饰键未按下时不会触发回调。
- 根据 `Action` 调用 `Input.GetKeyDown`、`Input.GetKey` 或 `Input.GetKeyUp`。
- 回调抛异常时，DSPCore 会按 `OwnerModGuid` 记录错误报告。

## 这个模块不负责什么

- 完整玩家重绑定 UI 尚未实现。
- `ConflictGroup` 当前只是 descriptor 字段，尚未提供冲突 UI 或自动冲突解决。
- 回调应保持短小；昂贵扫描或复杂状态机应交给你的模组后续 update 处理。
- 无效 `DefaultKey` 会写 warning 并跳过该绑定。

## 示例

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`
