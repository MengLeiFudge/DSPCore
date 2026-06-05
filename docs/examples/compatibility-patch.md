# Compatibility Patch / 兼容补丁

Use compatibility patches for cross-mod or cross-version fixes.

使用兼容补丁处理跨模组或跨版本修复。

```csharp
using DSPCore;

DspCore.Compatibility.Register(new CompatibilityPatchDescriptor(
    Id: "tutorial-fractionation-compat",
    TargetModGuid: "com.menglei.dsp.fe",
    TargetVersionRange: ">=2.3.0",
    Reason: "Tutorial flow needs a shared compatibility patch",
    Apply: () => { }));
```

This keeps shared fixes in the framework instead of scattering them across feature mods.

这会把共享修复留在框架层，而不是散落到功能模组里。
