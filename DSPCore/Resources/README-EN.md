# Resources

The Resources block records shared resource declarations and applies localization entries to DSP `Localization` data. It is for resource metadata and multilingual text that other feature blocks consume, not for feature-specific runtime behavior.

## What This Block Gives You

- You can centralize resource ownership, paths, and keywords instead of scattering paths across feature blocks.
- Localization entries are applied to DSP key indices and language string arrays, so each mod does not need to reflect into `Localization` itself.
- Multilingual text can be maintained separately from UI, Proto, and Tabs declarations, reducing hardcoded strings.

## Capability: Register Resource Descriptors

```csharp
DspCore.Resources.RegisterResource(new ResourceDescriptor(
    Id: "example-icons",
    OwnerModGuid: "com.example.my-mod",
    Keyword: "icons",
    RootPath: "assets/icons",
    BundleName: null));
```

Resource descriptors only record ownership and path metadata. The consuming feature block decides how and when to load them; icon sprite loading, for example, belongs to Icons.

## Capability: Register Localization Entries

```csharp
DspCore.Resources.RegisterLocalization(new LocalizationEntry(
    Key: "ExampleMachines",
    Language: "zhCN",
    Value: "示例机器",
    OwnerModGuid: "com.example.my-mod"));
```

`Language` can be `zhCN`, `enUS`, `frFR`, `deDE`, `esES`, `jaJA`, `koKO`, or a DSP language LCID numeric string.

## What DSPCore Does After The Call

- Resource descriptors only enter the registry for feature blocks to read later.
- Localization keys are added to DSP `Localization.namesIndexer`.
- When a language is applied, DSPCore selects entries matching the current language LCID and writes values into `Localization.strings`.
- If the language arrays are too short, DSPCore expands the corresponding strings / floats arrays.

## What This Block Does Not Own

- It does not load icon sprites; icon loading belongs to Icons.
- It does not create protos or UI; it only provides resource and text data.
- It does not automatically resolve same-key conflicts between mods; later writes for the same key and language decide the final text in the array.
- It does not validate resource paths by itself; consuming feature blocks must handle load failures.

## Examples

Resources does not yet have standalone examples. See Icons, Tabs, and ProtoRegistration examples for icon id and localization key usage.
