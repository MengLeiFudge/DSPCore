using System.IO;
using DSPCore;

internal sealed class ExampleCounterComponent : CoreFactoryComponent
{
    private int ticks;

    public override void Update(long time, bool isActive)
    {
        if (isActive)
        {
            ticks++;
        }
    }

    public override void Export(BinaryWriter writer)
    {
        writer.Write(ticks);
    }

    public override void Import(BinaryReader reader)
    {
        ticks = reader.ReadInt32();
    }
}

internal static class EntityComponentExample
{
    public static void Register()
    {
        Components.Register<ExampleCounterComponent>(
            componentId: "com.example.counter",
            ownerModGuid: "com.example.my-mod",
            itemId: 9554);
    }
}
