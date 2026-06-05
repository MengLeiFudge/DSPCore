using DSPCore;
using static DSPCore.DspCore;

namespace ExampleMod;

public static class ProtoPhasesExample
{
    public static void Register(ItemProto itemProto, RecipeProto recipeProto, TutorialProto tutorialProto)
    {
        Protos.RegisterItem(
            proto: itemProto,
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.Data,
            purpose: "Declare the base item");

        Protos.RegisterRecipe(
            proto: recipeProto,
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.DataUpdates,
            purpose: "Attach recipe after item declarations");

        Protos.RegisterTutorial(
            proto: tutorialProto,
            ownerModGuid: "com.example.my-mod",
            phase: CoreDataPhase.DataFinalFixes,
            purpose: "Final tutorial chain fix");
    }
}
