using DSPCore;

namespace ExampleMod;

public static class IconSetExample
{
    public static void Register()
    {
        Icons.Register(new IconDescriptor(
            Id: "example-machine",
            OwnerModGuid: "com.example.my-mod",
            AssetPath: "assets/icons/example-machine.png",
            FallbackIconId: "default-machine",
            TargetKind: ProtoKind.Item,
            TargetProtoId: 9554));
    }
}
