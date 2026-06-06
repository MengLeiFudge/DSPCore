# KeyBindRegistration

本场景用于注册可轮询的默认按键和触发回调。

## 适用时机

- 模组需要一个默认快捷键触发 UI、状态切换或轻量命令。
- 当前不需要完整玩家重绑定 UI。

## 关键参数

- `Id`：稳定按键 ID。
- `DefaultKey`：默认按键，当前支持单键和简单 `Ctrl`、`Alt`、`Shift` 修饰。
- `Action`：触发动作，例如按下。
- `ConflictGroup`：冲突分组。
- `Callback`：触发后的回调。

## 运行时前提

在启动阶段注册一次。回调应保持短小，重逻辑应交给模组自己的状态机或后续 update 处理。

## 常见误用

- 不要在回调里执行昂贵扫描。
- 不要把玩家重绑定 UI 当作当前 KeyBinds 已完成能力。

代码示例见 `KeyBindRegistrationExample.cs`。
