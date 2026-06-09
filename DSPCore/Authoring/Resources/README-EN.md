# Resources

The Resources block records shared resource declarations and applies localization entries to DSP `Localization` data. It is for resource metadata and multilingual text that other authoring capabilities or systems consume, not for feature-specific runtime behavior.

## What This Block Gives You

- You can centralize resource ownership, paths, and keywords instead of scattering paths across capabilities.
- Localization entries are applied to DSP key indices and language string arrays, so each mod does not need to reflect into `Localization` itself.
- Multilingual text can be maintained separately from UI, Proto, and Tabs declarations, reducing hardcoded strings.

## Capability: Register Resource Descriptors

```csharp
ModResources.Root(
    id: "example-icons",
    ownerModGuid: "com.example.my-mod",
    keyword: "icons",
    rootPath: "assets/icons");
```

Resource descriptors only record ownership and path metadata. The consuming capability or system decides how and when to load them; icon sprite loading, for example, belongs to Icons.

## Capability: Register Localization Entries

```csharp
ModResources.Text(
    key: "ExampleMachines",
    language: "zhCN",
    value: "示例机器",
    ownerModGuid: "com.example.my-mod");
```

`Language` can be `zhCN`, `enUS`, `frFR`, `deDE`, `esES`, `jaJA`, `koKO`, or a DSP language LCID numeric string.

## What DSPCore Does After The Call

- Resource descriptors only enter the registry for capabilities or systems to read later.
- Localization keys are added to DSP `Localization.namesIndexer`.
- When a language is applied, DSPCore selects entries matching the current language LCID and writes values into `Localization.strings`.
- If the language arrays are too short, DSPCore expands the corresponding strings / floats arrays.

## What This Block Does Not Own

- It does not load icon sprites; icon loading belongs to Icons.
- It does not create protos or UI; it only provides resource and text data.
- It does not automatically resolve same-key conflicts between mods; later writes for the same key and language decide the final text in the array.
- It does not validate resource paths by itself; consuming capabilities or systems must handle load failures.

## Examples

- `Examples/ResourceRegistration.md`
- `Examples/ResourceRegistrationExample.cs`
