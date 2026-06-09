using DSPCore;

internal static class BuildingParametersExample
{
    public static void Register()
    {
        Blueprints.Register(
            blockId: "com.example.mode",
            ownerModGuid: "com.example.my-mod",
            copy: static (factory, objectId) => new[] { 1 },
            paste: static (factory, entityId, data) =>
            {
                int mode = data.Length > 0 ? data[0] : 0;
            },
            canPaste: static (factory, entityId, data) => data.Length <= 4,
            applyPrebuild: static (factory, entityId, data) =>
            {
                int mode = data.Length > 0 ? data[0] : 0;
            });
    }
}
