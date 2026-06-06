# Recipe Types

The RecipeTypes block lets a mod classify existing recipes as custom recipe types and declare which machines may use them. It is not a recipe creation tool; it is a classification and runtime guard layer after recipe creation.

## What This Block Gives You

- You do not need every mod to patch assembler recipe-setting logic itself.
- A set of custom recipes can declare one type id, display name, recipe ids, and allowed machines.
- DSPCore marks target recipes as `ERecipeType.Custom` after proto cache rebuilds.
- Runtime blocks unsupported assemblers from setting those recipes, reducing cases where a recipe is used by the wrong machine.

## Capability: Declare Custom Recipe Types

```csharp
RecipeTypes.Register(new RecipeTypeDescriptor(
    Id: "example-smelting",
    OwnerModGuid: "com.example.my-mod",
    DisplayName: "ExampleSmelting",
    RecipeIds: new[] { 9554001, 9554002 },
    AssemblerItemIds: new[] { 2303 }));
```

`RecipeIds` points to existing recipes in this type. If `AssemblerItemIds` is empty or null, current runtime does not restrict machines. If it is non-empty, only assemblers whose item proto id is listed may use this recipe type.

## What DSPCore Does After The Call

- Registration stores descriptors by `Id`; if the same `Id` is registered more than once, the later declaration replaces the earlier one.
- During derived-cache rebuild, DSPCore assigns a runtime id for each type and looks up recipes listed in `RecipeIds`.
- Found recipes are marked `ERecipeType.Custom`, and DSPCore maps recipe id to descriptor.
- When `AssemblerComponent.SetRecipe` is called, DSPCore checks whether a custom recipe's current assembler entity `protoId` appears in `AssemblerItemIds`.
- Unsupported machines are blocked from setting that recipe.

## What This Block Does Not Own

- It does not create `RecipeProto`; recipe creation belongs to ProtoRegistration.
- It does not hide invalid recipes before the recipe picker opens; current protection happens at `SetRecipe`.
- It does not handle save migration for changed recipe ids; stable ids remain the mod author's responsibility.
- It does not define a full UI category model; display and tab behavior should be handled with Tabs, Pickers, or concrete UI features.

## Examples

- `Examples/RecipeTypeRegistration.md`
- `Examples/RecipeTypeRegistrationExample.cs`
