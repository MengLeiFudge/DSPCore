# Achievements

## Responsibility

This block aggregates achievement-disable declarations from all mods and exposes the final decision to the runtime patches.

## Public API

- `Achievements`: author-facing short entry point.
- `AchievementPolicyDeclaration`: one mod's `ModGuid` + `DisableAchievements` declaration.
- `AchievementPolicyRegistry`: global policy registry.

## Example

- `Examples/AchievementPolicyExample.cs`

## Runtime

`AchievementRuntime.cs` patches abnormality checks, local achievement mutation, Milky Way uploads, leaderboard uploads, and platform achievement/metadata calls.

There is only one rule: when any mod declares `DisableAchievements = true`, DSPCore disables achievement-related capabilities; otherwise it blocks vanilla abnormality errors and allows achievement access, leaderboard upload, and platform metadata access.

## Boundaries

This block does not let each mod patch achievements independently, and it does not expose separate upload or platform switches. Mods declare only their GUID and achievement-disable flag; DSPCore applies one aggregated policy.
