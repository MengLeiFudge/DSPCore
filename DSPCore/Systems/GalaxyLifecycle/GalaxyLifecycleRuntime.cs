using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace DSPCore;

internal static class GalaxyLifecycleRuntime
{
    private const int SaveVersion = 1;
    private const string SaveKey = "DSPCore.GalaxyLifecycle";
    private static readonly Dictionary<int, Dictionary<string, CoreStarSystem>> StarSystemsByStar = new();
    private static readonly Dictionary<string, CoreGalaxySystem> GalaxySystems = new(StringComparer.Ordinal);
    private static readonly Dictionary<int, List<PendingSystemState>> PendingStars = new();
    private static readonly List<PendingSystemState> PendingGalaxy = new();

    public static void Initialize()
    {
        DspCore.Saves.Register(SaveKey, new SaveHandler(), CoreLoadOrder.Postload);
    }

    public static void EnsureGalaxy(GalaxyData galaxy)
    {
        if (galaxy == null)
        {
            return;
        }

        foreach (var descriptor in DspCore.GalaxySystems.GetAll())
        {
            if (GalaxySystems.ContainsKey(descriptor.SystemId))
            {
                continue;
            }

            var system = CreateGalaxySystem(descriptor, galaxy);
            if (system == null)
            {
                continue;
            }

            GalaxySystems[descriptor.SystemId] = system;
            ImportOrInitializeGalaxy(descriptor.SystemId, system);
            SafeCall(descriptor.SystemId, system.Init);
        }

        if (galaxy.stars == null)
        {
            return;
        }

        foreach (var star in galaxy.stars)
        {
            EnsureStar(star);
        }
    }

    public static void Update(GalaxyData galaxy, long time)
    {
        EnsureGalaxy(galaxy);
        foreach (var system in GalaxySystems.Values.ToArray())
        {
            SafeCall(system.SystemId, () => system.Update(time));
        }

        foreach (var system in StarSystemsByStar.Values.SelectMany(item => item.Values).ToArray())
        {
            SafeCall(system.SystemId, () => system.Update(time));
        }
    }

    private static void EnsureStar(StarData star)
    {
        if (star == null)
        {
            return;
        }

        if (!StarSystemsByStar.TryGetValue(star.id, out var bySystem))
        {
            bySystem = new Dictionary<string, CoreStarSystem>(StringComparer.Ordinal);
            StarSystemsByStar[star.id] = bySystem;
        }

        foreach (var descriptor in DspCore.StarSystems.GetAll())
        {
            if (bySystem.ContainsKey(descriptor.SystemId))
            {
                continue;
            }

            var system = CreateStarSystem(descriptor, star);
            if (system == null)
            {
                continue;
            }

            bySystem[descriptor.SystemId] = system;
            ImportOrInitializeStar(star.id, descriptor.SystemId, system);
            SafeCall(descriptor.SystemId, system.Init);
        }
    }

    private static CoreStarSystem? CreateStarSystem(StarSystemDescriptor descriptor, StarData star)
    {
        try
        {
            var system = descriptor.Factory(star);
            system.InitializeContext(descriptor.SystemId, star);
            return system;
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            return null;
        }
    }

    private static CoreGalaxySystem? CreateGalaxySystem(GalaxySystemDescriptor descriptor, GalaxyData galaxy)
    {
        try
        {
            var system = descriptor.Factory(galaxy);
            system.InitializeContext(descriptor.SystemId, galaxy);
            return system;
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            return null;
        }
    }

    private static void ImportOrInitializeStar(int starId, string systemId, CoreStarSystem system)
    {
        if (PendingStars.TryGetValue(starId, out var pending))
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

    private static void ImportOrInitializeGalaxy(string systemId, CoreGalaxySystem system)
    {
        var index = PendingGalaxy.FindIndex(item => item.SystemId == systemId);
        if (index >= 0)
        {
            var state = PendingGalaxy[index];
            PendingGalaxy.RemoveAt(index);
            SafeCall(systemId, () =>
            {
                using var stream = new MemoryStream(state.Data);
                using var reader = new BinaryReader(stream);
                system.Import(reader);
            });
            return;
        }

        SafeCall(systemId, system.IntoOtherSave);
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
            DspCore.Logger?.LogError($"DSPCore galaxy lifecycle callback failed for {owner}: {ex}");
        }
    }

    private sealed class SaveHandler : ICoreSaveHandler
    {
        public void Export(BinaryWriter writer)
        {
            var starStates = StarSystemsByStar
                .SelectMany(star => star.Value.Values.Select(system => (StarId: star.Key, System: system)))
                .ToArray();
            var galaxyStates = GalaxySystems.Values.ToArray();

            writer.Write(SaveVersion);
            writer.Write(galaxyStates.Length);
            foreach (var system in galaxyStates)
            {
                WriteSystem(writer, 0, system.SystemId, system.Export);
            }

            writer.Write(starStates.Length);
            foreach (var state in starStates)
            {
                WriteSystem(writer, state.StarId, state.System.SystemId, state.System.Export);
            }
        }

        public void Import(BinaryReader reader)
        {
            StarSystemsByStar.Clear();
            GalaxySystems.Clear();
            PendingStars.Clear();
            PendingGalaxy.Clear();

            var version = reader.ReadInt32();
            if (version > SaveVersion)
            {
                DspCore.Logger?.LogWarning($"DSPCore galaxy lifecycle save version {version} is newer than runtime version {SaveVersion}.");
            }

            var galaxyCount = reader.ReadInt32();
            for (var i = 0; i < galaxyCount; i++)
            {
                PendingGalaxy.Add(ReadSystem(reader));
            }

            var starCount = reader.ReadInt32();
            for (var i = 0; i < starCount; i++)
            {
                var state = ReadSystem(reader);
                if (!PendingStars.TryGetValue(state.OwnerId, out var pending))
                {
                    pending = new List<PendingSystemState>();
                    PendingStars[state.OwnerId] = pending;
                }

                pending.Add(state);
            }

            if (GameMain.data?.galaxy != null)
            {
                EnsureGalaxy(GameMain.data.galaxy);
            }
        }

        public void IntoOtherSave()
        {
            StarSystemsByStar.Clear();
            GalaxySystems.Clear();
            PendingStars.Clear();
            PendingGalaxy.Clear();
            if (GameMain.data?.galaxy != null)
            {
                EnsureGalaxy(GameMain.data.galaxy);
            }
        }

        private static void WriteSystem(BinaryWriter writer, int ownerId, string systemId, Action<BinaryWriter> export)
        {
            using var stream = new MemoryStream();
            using (var blockWriter = new BinaryWriter(stream))
            {
                export(blockWriter);
            }

            var data = stream.ToArray();
            writer.Write(ownerId);
            writer.Write(systemId);
            writer.Write(data.Length);
            writer.Write(data);
        }

        private static PendingSystemState ReadSystem(BinaryReader reader)
        {
            return new PendingSystemState(reader.ReadInt32(), reader.ReadString(), reader.ReadBytes(reader.ReadInt32()));
        }
    }

    private sealed record PendingSystemState(int OwnerId, string SystemId, byte[] Data);
}

internal static class GalaxyLifecycleRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.SetForNewGame))]
    private static void SetForNewGame(GameData __instance)
    {
        GalaxyLifecycleRuntime.EnsureGalaxy(__instance.galaxy);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpaceSector), nameof(SpaceSector.GameTick))]
    private static void SpaceSectorGameTick(SpaceSector __instance, long time)
    {
        GalaxyLifecycleRuntime.Update(__instance.galaxy, time);
    }
}
