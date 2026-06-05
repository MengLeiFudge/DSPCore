# 图标

## 职责

本功能块注册图标描述，并解析共享图标资源。

## 公开入口

- `Icons`：作者侧短入口。
- `IconDescriptor`
- `IconSetRegistry`

## 示例

- `Examples/IconSetExample.cs`

## 运行时

`IconRuntime.cs` 会加载 Unity `Resources` sprite 或本地 PNG 文件，缓存 sprite，解析 fallback，并把图标写入目标 Proto。

## 边界

图标注册不创建物品、配方或科技 Proto。原型功能需要图标绑定时调用本功能块。
