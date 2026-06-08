using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace DSPCore;

internal static class PlanetLifecycleRuntime
{
    private const int SaveVersion = 1;
    private const string SaveKey = "DSPCore.PlanetLifecycle";
    private static readonly Dictionary<int, Dictionary<string, CorePlanetSystem>> SystemsByPlanet = new();
    private static readonly Dictionary<int, List<PendingSystemState>> PendingByPlanet = new();

    public static void Initialize()
    {
        DspCore.Saves.Register(SaveKey, new SaveHandler(), CoreLoadOrder.Postload);
    }

    public static bool TryGet<T>(PlanetFactory factory, string systemId, out T system)
        where T : CorePlanetSystem
    {
        system = null!;
        if (factory?.planet == null ||
            !SystemsByPlanet.TryGetValue(factory.planet.id, out var bySystem) ||
            !bySystem.TryGetValue(systemId, out var candidate) ||
            candidate is not T typed)
        {
            return false;
        }

        system = typed;
        return true;
    }

    public static void EnsureFactory(PlanetFactory factory)
    {
        if (factory?.planet == null)
        {
            return;
        }

        var planetId = factory.planet.id;
        if (!SystemsByPlanet.TryGetValue(planetId, out var bySystem))
        {
            bySystem = new Dictionary<string, CorePlanetSystem>(StringComparer.Ordinal);
            SystemsByPlanet[planetId] = bySystem;
        }

        foreach (var descriptor in DspCore.PlanetSystems.GetAll())
        {
            if (bySystem.ContainsKey(descriptor.SystemId))
            {
                continue;
            }

            var system = CreateSystem(descriptor, factory);
            if (system == null)
            {
                continue;
            }

            bySystem[descriptor.SystemId] = system;
            ImportOrInitialize(planetId, descriptor.SystemId, system);
            SafeCall(descriptor.SystemId, system.Init);
        }
    }

    public static void DrawUpdate(PlanetFactory factory)
    {
        ForEach(factory, system => system.DrawUpdate());
    }

    public static void PowerUpdate(PlanetFactory factory, long time, bool isActive)
    {
        ForEach(factory, system => system.PowerUpdate(time, isActive));
    }

    public static void PreUpdate(PlanetFactory factory, long time, bool isActive)
    {
        ForEach(factory, system => system.PreUpdate(time, isActive));
    }

    public static void Update(PlanetFactory factory, long time, bool isActive)
    {
        ForEach(factory, system => system.Update(time, isActive));
    }

    public static void PostUpdate(PlanetFactory factory, long time)
    {
        ForEach(factory, system => system.PostUpdate(time));
    }

    private static CorePlanetSystem? CreateSystem(PlanetSystemDescriptor descriptor, PlanetFactory factory)
    {
        try
        {
            var system = descriptor.Factory(factory);
            system.InitializeContext(descriptor.SystemId, factory);
            return system;
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"Failed to create DSPCore planet system {descriptor.SystemId}: {ex}");
            return null;
        }
    }

    private static void ImportOrInitialize(int planetId, string systemId, CorePlanetSystem system)
    {
        if (PendingByPlanet.TryGetValue(planetId, out var pending))
        {
            var index = pending.FindIndex(item => item.SystemId == systemId);
            if (index >= 0)
            {
                var state = pending[index];
                pending.RemoveAt(index);
                SafeCall(systemId, () =>
                {
                    using var stream = new MemoryStream(state.Data);
                    using var reader = new BinaryReader(stream);
                    system.Import(reader);
                });
                return;
            }
        }

        SafeCall(systemId, system.IntoOtherSave);
    }

    private static void ForEach(PlanetFactory factory, Action<CorePlanetSystem> action)
    {
        if (factory?.planet == null ||
            !SystemsByPlanet.TryGetValue(factory.planet.id, out var bySystem))
        {
            return;
        }

        foreach (var system in bySystem.Values.ToArray())
        {
            SafeCall(system.SystemId, () => action(system));
        }
    }

    private static void SafeCall(string owner, Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(owner, ex);
            DspCore.Logger?.LogError($"DSPCore planet system callback failed for {owner}: {ex}");
        }
    }

    private sealed class SaveHandler : ICoreSaveHandler
    {
        public void Export(BinaryWriter writer)
        {
            var states = SystemsByPlanet
                .SelectMany(planet => planet.Value.Values.Select(system => (PlanetId: planet.Key, System: system)))
                .ToArray();

            writer.Write(SaveVersion);
            writer.Write(states.Length);
            foreach (var state in states)
            {
                using var stream = new MemoryStream();
                using (var blockWriter = new BinaryWriter(stream))
                {
                    state.System.Export(blockWriter);
                }

                var data = stream.ToArray();
                writer.Write(state.PlanetId);
                writer.Write(state.System.SystemId);
                writer.Write(data.Length);
                writer.Write(data);
            }
        }

        public void Import(BinaryReader reader)
        {
            SystemsByPlanet.Clear();
            PendingByPlanet.Clear();
            var version = reader.ReadInt32();
            if (version > SaveVersion)
            {
                DspCore.Logger?.LogWarning($"DSPCore planet lifecycle save version {version} is newer than runtime version {SaveVersion}.");
            }

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var state = new PendingSystemState(reader.ReadInt32(), reader.ReadString(), reader.ReadBytes(reader.ReadInt32()));
                if (!PendingByPlanet.TryGetValue(state.PlanetId, out var pending))
                {
                    pending = new List<PendingSystemState>();
                    PendingByPlanet[state.PlanetId] = pending;
                }

                pending.Add(state);
            }

            foreach (var factory in LoadedFactories())
            {
                EnsureFactory(factory);
            }
        }

        public void IntoOtherSave()
        {
            SystemsByPlanet.Clear();
            PendingByPlanet.Clear();
        }
    }

    private static IEnumerable<PlanetFactory> LoadedFactories()
    {
        var data = GameMain.data;
        if (data?.factories == null)
        {
            yield break;
        }

        for (var i = 0; i < data.factoryCount; i++)
        {
            var factory = data.factories[i];
            if (factory != null)
            {
                yield return factory;
            }
        }
    }

    private sealed record PendingSystemState(int PlanetId, string SystemId, byte[] Data);
}

internal static class PlanetLifecycleRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.GetOrCreateFactory))]
    private static void GetOrCreateFactory(PlanetFactory __result)
    {
        PlanetLifecycleRuntime.EnsureFactory(__result);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FactoryModel), "OnCameraPostRender")]
    private static void FactoryModelOnCameraPostRender(FactoryModel __instance)
    {
        if (__instance.planet?.factory != null)
        {
            PlanetLifecycleRuntime.DrawUpdate(__instance.planet.factory);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PowerSystem), nameof(PowerSystem.GameTick))]
    private static void PowerSystemGameTick(PowerSystem __instance, long time, bool isActive)
    {
        PlanetLifecycleRuntime.PowerUpdate(__instance.factory, time, isActive);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FactorySystem), nameof(FactorySystem.GameTick))]
    private static void FactorySystemPreGameTick(FactorySystem __instance, long time, bool isActive)
    {
        PlanetLifecycleRuntime.PreUpdate(__instance.factory, time, isActive);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FactorySystem), nameof(FactorySystem.GameTick))]
    private static void FactorySystemPostGameTick(FactorySystem __instance, long time, bool isActive)
    {
        PlanetLifecycleRuntime.Update(__instance.factory, time, isActive);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FactorySystem), nameof(FactorySystem.GameTickLabOutputToNext))]
    private static void FactorySystemGameTickLabOutputToNext(FactorySystem __instance, long time)
    {
        PlanetLifecycleRuntime.PostUpdate(__instance.factory, time);
    }
}
