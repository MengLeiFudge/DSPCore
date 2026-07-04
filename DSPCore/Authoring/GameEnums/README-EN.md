# Game Enum Extensions

GameEnums owns the author-facing capability of extending vanilla game enum-like domains, such as new recipe types or item types. The current implementation covers custom recipe type guards, custom item type declarations, and runtime mapping through DSPCore-reserved enum slots.

## What This Block Gives You

- You do not need every mod to patch assembler recipe-setting logic itself.
- A set of custom recipes can declare one type id, display name, recipe ids, and allowed machines.
- A set of custom items can declare one type id, display name, and item ids.
- DSPCore marks target recipes as `ERecipeType.Custom` after proto cache rebuilds.
- DSPCore Preloader injects `ERecipeType.Custom = 20` and `EItemType.Custom = 100` when they are missing. Runtime code should use `GameEnums.CustomRecipeTypeValue` / `CustomItemTypeValue` instead of compiling directly against injected enum members.
- Items can be declared in batches through `GameEnums.RegisterItemType(...)`, or marked one by one through `ItemProto.SetCustomItemType()`.
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
GameEnums.RegisterItemType(
    id: "example-items",
    ownerModGuid: "com.example.my-mod",
    displayName: "Example Items",
    itemIds: new[] { 9554001, 9554002 });
```

`ItemIds` points to existing items in this type. During derived-cache rebuild, DSPCore sets those items' `ItemProto.Type` to `(EItemType)GameEnums.CustomItemTypeValue` and keeps an item id -> descriptor lookup. This stable id / runtime id is a DSPCore author-layer type, not a CLR enum literal name.

When you already have an `ItemProto`, you can also mark it directly:

```csharp
itemProto.SetCustomItemType();
bool isCustom = itemProto.IsCustomItemType();
```

The direct marker only sets `ItemProto.Type`; it does not register an item id -> descriptor mapping.

## What DSPCore Does After The Call

- Registration stores descriptors by `Id`; if the same `Id` is registered more than once, the later declaration replaces the earlier one.
- During derived-cache rebuild, DSPCore assigns runtime ids for recipe types and item types, then looks up protos listed in `RecipeIds` / `ItemIds`.
- Found recipes are marked `ERecipeType.Custom`, and DSPCore maps recipe id to descriptor.
- Found items are marked with the reserved `EItemType.Custom` value, and DSPCore maps item id to descriptor.
- `ItemProto.SetCustomItemType()` directly changes the target item's `Type` field, which fits temporary or manual markers in `DataUpdates` or `DataFinalFixes`.
- `GameEnums.CanAssemblerUseRecipe(assemblerEntityId, recipeId)` reuses the same restriction check.
- `GameEnums.TryGetRecipeTypeForRecipe(...)` and `TryGetItemTypeForItem(...)` query runtime mappings.
- When an assembler window opens or refreshes the recipe picker, DSPCore records the current assembler entity and hides unsupported custom recipes during `UIRecipePicker.RefreshIcons`.
- When `AssemblerComponent.SetRecipe` is called, DSPCore still checks whether a custom recipe's current assembler entity `protoId` appears in `AssemblerItemIds`.
- Unsupported machines do not see those recipes in the picker; if external UI or another patch bypasses the picker, the recipe is still blocked at assignment time.

## What This Block Does Not Own

- It does not create `RecipeProto`; recipe creation belongs to ProtoRegistration.
- It does not create `ItemProto`; item creation belongs to ProtoRegistration / Items.
- It does not handle save migration for changed recipe ids; stable ids remain the mod author's responsibility.
- Custom item types provide stable descriptors, runtime ids, reserved enum-slot mapping, and lookup; they do not provide a complete item category UI or filtering model.
- It does not create arbitrary CLR enum literals at runtime. New literals can only be injected by the Preloader before the game assembly loads, and DSPCore currently reserves fixed values.
- It does not define a full UI category model; display and tab behavior should be handled with Tabs or concrete UI features.

## Examples

- `Examples/RecipeTypeRegistration.md`
- `Examples/RecipeTypeRegistrationExample.cs`
