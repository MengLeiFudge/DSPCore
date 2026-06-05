using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace DSPCore;

internal static class RecipeTypeRuntime
{
    private static readonly Dictionary<int, RecipeTypeDescriptor> RecipeToType = new();

    public static void Apply()
    {
        RecipeToType.Clear();
        foreach (var descriptor in DspCore.RecipeTypes.GetAll())
        {
            DspCore.RecipeTypes.GetOrAssignRuntimeId(descriptor.Id);
            if (descriptor.RecipeIds == null)
            {
                continue;
            }

            foreach (var recipeId in descriptor.RecipeIds)
            {
                var recipe = LDB.recipes.Select(recipeId);
                if (recipe == null)
                {
                    DspCore.Logger?.LogWarning($"Recipe type {descriptor.Id} references missing recipe {recipeId}.");
                    continue;
                }

                recipe.Type = ERecipeType.Custom;
                RecipeToType[recipeId] = descriptor;
            }
        }
    }

    public static bool CanAssemblerUseRecipe(int assemblerEntityId, int recipeId)
    {
        if (!RecipeToType.TryGetValue(recipeId, out var descriptor))
        {
            return true;
        }

        if (descriptor.AssemblerItemIds == null || descriptor.AssemblerItemIds.Count == 0)
        {
            return true;
        }

        var factory = GameMain.localPlanet?.factory;
        if (factory == null || assemblerEntityId <= 0)
        {
            return true;
        }

        var protoId = factory.entityPool[assemblerEntityId].protoId;
        return descriptor.AssemblerItemIds.Contains(protoId);
    }
}

internal static class RecipeTypeRuntimePatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AssemblerComponent), "SetRecipe")]
    private static bool SetRecipe(ref AssemblerComponent __instance, int recpId)
    {
        if (recpId <= 0)
        {
            return true;
        }

        return RecipeTypeRuntime.CanAssemblerUseRecipe(__instance.entityId, recpId);
    }
}
