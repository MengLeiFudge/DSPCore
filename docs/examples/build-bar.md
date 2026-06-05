# Build Bar / 建造栏

Register a secondary-row build button with the new API.

使用新 API 注册第二行建造栏按钮。

```csharp
using DSPCore;

DspCore.BuildBar.SetBuildBar(3, 4, 9554, layer: 2);
```

Legacy BuildBarTool calls are still accepted but obsolete.

旧 BuildBarTool 调用仍可使用，但已废弃。

```csharp
#pragma warning disable CS0618
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
```
