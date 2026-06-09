# Achievements

The Achievements block lets a mod declare whether it requests competitive uploads to be blocked. DSPCore aggregates declarations from all mods and applies one runtime policy to vanilla abnormality checks, Milky Way / leaderboard uploads, and Steam / RAIL / XGP platform achievement calls.

## What This Block Gives You

- You only declare your mod GUID and whether competitive uploads should be blocked; you do not need to patch `GameAbnormalityData_0925`, `MilkyWayWebClient`, `STEAMX`, `RAILX`, or `XGPX` yourself.
- When multiple mods are loaded, you do not need to coordinate achievement policy with other mods; DSPCore resolves the declarations into one global policy.
- DSPCore always blocks vanilla abnormality checks so local achievements stay available. When any mod requests competitive upload blocking, DSPCore blocks only Milky Way / Steam leaderboard uploads.

## How To Use It

Call this once during plugin `Awake` or feature registration:

```csharp
using DSPCore;

Achievements.BlockCompetitiveUpload("com.example.my-mod");
```

Use a stable `modGuid`, normally your BepInEx plugin GUID. `BlockCompetitiveUpload(...)` tells DSPCore that your mod requests Milky Way / leaderboard uploads to be blocked.

## What DSPCore Does After The Call

DSPCore records the declaration by `modGuid`. If the same `modGuid` declares more than once, the later declaration replaces the earlier one.

At runtime, DSPCore computes all declarations:

- If any mod requests competitive upload blocking, the final policy blocks Milky Way / leaderboard uploads.
- If no mod requests blocking, the final policy allows uploads.

The final policy affects these game behaviors:

- Vanilla abnormality checks: DSPCore always blocks abnormality checks and clears imported abnormality runtime data, so local achievements stay available.
- Local achievement mutation: DSPCore does not block `AchievementSystem.UnlockAchievement` or `SetAchievementProgress`.
- Uploads: when any mod requests competitive upload blocking, DSPCore blocks Milky Way uploads and Steam leaderboard uploads.
- Platform calls: DSPCore does not block Steam / RAIL / XGP platform achievement or metadata calls.

## Common Cases

- You do not call `Achievements.BlockCompetitiveUpload(...)`: your mod does not request upload blocking.
- You call `Achievements.BlockCompetitiveUpload(modGuid)`: your mod requests competitive upload blocking. One true declaration from any mod makes DSPCore block Milky Way / leaderboard uploads.
- You call `Achievements.BlockCompetitiveUpload(modGuid, blockUpload: false)`: your mod explicitly says it does not request upload blocking. This behaves like not declaring for the final policy, but the declaration appears in the `GetDeclarations()` snapshot.
- Multiple mods declare policies: any true wins; false cannot cancel another mod's true declaration.
- The same GUID declares more than once: the later declaration replaces the earlier one. Do not declare every frame in `Update`.

`Achievements.Declare(modGuid, disableAchievements)` and `AchievementPolicyDeclaration.DisableAchievements` remain as compatibility entries. In the current policy, this bool means "block competitive uploads"; it no longer means blocking local or platform achievements.

## What This Block Does Not Provide

- It does not provide independent switches such as "block local achievements but allow uploads".
- It does not let each mod apply its own separate achievement patch result; DSPCore applies one aggregated global policy.
- It does not decide whether your mod should block competitive uploads; the mod author still owns that gameplay judgment.

## Examples

- `Examples/AchievementPolicy.md`
- `Examples/AchievementPolicyExample.cs`
