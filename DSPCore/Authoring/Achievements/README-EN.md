# Achievements

The Achievements block lets a mod declare whether it requires achievements to be disabled. DSPCore aggregates declarations from all mods and applies one runtime policy to vanilla abnormality checks, local achievement mutation, Milky Way / leaderboard uploads, and Steam / RAIL / XGP platform achievement calls.

## What This Block Gives You

- You only declare your mod GUID and `disableAchievements`; you do not need to patch `AchievementSystem`, `GameAbnormalityData_0925`, `MilkyWayWebClient`, `STEAMX`, `RAILX`, or `XGPX` yourself.
- When multiple mods are loaded, you do not need to coordinate achievement policy with other mods; DSPCore resolves the declarations into one global policy.
- DSPCore applies the disable decision consistently to abnormality checks, achievement unlock/progress calls, uploads, and platform metadata calls.

## How To Use It

Call this once during plugin `Awake` or feature registration:

```csharp
using DSPCore;

Achievements.Declare("com.example.my-mod", disableAchievements: true);
```

Use a stable `modGuid`, normally your BepInEx plugin GUID. `disableAchievements` tells DSPCore whether your mod requires achievements to be disabled globally.

## What DSPCore Does After The Call

DSPCore records the declaration by `modGuid`. If the same `modGuid` declares more than once, the later declaration replaces the earlier one.

At runtime, DSPCore computes all declarations:

- If any mod declares `disableAchievements: true`, the final policy disables achievements.
- If no mod declares true, the final policy keeps achievements enabled.

The final policy affects these game behaviors:

- Vanilla abnormality checks: when achievements are not disabled, DSPCore blocks abnormality checks and clears imported abnormality runtime data; when achievements are disabled, it no longer blocks abnormality checks.
- Local achievement mutation: when achievements are disabled, DSPCore blocks `AchievementSystem.UnlockAchievement` and `SetAchievementProgress`.
- Uploads: when achievements are disabled, DSPCore blocks Milky Way uploads and Steam leaderboard uploads.
- Platform calls: when achievements are disabled, DSPCore blocks Steam / RAIL / XGP platform achievement and metadata calls.

## Common Cases

- You do not call `Achievements.Declare(...)`: your mod does not request achievement disabling. If no other mod declares true, DSPCore keeps achievements available and blocks vanilla abnormality checks.
- You declare `disableAchievements: false`: your mod explicitly says it does not require achievement disabling. This behaves like not declaring for the final policy, but the declaration appears in the `GetDeclarations()` snapshot.
- You declare `disableAchievements: true`: your mod requests global achievement disabling. One true declaration from any mod makes DSPCore block achievement mutation, uploads, and platform calls.
- Multiple mods declare policies: any true wins; false cannot cancel another mod's true declaration.
- The same GUID declares more than once: the later declaration replaces the earlier one. Do not declare every frame in `Update`.

## What This Block Does Not Provide

- It does not provide independent switches such as "disable uploads but keep platform achievements".
- It does not let each mod apply its own separate achievement patch result; DSPCore applies one aggregated global policy.
- It does not decide whether your mod should disable achievements; the mod author still owns that gameplay judgment.

## Examples

- `Examples/AchievementPolicy.md`
- `Examples/AchievementPolicyExample.cs`
