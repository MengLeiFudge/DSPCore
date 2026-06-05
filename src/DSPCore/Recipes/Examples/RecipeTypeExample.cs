using DSPCore;

namespace ExampleMod;

public static class RecipeTypeExample
{
    public static void Register()
    {
        RecipeTypes.Register(new RecipeTypeDescriptor(
            Id: "example.special-smelting",
            OwnerModGuid: "com.example.my-mod",
            DisplayName: "Special Smelting",
            RecipeIds: new[] { 955401, 955402 },
            AssemblerItemIds: new[] { 9554 }));
    }
}
