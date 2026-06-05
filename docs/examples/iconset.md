# Icon Set / 图标集

Register icon metadata with `IconSetRegistry`. DSPCore can load a Unity `Resources` sprite path or a local PNG file path, then apply it to a target proto when `TargetKind` and `TargetProtoId` are provided.

使用 `IconSetRegistry` 注册图标元数据。DSPCore 可以加载 Unity `Resources` sprite 路径或本地 PNG 文件路径，并在提供 `TargetKind` 与 `TargetProtoId` 时写入目标 Proto。

```csharp
using DSPCore;

DspCore.Icons.Register(new IconDescriptor(
    Id: "example-machine",
    OwnerModGuid: "com.example.my-mod",
    AssetPath: "assets/icons/example-machine.png",
    FallbackIconId: "default-machine",
    TargetKind: ProtoKind.Item,
    TargetProtoId: 9554));
```

If the primary asset cannot be loaded, DSPCore resolves `FallbackIconId` and caches the loaded sprite.

如果主资源无法加载，DSPCore 会解析 `FallbackIconId` 并缓存已加载的 sprite。
