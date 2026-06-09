# Icons

The Icons block lets a mod register icon resources by stable id and apply them to target item, recipe, tech, tutorial, or signal protos after proto cache rebuilds.

## What This Block Gives You

- You do not need to repeat PNG loading, Unity Resources loading, sprite caching, and fallback logic in every capability.
- Other modules can reference a stable `IconId` instead of depending directly on file paths.
- Icons can declare a target Proto, and DSPCore resolves the target and writes `_iconSprite` at runtime.
- Load failures and missing targets are logged centrally.

## Capability: Register Shared Icons

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

`AssetPath` can be a Unity `Resources` sprite path or a local PNG file path. Keep `Id` stable so Tabs, ProtoRegistration, or your own module code can reference it.

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
- Icon resolution first tries `Resources.Load<Sprite>`, then reads a PNG file path.
- If the primary icon fails and `FallbackIconId` is set, DSPCore recursively resolves the fallback icon.
- Successfully resolved sprites are cached by `Id`.
- During proto derived-cache rebuild, DSPCore applies icons that declare target protos and writes the target `_iconSprite`.

## What This Block Does Not Own

- It does not create protos; target items, recipes, techs, and other protos must already exist.
- It does not own localization text; use Resources for text.
- It does not make external PNG paths stable across machines; published mods should use deterministic resource paths.
- It does not handle icon art quality, dimensions, or transparent edges for you.

## Examples

- `Examples/IconSetRegistration.md`
- `Examples/IconSetRegistrationExample.cs`
