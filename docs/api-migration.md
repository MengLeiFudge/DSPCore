# API Migration / API 迁移

## LDBTool / LDBTool

Old:

```csharp
xiaoye97.LDBTool.PreAddProto(proto);
```

New:

```csharp
DSPCore.DspCore.Protos.RegisterPreload(proto.GetType(), proto, "your.mod.guid");
```

旧 API 会保留为 `[Obsolete]` shim。新模组应迁移到 DSPCore。

Legacy APIs are kept as `[Obsolete]` shims. New mods should migrate to DSPCore.

## DSPModSave / DSPModSave

Old:

```csharp
class MyPlugin : crecheng.DSPModSave.IModCanSave
{
}
```

New:

```csharp
class MySaveHandler : DSPCore.ICoreSaveHandler
{
}
```

## CommonAPI / CommonAPI

Old:

```csharp
[CommonAPI.CommonAPISubmoduleDependency("ProtoRegistry")]
```

New:

```csharp
DSPCore.DspCore.Modules.Register(new DSPCore.ModuleDescriptor(
    "ProtoRegistry",
    "Proto Registry",
    () => { }));
```

## BuildBarTool / BuildBarTool

Old:

```csharp
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
```

New:

```csharp
DSPCore.DspCore.BuildBar.SetBuildBar(3, 4, 9554, DSPCore.BuildBarTier.Secondary);
```
