# 按键

## 职责

本功能块声明按键绑定及其触发行为。

## 公开入口

- `Api/KeyBinds.cs`：作者侧短入口。
- `Api/KeyBindDescriptor.cs`
- `Api/KeyBindRegistry.cs`
- `Api/CoreKeyAction.cs`

## 示例

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`

## 运行时

`Runtime/KeyBindRuntime.cs` 会轮询已注册按键，并在按下、按住或释放时调用回调。

## 边界

当前运行时支持直接回调和简单 `Ctrl`/`Alt`/`Shift` 修饰键。完整玩家重绑定 UI 尚未实现。
