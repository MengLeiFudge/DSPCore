using DSPCore;

internal static class CommonNetworkExample
{
    public static void Register()
    {
        Networks.Register(
            networkId: "com.example.power-group",
            ownerModGuid: "com.example.my-mod",
            tryGetCommonNetwork: static (factory, entityA, entityB) =>
            {
                if (entityA <= 0 || entityB <= 0)
                {
                    return null;
                }

                return 1;
            },
            isConnectedToNetwork: static (factory, entityId, networkId) => entityId > 0 && networkId == 1);
    }
}
