using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - GameEnums 当前用于声明一组配方属于自定义配方类型。
// - 它不创建 RecipeProto；配方创建应先由 ProtoRegistration 完成。
// - 它负责把已有配方和允许使用这些配方的机器关联起来。
// - 已有 ItemProto 或 item id 时，可以声明或标记 DSPCore 预留物品类型。
//
// Usage:
// - Register after recipe ids and machine item ids are known.
// - Keep RecipeIds and AssemblerItemIds stable across versions when possible.
// - Use GameEnums.RegisterItemType(...) when you need a stable item type descriptor.
// - Use ItemProto.SetCustomItemType() when you only need a direct marker.
public static class RecipeTypeRegistrationExample
{
    public static void Register()
    {
        GameEnums.RegisterRecipeType(
            id: "example.special-smelting",
            ownerModGuid: "com.example.my-mod",
            displayName: "Special Smelting",

            // 这些配方会被标记为自定义类型。
            // These recipes will be treated as a custom recipe type.
            recipeIds: new[] { 955401, 955402 },

            // 只有这些制作器/机器能使用上面的配方。
            // Only these assembler or machine item ids can use those recipes.
            assemblerItemIds: new[] { 9554 });

        GameEnums.RegisterItemType(
            id: "example.special-items",
            ownerModGuid: "com.example.my-mod",
            displayName: "Special Items",

            // 这些物品会映射到 DSPCore 预留的 EItemType.Custom 槽位。
            // These items will be mapped to DSPCore's reserved EItemType.Custom slot.
            itemIds: new[] { 955401, 955402 });
    }

    public static void MarkExistingItem(ItemProto itemProto)
    {
        itemProto.SetCustomItemType();
    }

    public static bool TryGetItemType(int itemId, out ItemTypeDescriptor itemType)
    {
        return GameEnums.TryGetItemTypeForItem(itemId, out itemType);
    }
}
