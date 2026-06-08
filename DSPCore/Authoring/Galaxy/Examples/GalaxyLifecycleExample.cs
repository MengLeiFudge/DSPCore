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
        GalaxySystems.RegisterGalaxy(new GalaxySystemDescriptor(
            SystemId: "com.example.galaxy",
            OwnerModGuid: "com.example.my-mod",
            Factory: static galaxy => new ExampleGalaxySystem()));
    }
}
