# Icon Set / 图标集

Register icon metadata with `IconSetRegistry`.

使用 `IconSetRegistry` 注册图标元数据。

```csharp
using DSPCore;

DspCore.Icons.Register(new IconDescriptor(
    Id: "example-machine",
    OwnerModGuid: "com.example.my-mod",
    AssetPath: "assets/icons/example-machine.png",
    FallbackIconId: "default-machine"));
```

The first version defines registration and lookup. Runtime asset loading is a later step.

初版定义注册和查找。运行时资源加载属于后续步骤。
