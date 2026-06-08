using DSPCore;

internal static class BuildingParametersExample
{
    public static void Register()
    {
        Blueprints.Register(new BuildingParameterDescriptor(
            blockId: "com.example.mode",
            ownerModGuid: "com.example.my-mod",
            copy: static (factory, objectId) => new BuildingParameterBlock("com.example.mode", new[] { 1 }),
            paste: static (factory, entityId, block) =>
            {
                int mode = block.Data.Length > 0 ? block.Data[0] : 0;
            },
            canPaste: static (factory, entityId, block) => block.Data.Length <= 4,
            applyPrebuild: static (factory, entityId, block) =>
            {
                int mode = block.Data.Length > 0 ? block.Data[0] : 0;
            }));
    }
}
