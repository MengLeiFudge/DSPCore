# KeyBindRegistration

本场景用于注册可轮询的默认按键和触发回调。

## 适用时机

- 模组需要一个默认快捷键触发 UI、状态切换或轻量命令。
- 希望玩家可以在 DSPCore 统一设置窗口里用 Capture 按钮或文本方式改键。

## 关键参数

- `Id`：稳定按键 ID。
- `DefaultKey`：默认按键，当前支持单键和简单 `Ctrl`、`Alt`、`Shift` 修饰。
- `Action`：触发动作，例如按下。
- `ConflictGroup`：冲突分组；为 0 时不检测，非 0 时同组同键会在统一设置窗口提示。
- `CanOverride`：为 true 时自动生成设置窗口里的按键配置行。
- `DisplayPageId`：可选模组设置页 ID；传入后玩家可选择让按键显示在该模组页、统一按键页或两处。
- `Callback`：触发后的回调。

## 运行时前提

在启动阶段注册一次。普通场景使用 `KeyBinds.Register(...)` 参数重载；只有需要完整声明对象时再使用 `KeyBindDescriptor`。回调应保持短小，重逻辑应交给模组自己的状态机或后续 update 处理。统一设置窗口必须在 `UIRoot` 初始化后由按钮、快捷键或自定义 UI 回调打开。

如果传入 `displayPageId`，应先用 `Options.Page(...)` 注册同一个页面 ID；否则 DSPCore 会把没有模组页的按键回落到统一按键页，避免玩家找不到它。

## 常见误用

- 不要在回调里执行昂贵扫描。
- Capture 只捕获下一次按下的非修饰键；如果需要手写配置，玩家仍可输入类似 `Ctrl+E`、`Alt+E` 或 `E` 的文本。
- 不要期待 `ConflictGroup` 自动解决冲突；它只提示同组同键，具体改成哪个键仍由玩家或模组作者决定。

代码示例见 `KeyBindRegistrationExample.cs`。
