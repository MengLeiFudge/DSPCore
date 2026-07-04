using System.Linq;
using BepInEx.Bootstrap;

namespace DSPCore;

internal static class MultiplayerRuntime
{
    private static IMultiplayerTransport? transport;

    public static bool IsNebulaAvailable { get; private set; }

    public static bool HasTransport => transport != null;

    public static void Initialize()
    {
        IsNebulaAvailable = Chainloader.PluginInfos.Keys.Any(key => key.Contains("nebula") || key.Contains("Nebula"));
    }

    public static void RegisterTransport(IMultiplayerTransport newTransport)
    {
        transport = newTransport;
        DspCore.Logger?.LogInfo($"DSPCore multiplayer transport registered: {newTransport.GetType().FullName}");
    }

    public static void UnregisterTransport(IMultiplayerTransport oldTransport)
    {
        if (ReferenceEquals(transport, oldTransport))
        {
            transport = null;
            DspCore.Logger?.LogInfo($"DSPCore multiplayer transport unregistered: {oldTransport.GetType().FullName}");
        }
    }

    public static bool SendPacket(string packetId, byte[] payload, MultiplayerSendTarget target, int targetId)
    {
        return transport?.SendPacket(packetId, payload ?? System.Array.Empty<byte>(), target, targetId) == true;
    }

    public static bool SendHostRelay(string packetId, byte[] payload)
    {
        return transport?.SendHostRelay(packetId, payload ?? System.Array.Empty<byte>()) == true;
    }

    public static bool RequestPlanetData(string requestId, int planetId)
    {
        return transport?.RequestPlanetData(requestId, planetId) == true;
    }

    public static void ApplyClientIntoOtherSave()
    {
        foreach (var descriptor in DspCore.Multiplayer.GetClientSaveInitializers())
        {
            try
            {
                descriptor.IntoOtherSave();
            }
            catch (System.Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            }
        }
    }

    public static bool DispatchPacket(string packetId, byte[] payload)
    {
        if (!DspCore.Multiplayer.TryGetPacket(packetId, out var descriptor))
        {
            return false;
        }

        try
        {
            descriptor.Handler(payload);
            return true;
        }
        catch (System.Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"{descriptor.OwnerModGuid} multiplayer packet {packetId} handler failed: {ex}");
            return true;
        }
    }

    public static bool DispatchHostRelay(string packetId, byte[] payload)
    {
        if (!DspCore.Multiplayer.TryGetRelay(packetId, out var descriptor))
        {
            return false;
        }

        try
        {
            descriptor.HandleOnHost(payload);
            return true;
        }
        catch (System.Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"{descriptor.OwnerModGuid} multiplayer host relay {packetId} handler failed: {ex}");
            return true;
        }
    }

    public static bool TryExportPlanetData(string requestId, int planetId, out byte[] payload)
    {
        payload = System.Array.Empty<byte>();
        if (!DspCore.Multiplayer.TryGetPlanetDataRequest(requestId, out var descriptor))
        {
            return false;
        }

        try
        {
            payload = descriptor.ExportPlanetData(planetId) ?? System.Array.Empty<byte>();
            return true;
        }
        catch (System.Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"{descriptor.OwnerModGuid} multiplayer planet data {requestId} export failed: {ex}");
            return true;
        }
    }

    public static bool ImportPlanetData(string requestId, int planetId, byte[] payload)
    {
        if (!DspCore.Multiplayer.TryGetPlanetDataRequest(requestId, out var descriptor))
        {
            return false;
        }

        try
        {
            descriptor.ImportPlanetData(planetId, payload);
            return true;
        }
        catch (System.Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"{descriptor.OwnerModGuid} multiplayer planet data {requestId} import failed: {ex}");
            return true;
        }
    }
}
