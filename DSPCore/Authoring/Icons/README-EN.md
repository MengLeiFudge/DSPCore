# Icons

The Icons block lets a mod register icon resources by stable id and apply them to target item, recipe, tech, tutorial, or signal protos after proto cache rebuilds.

## What This Block Gives You

- You do not need to repeat PNG loading, Unity Resources / AssetBundle loading, sprite caching, and fallback logic in every capability.
- If one mod has a shared resource root, `ModResources.Pack(...)` can register icons without repeating `ownerModGuid` and path prefixes.
- When the target kind is clear, use `pack.ItemIcon(...)`, `RecipeIcon(...)`, `TechIcon(...)`, `TutorialIcon(...)`, or `SignalIcon(...)` without repeating `ProtoKind`.
- Other modules can reference a stable `IconId` instead of depending directly on file paths.
- Icons can declare a target Proto, and DSPCore resolves the target and writes `_iconSprite` at runtime.
- Load failures and missing targets are logged centrally.

## Capability: Register Shared Icons Through A Resource Pack

```csharp
var pack = ModResources.Pack(
    ownerModGuid: "com.example.my-mod",
    rootPath: "assets/icons",
    assembly: typeof(MyPlugin).Assembly);

pack.IconFromFile("example-file-icon", "example-icon.png", fallbackIconId: "default-machine");
pack.IconFromEmbedded("example-embedded-icon", "ExampleMod.Assets.example-icon.png");
pack.IconFromAssetBundle("example-bundle-icon", "example-icons", "example-machine");
pack.ItemIcon("example-machine", "example-machine.png", itemId: 9554);
```

The pack resolves relative paths under `RootPath` through the shared Resources path rules, so `"example-machine.png"` becomes `"assets/icons/example-machine.png"`. `ItemIcon` / `RecipeIcon` typed helpers automatically select the target `ProtoKind`. Embedded resource names are not combined with the root because they must be assembly manifest resource names.

## Capability: Register Independent Shared Icons

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

Icons.FromEmbedded(
    id: "example-embedded-icon",
    ownerModGuid: "com.example.my-mod",
    assembly: typeof(MyPlugin).Assembly,
    resourceName: "ExampleMod.Assets.example-icon.png");

Icons.FromAssetBundle(
    id: "example-bundle-icon",
    ownerModGuid: "com.example.my-mod",
    bundlePath: "assets/example-icons",
    assetName: "example-machine");
```

The pack typed helpers are short forms of this advanced entry. `pack.ItemIcon(...)` reuses the pack owner/root and fixes `targetKind` to `ProtoKind.Item`.

`AssetPath` can be a Unity `Resources` sprite path, a local PNG file path, an embedded PNG in a loaded assembly through `FromEmbedded`, or a `Sprite` / `Texture2D` inside an AssetBundle through `FromAssetBundle`. Keep `Id` stable so Tabs, ProtoRegistration, or your own module code can reference it.

## Capability: Apply Icons To Target Proto Objects

If a descriptor specifies `TargetKind` and `TargetProtoId`, DSPCore tries to write the resolved sprite to that target Proto:

```csharp
Icons.BindToProto(
    id: "example-item-icon",
    ownerModGuid: "com.example.my-mod",
    assetPath: "assets/example-item.png",
    targetKind: ProtoKind.Item,
    targetProtoId: 9554,
    fallbackIconId: "example-icon");
```

## What DSPCore Does After The Call

- Registration only stores the icon descriptor; if the same `Id` is registered more than once, the later registration replaces the earlier one.
- `ModResourcePack` only fills owner and relative paths; the final registration is still a normal `IconDescriptor`.
- Icon resolution recognizes internal `embedded://` and `assetbundle://` paths. Normal paths first try `Resources.Load<Sprite>`, then read a PNG file path.
- Local PNG and AssetBundle paths resolve through `DspCore.Resources`: runtime tries the original path first, then checks resource roots registered by the icon owner.
- `FromEmbedded` uses an internal `embedded://` path convention. Runtime reads the manifest resource stream from an assembly already loaded into the current AppDomain.
- `FromAssetBundle` uses an internal `assetbundle://` path convention. Runtime caches the AssetBundle, loads a `Sprite` by asset name, and falls back to `Texture2D`.
- If the primary icon fails and `FallbackIconId` is set, DSPCore recursively resolves the fallback icon.
- Successfully resolved sprites are cached by `Id`.
- During proto derived-cache rebuild, DSPCore applies icons that declare target protos and writes the target `_iconSprite`.

## What This Block Does Not Own

- It does not create protos; target items, recipes, techs, and other protos must already exist.
- It does not own localization text; use Resources for text.
- It does not make external PNG paths stable across machines unless a resource root is registered. Published mods should use deterministic paths or register a root through `ModResources.Root(...)` / `ModResources.Pack(...)`.
- Typed bindings for embedded resources and AssetBundles are not expanded separately yet. Use `BindEmbeddedIconToProto(...)` or `BindAssetBundleIconToProto(...)` for complex sources.
- AssetBundle unloading is not owned here; mods that need finer lifecycle control should manage separate bundles themselves.
- It does not actively load resource DLLs. If you use a resource DLL, your mod or loader must load that assembly into the current AppDomain first.
- It does not handle icon art quality, dimensions, or transparent edges for you.

## Examples

- `Examples/IconSetRegistration.md`
- `Examples/IconSetRegistrationExample.cs`
