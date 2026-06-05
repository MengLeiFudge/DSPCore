# Getting Started / 入门

## Install / 安装

Install `MengLei-DSPCore` in the mod manager, or place `DSPCore.dll` in the BepInEx plugins folder.

在模组管理器中安装 `MengLei-DSPCore`，或把 `DSPCore.dll` 放入 BepInEx plugins 文件夹。

## Use the New API / 使用新 API

Reference `DSPCore.dll` and use the `DSPCore` namespace.

引用 `DSPCore.dll`，并使用 `DSPCore` 命名空间。

```csharp
using DSPCore;

DspCore.Initialize();
```

## Legacy API / 旧 API

The first version keeps legacy namespaces so existing mods can run without code changes.

初版保留旧命名空间，使已有模组无需修改代码即可运行。

Legacy APIs are marked `[Obsolete]`. Treat them as migration bridges, not the long-term standard.

旧 API 会标记 `[Obsolete]`。它们是迁移桥，不是长期标准。
