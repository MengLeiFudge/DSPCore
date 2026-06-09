# 图标

Icons 模块让模组用稳定 ID 注册图标资源，并在 Proto 缓存重建后把图标应用到目标物品、配方、科技、教程或信号。

## 这个模块带来什么便利

- 你不需要在每个功能块里重复写 PNG 加载、Unity Resources 加载、sprite 缓存和 fallback 逻辑。
- 其他模块可以用稳定 `IconId` 引用图标，而不是直接依赖文件路径。
- 图标可以声明目标 Proto，DSPCore 会在运行时解析目标并写入 `_iconSprite`。
- 加载失败和目标缺失会写日志，不需要每个模组自己加同样的诊断。

## 功能：注册共享图标

```csharp
Icons.FromResources(
    id: "example-icon",
    ownerModGuid: "com.example.my-mod",
    resourcesPath: "icons/example-icon");

Icons.FromFile(
    id: "example-file-icon",
    ownerModGuid: "com.example.my-mod",
    filePath: "assets/example-icon.png",
    fallbackIconId: "example-icon");
```

`AssetPath` 可以是 Unity `Resources` sprite 路径，也可以是本地 PNG 文件路径。`Id` 应保持稳定，供 Tabs、ProtoRegistration 或你的模块代码引用。

## 功能：把图标应用到目标 Proto

如果 descriptor 指定了 `TargetKind` 和 `TargetProtoId`，DSPCore 会尝试把解析出的 sprite 写到目标 Proto：

```csharp
Icons.BindToProto(
    id: "example-item-icon",
    ownerModGuid: "com.example.my-mod",
    assetPath: "assets/example-item.png",
    targetKind: ProtoKind.Item,
    targetProtoId: 9554,
    fallbackIconId: "example-icon");
```

## 调用后 DSPCore 会怎么处理

- 注册阶段只保存图标 descriptor；同一个 `Id` 后一次注册会覆盖前一次。
- 图标解析时优先从 Unity `Resources.Load<Sprite>` 加载；失败后再按文件路径读取 PNG。
- 如果主图标加载失败且设置了 `FallbackIconId`，DSPCore 会递归解析 fallback 图标。
- 成功解析的 sprite 会按 `Id` 缓存。
- Proto 派生缓存重建时，DSPCore 会处理带目标 Proto 的图标并写入目标 `_iconSprite`。

## 这个模块不负责什么

- 不创建 Proto；目标物品、配方、科技等必须已经存在。
- 不负责本地化文本；文本属于 Resources。
- 不保证外部 PNG 路径跨机器稳定；发布模组时应使用确定的资源路径。
- 不替你处理图标尺寸、美术风格或透明边缘。

## 示例

- `Examples/IconSetRegistration.md`
- `Examples/IconSetRegistrationExample.cs`
