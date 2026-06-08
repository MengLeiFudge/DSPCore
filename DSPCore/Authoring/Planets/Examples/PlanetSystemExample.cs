using DSPCore;

internal sealed class ExamplePlanetSystem : CorePlanetSystem
{
    public override void PostUpdate(long time)
    {
        PlanetFactory? factory = Factory;
        if (factory == null)
        {
            return;
        }

        // Keep planet-wide cached state here.
    }
}

internal static class PlanetSystemExample
{
    public static void Register()
    {
        PlanetSystems.Register(new PlanetSystemDescriptor(
            systemId: "com.example.planet-cache",
            ownerModGuid: "com.example.my-mod",
            factory: static factory => new ExamplePlanetSystem()));
    }
}
