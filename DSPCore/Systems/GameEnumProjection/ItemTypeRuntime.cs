using System.Collections.Generic;

namespace DSPCore;

internal static class ItemTypeRuntime
{
    private static readonly Dictionary<int, ItemTypeDescriptor> ItemToType = new();

    public static void Apply()
    {
        ItemToType.Clear();
        foreach (var descriptor in DspCore.GameEnums.GetItemTypes())
        {
            DspCore.GameEnums.GetOrAssignItemTypeRuntimeId(descriptor.Id);
            if (descriptor.ItemIds == null)
            {
                continue;
            }

            foreach (var itemId in descriptor.ItemIds)
            {
                var item = LDB.items.Select(itemId);
                if (item == null)
                {
                    DspCore.Logger?.LogWarning($"Item type {descriptor.Id} references missing item {itemId}.");
                    continue;
                }

                item.Type = (EItemType)GameEnums.CustomItemTypeValue;
                ItemToType[itemId] = descriptor;
            }
        }
    }

    public static bool TryGetItemType(int itemId, out ItemTypeDescriptor descriptor)
    {
        return ItemToType.TryGetValue(itemId, out descriptor!);
    }
}
