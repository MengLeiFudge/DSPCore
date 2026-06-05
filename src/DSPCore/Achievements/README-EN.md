# Achievements

## Responsibility

This block aggregates achievement policy declarations from all mods and exposes the final decision to the runtime patches.

## Public API

- `AchievementPolicyDeclaration`: one mod's declaration.
- `AchievementPolicyRegistry`: global policy registry.
- `AchievementMetadataMode`: metadata retention level.

## Example

- `Examples/AchievementPolicyExample.cs`

## Runtime

`Runtime/AchievementRuntime.cs` patches abnormality checks, local achievement mutation, Milky Way uploads, leaderboard uploads, and platform achievement calls.

## Boundaries

This block does not let each mod patch achievements independently. Mods declare intent; DSPCore applies one aggregated policy.
