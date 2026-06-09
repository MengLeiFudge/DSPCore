# Game Enum Extensions

GameEnums owns the author-facing capability of extending vanilla game enum-like domains, such as new recipe types or item types. The current implementation covers custom recipe type guards and the DSPCore-reserved custom item type marker.

## What This Block Gives You

- You do not need every mod to patch assembler recipe-setting logic itself.
- A set of custom recipes can declare one type id, display name, recipe ids, and allowed machines.
- DSPCore marks target recipes as `ERecipeType.Custom` after proto cache rebuilds.
- DSPCore Preloader injects `ERecipeType.Custom = 20` and `EItemType.Custom = 100` when they are missing. Runtime code should use `GameEnums.CustomRecipeTypeValue` / `CustomItemTypeValue` instead of compiling directly against injected enum members.
- `ItemProto.SetCustomItemType()` marks an item as DSPCore's reserved custom item type through a short object extension.
- When an assembler opens or refreshes the recipe picker, DSPCore hides custom recipes that the current machine cannot use.
- The `SetRecipe` stage still keeps a final guard, reducing bad assignments if external patches or non-standard UI bypass picker filtering.

## Current Capability: Declare Custom Recipe Types

```csharp
GameEnums.RegisterRecipeType(
    id: "example-smelting",
    ownerModGuid: "com.example.my-mod",
    displayName: "ExampleSmelting",
    recipeIds: new[] { 9554001, 9554002 },
    assemblerItemIds: new[] { 2303 });
```

`RecipeIds` points to existing recipes in this type. If `AssemblerItemIds` is empty or null, current runtime does not restrict machines. If it is non-empty, only assemblers whose item proto id is listed may use this recipe type.

Existing `RecipeTypes.Register(...)` remains as the old short entry, supports the same parameter overload, and forwards to `GameEnums.RegisterRecipeType(...)`. New documentation and examples still prefer `GameEnums.RegisterRecipeType(...)` so the GameEnums capability does not stay narrowed to the RecipeTypes concept.

## Current Capability: Mark Custom Item Type

```csharp
itemProto.SetCustomItemType();
bool isCustom = itemProto.IsCustomItemType();
```

This sets `ItemProto.Type` to `(EItemType)GameEnums.CustomItemTypeValue`. It provides a stable enum slot and short author entry only; it does not create items, tabs, filters, or a full UI category.

## What DSPCore Does After The Call

- Registration stores descriptors by `Id`; if the same `Id` is registered more than once, the later declaration replaces the earlier one.
- During derived-cache rebuild, DSPCore assigns a runtime id for each type and looks up recipes listed in `RecipeIds`.
- Found recipes are marked `ERecipeType.Custom`, and DSPCore maps recipe id to descriptor.
- `ItemProto.SetCustomItemType()` directly changes the target item's `Type` field, which fits `DataUpdates` or `DataFinalFixes` cleanup for existing items.
- `GameEnums.CanAssemblerUseRecipe(assemblerEntityId, recipeId)` reuses the same restriction check.
- When an assembler window opens or refreshes the recipe picker, DSPCore records the current assembler entity and hides unsupported custom recipes during `UIRecipePicker.RefreshIcons`.
- When `AssemblerComponent.SetRecipe` is called, DSPCore still checks whether a custom recipe's current assembler entity `protoId` appears in `AssemblerItemIds`.
- Unsupported machines do not see those recipes in the picker; if external UI or another patch bypasses the picker, the recipe is still blocked at assignment time.

## What This Block Does Not Own

- It does not create `RecipeProto`; recipe creation belongs to ProtoRegistration.
- It does not create `ItemProto`; item creation belongs to ProtoRegistration / Items.
- It does not handle save migration for changed recipe ids; stable ids remain the mod author's responsibility.
- Custom item type currently provides only the reserved enum slot and marker helpers, not a complete item category UI or filtering model.
- It does not define a full UI category model; display and tab behavior should be handled with Tabs or concrete UI features.

## Examples

- `Examples/RecipeTypeRegistration.md`
- `Examples/RecipeTypeRegistrationExample.cs`
