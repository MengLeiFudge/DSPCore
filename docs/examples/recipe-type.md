# Recipe Type / 自定义配方类型

Register a custom recipe type when a group of recipes should only be available on specific machines.

当一组配方只能在指定机器中使用时，注册自定义配方类型。

```csharp
using DSPCore;

DspCore.RecipeTypes.Register(new RecipeTypeDescriptor(
    Id: "example.special-smelting",
    OwnerModGuid: "com.example.my-mod",
    DisplayName: "Special Smelting",
    RecipeIds: new[] { 955401, 955402 },
    AssemblerItemIds: new[] { 9554 }));
```

DSPCore marks declared recipes as `ERecipeType.Custom` and blocks unsupported assembler machines from accepting those recipe ids.

DSPCore 会把声明的配方标记为 `ERecipeType.Custom`，并阻止不支持的制作器接受这些配方 ID。

Current runtime note: unsupported selection is blocked in `AssemblerComponent.SetRecipe`; the assembler recipe picker list is not yet filtered before selection.

当前运行时说明：不支持的选择会在 `AssemblerComponent.SetRecipe` 中被阻断；制作器配方选择列表尚未在选择前过滤。
