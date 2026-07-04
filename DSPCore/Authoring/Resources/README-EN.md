# Resources

The Resources block records shared resource declarations and applies localization entries to DSP `Localization` data. It is for resource metadata and multilingual text that other authoring capabilities or systems consume, not for feature-specific runtime behavior.

## What This Block Gives You

- You can centralize resource ownership, paths, and keywords and consume those resources through one resolve/read surface instead of scattering paths across capabilities.
- One mod can create `ModResources.Pack(...)` first, then register text, icons, and proto icon bindings without repeating `ownerModGuid` and root paths.
- When the target proto kind is already clear, use `pack.ItemIcon(...)`, `RecipeIcon(...)`, `TechIcon(...)`, `TutorialIcon(...)`, or `SignalIcon(...)` without writing `ProtoKind` manually.
- Localization entries are applied to DSP key indices and language string arrays, so each mod does not need to reflect into `Localization` itself.
- After registration, localization entries can be queried immediately through `ModResources.Translate(...)` or `pack.Translate(...)` without waiting for DSP `Localization.LoadLanguage`.
- Players or translators can maintain `DSPCore/Locales/locale-<language>.tsv` under the BepInEx config directory to override translations from any mod. The override layer affects both live queries and vanilla language arrays.
- Multilingual text can be maintained separately from UI, Proto, and Tabs declarations, reducing hardcoded strings.

## Capability: Use A Resource-Pack Short Entry

```csharp
var pack = ModResources.Pack(
    ownerModGuid: "com.example.my-mod",
    rootPath: "assets",
    assembly: typeof(MyPlugin).Assembly);

pack.Root(id: "example-assets", keyword: "assets");
pack.Text("ExampleMachines", "zhCN", "示例机器");
pack.Text("ExampleMachines", "enUS", "Example Machines");
var title = pack.Translate("ExampleMachines", language: "zhCN");
pack.ItemIcon("example-machine-icon", "icons/example-machine.png", itemId: 9554);
```

`ModResourcePack` reuses the same owner, resource root, and default assembly. It fits a mod that registers multiple localization entries, icons, or proto icon bindings around the same resources. `ItemIcon` / `RecipeIcon` typed helpers resolve relative paths under `RootPath` and fix the target kind to the `ProtoKind` named by the method.

## Capability: Register Resource Descriptors

If you only need one independent resource root, the lower-level short entry remains available:

```csharp
ModResources.Root(
    id: "example-icons",
    ownerModGuid: "com.example.my-mod",
    keyword: "icons",
    rootPath: "assets/icons");
```

Resource descriptors enter a shared resource index. Consumers can use `DspCore.Resources.ResolvePath(...)`, `TryResolvePath(...)`, `TryOpenRead(...)`, or `TryReadBytes(...)` to resolve and read files by root id, owner + keyword, or the owner's default resource root; Icons reuses this for local PNG and AssetBundle paths.

## Capability: Register Independent Localization Entries

```csharp
ModResources.Text(
    key: "ExampleMachines",
    language: "zhCN",
    value: "示例机器",
    ownerModGuid: "com.example.my-mod");
```

`Language` can be `zhCN`, `enUS`, `frFR`, `deDE`, `esES`, `jaJA`, `koKO`, or a DSP language LCID numeric string.

## Capability: Query Registered Text Immediately

```csharp
ModResources.Text("ExampleMachines", "zhCN", "示例机器", "com.example.my-mod");

var zh = ModResources.Translate("ExampleMachines", language: "zhCN");
var current = ModResources.Translate("ExampleMachines", fallback: "Example Machines");
```

`Translate(...)` reads the DSPCore registry directly. When `language` is provided, it queries that language; otherwise it uses the current game language LCID. When the key is missing, it returns `fallback`, or the key itself if no fallback is provided. This query does not depend on whether DSP `Localization.currentStrings` has already been refreshed.

## Capability: Player Locale File Overrides

Players, translators, or modpack authors can create these files under the BepInEx config directory:

```text
DSPCore/Locales/locale-zhCN.tsv
DSPCore/Locales/locale-enUS.tsv
```

Each non-comment line is one translation in `key<TAB>value` format:

```text
# Comment lines are ignored.
ExampleMachines	Example Machines
ExampleDescription	Line one\nline two
```

Language suffixes support `zhCN`, `enUS`, `frFR`, `deDE`, `esES`, `jaJA`, `koKO`, or a direct DSP LCID numeric string. DSPCore reads these files on startup, so file edits require a game restart. Player overrides have higher priority than mod-author entries for the same key and language, which lets them override translations from any mod.

## What DSPCore Does After The Call

- Resource descriptors enter the registry and can be consumed through shared path resolution and file-reading entries.
- `ModResourcePack` reuses the same resource path rules to resolve relative paths under its `RootPath`; embedded resource names are still passed to Icons as manifest resource names.
- When Icons reads local PNG files or AssetBundles, it first tries the original path, then resolves it through registered resource roots for the icon owner.
- At startup, DSPCore reads `DSPCore/Locales/locale-*.tsv` and registers player overrides into DSPCore's own localization index.
- Localization keys are added to DSP `Localization.namesIndexer`.
- When a language is applied, DSPCore selects entries matching the current language LCID and writes values into `Localization.strings`; if a player file and an author entry both define the same key, the player file wins.
- `Translate(...)` / `TryTranslate(...)` read DSPCore's own live localization index, so newly registered entries are visible immediately and player file overrides are returned first.
- If the language arrays are too short, DSPCore expands the corresponding strings / floats arrays.

## What This Block Does Not Own

- It does not load icon sprites; icon loading belongs to Icons.
- It does not create protos or UI; it only provides resource and text data.
- `ItemIcon` typed helpers only register icon descriptors and target bindings; they do not create target protos.
- It does not automatically resolve same-key conflicts between mods. Inside the author-entry layer, the last value written for the same key and language still wins, but player locale files always have priority.
- It does not validate resource paths at registration time; consuming capabilities or systems must handle load failures, and `TryOpenRead(...)` / `TryReadBytes(...)` report success through their return value.
- It does not actively load resource DLLs; `Pack(..., assembly: ...)` only records the default assembly, and resource DLLs must already be loaded into the current AppDomain.
- It does not hot-reload player locale files. Restart the game after editing translation files.

## Examples

- `Examples/ResourceRegistration.md`
- `Examples/ResourceRegistrationExample.cs`
