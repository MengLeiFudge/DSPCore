using DSPCore;

internal sealed class ExampleGalaxySystem : CoreGalaxySystem
{
    public override void Update(long time)
    {
        GalaxyData? galaxy = Galaxy;
    }
}

internal static class GalaxyLifecycleExample
{
    public static void Register()
    {
        GalaxySystems.RegisterGalaxy<ExampleGalaxySystem>(
            systemId: "com.example.galaxy",
            ownerModGuid: "com.example.my-mod");
    }
}
