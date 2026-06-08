using DSPCore;

internal static class CommonNetworkExample
{
    public static void Register()
    {
        Networks.Register(new NetworkDescriptor(
            NetworkId: "com.example.power-group",
            OwnerModGuid: "com.example.my-mod",
            TryGetCommonNetwork: static (factory, entityA, entityB) =>
            {
                if (entityA <= 0 || entityB <= 0)
                {
                    return null;
                }

                return 1;
            },
            IsConnectedToNetwork: static (factory, entityId, networkId) => entityId > 0 && networkId == 1));
    }
}
