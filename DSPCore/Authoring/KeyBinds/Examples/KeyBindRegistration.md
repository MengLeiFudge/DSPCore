# KeyBindRegistration

本场景用于注册可轮询的默认按键和触发回调。

## 适用时机

- 模组需要一个默认快捷键触发 UI、状态切换或轻量命令。
- 希望玩家可以在原版按键页里修改模组按键。

## 关键参数

- `Id`：稳定按键 ID。
- `DefaultKey`：默认按键，当前支持单键和简单 `Ctrl`、`Alt`、`Shift` 修饰。
- `Action`：触发动作，例如按下。
- `ConflictGroup`：冲突分组；写入原版 `BuiltinKey.conflictGroup` 后交给原版按键页提示。
- `CanOverride`：为 true 时注入原版按键页。
- `DisplayPageId`：旧兼容参数；当前不再控制玩家 UI 位置。
- `Callback`：触发后的回调。

## 运行时前提

在启动阶段注册一次。普通场景使用 `KeyBinds.Register(...)` 参数重载；只有需要完整声明对象时再使用 `KeyBindDescriptor`。回调应保持短小，重逻辑应交给模组自己的状态机或后续 update 处理。原版按键页在打开时会读取 DSPCore 注入的 `BuiltinKey` 条目。

## 常见误用

- 不要在回调里执行昂贵扫描。
- 不要期待 `ConflictGroup` 自动解决冲突；它只提示同组同键，具体改成哪个键仍由玩家或模组作者决定。

代码示例见 `KeyBindRegistrationExample.cs`。
