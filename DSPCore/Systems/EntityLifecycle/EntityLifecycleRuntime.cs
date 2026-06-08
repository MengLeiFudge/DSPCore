using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace DSPCore;

internal static class EntityLifecycleRuntime
{
    private const int SaveVersion = 1;
    private const string SaveKey = "DSPCore.EntityLifecycle";
    private static readonly Dictionary<int, Dictionary<int, Dictionary<string, CoreFactoryComponent>>> ComponentsByPlanet = new();
    private static readonly Dictionary<int, List<PendingComponentState>> PendingByPlanet = new();

    public static void Initialize()
    {
        DspCore.Saves.Register(SaveKey, new SaveHandler(), CoreLoadOrder.Postload);
    }

    public static bool TryGet<T>(PlanetFactory factory, int entityId, out T component)
        where T : CoreFactoryComponent
    {
        component = null!;
        if (factory?.planet == null)
        {
            return false;
        }

        if (!ComponentsByPlanet.TryGetValue(factory.planet.id, out var byEntity) ||
            !byEntity.TryGetValue(entityId, out var byComponent))
        {
            return false;
        }

        foreach (var candidate in byComponent.Values)
        {
            if (candidate is T typed)
            {
                component = typed;
                return true;
            }
        }

        return false;
    }

    public static void EnsureFactory(PlanetFactory factory)
    {
        if (factory?.planet == null)
        {
            return;
        }

        if (!ComponentsByPlanet.ContainsKey(factory.planet.id))
        {
            ComponentsByPlanet[factory.planet.id] = new Dictionary<int, Dictionary<string, CoreFactoryComponent>>();
        }

        ApplyPending(factory);
    }

    public static void AttachComponents(PlanetFactory factory, int entityId, PrefabDesc desc, int prebuildId)
    {
        if (factory?.planet == null || entityId <= 0 || desc == null || factory.entityPool[entityId].id != entityId)
        {
            return;
        }

        EnsureFactory(factory);
        var byEntity = ComponentsByPlanet[factory.planet.id];
        if (!byEntity.TryGetValue(entityId, out var byComponent))
        {
            byComponent = new Dictionary<string, CoreFactoryComponent>(StringComparer.Ordinal);
            byEntity[entityId] = byComponent;
        }

        foreach (var descriptor in DspCore.Components.GetAll())
        {
            if (byComponent.ContainsKey(descriptor.ComponentId) || !descriptor.Matches(factory, entityId, desc))
            {
                continue;
            }

            var component = CreateComponent(descriptor, factory, entityId, desc, prebuildId);
            if (component == null)
            {
                continue;
            }

            byComponent[descriptor.ComponentId] = component;
            ImportOrInitialize(factory.planet.id, entityId, descriptor.ComponentId, component);
            InjectedFieldAccess.SetEntityMarker(factory, entityId, descriptor.ComponentId);
            SafeCall(descriptor.ComponentId, () => component.OnAdded(prebuildId));
        }

        if (byComponent.Count == 0)
        {
            byEntity.Remove(entityId);
        }
    }

    public static void RemoveComponents(PlanetFactory factory, int entityId)
    {
        if (factory?.planet == null ||
            !ComponentsByPlanet.TryGetValue(factory.planet.id, out var byEntity) ||
            !byEntity.TryGetValue(entityId, out var byComponent))
        {
            return;
        }

        foreach (var component in byComponent.Values.ToArray())
        {
            SafeCall(component.ComponentId, component.OnRemoved);
        }

        byEntity.Remove(entityId);
    }

    public static void PowerUpdate(PlanetFactory factory, long time, bool isActive)
    {
        ForEach(factory, component => component.PowerUpdate(time, isActive));
    }

    public static void Update(PlanetFactory factory, long time, bool isActive)
    {
        ForEach(factory, component => component.Update(time, isActive));
    }

    public static void PostUpdate(PlanetFactory factory, long time)
    {
        ForEach(factory, component => component.PostUpdate(time));
    }

    private static CoreFactoryComponent? CreateComponent(ComponentDescriptor descriptor, PlanetFactory factory, int entityId, PrefabDesc desc, int prebuildId)
    {
        try
        {
            var component = descriptor.Factory(factory, entityId, desc, prebuildId);
            component.InitializeContext(descriptor.ComponentId, factory, entityId);
            return component;
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"Failed to create DSPCore component {descriptor.ComponentId}: {ex}");
            return null;
        }
    }

    private static void ImportOrInitialize(int planetId, int entityId, string componentId, CoreFactoryComponent component)
    {
        if (PendingByPlanet.TryGetValue(planetId, out var pending))
        {
            var index = pending.FindIndex(item => item.EntityId == entityId && item.ComponentId == componentId);
            if (index >= 0)
            {
                var state = pending[index];
                pending.RemoveAt(index);
                SafeCall(componentId, () =>
                {
                    using var stream = new MemoryStream(state.Data);
                    using var reader = new BinaryReader(stream);
                    component.Import(reader);
                });
                return;
            }
        }

        SafeCall(componentId, component.IntoOtherSave);
    }

    private static void ApplyPending(PlanetFactory factory)
    {
        var planetId = factory.planet.id;
        if (!PendingByPlanet.TryGetValue(planetId, out var pending) || pending.Count == 0)
        {
            return;
        }

        foreach (var state in pending.ToArray())
        {
            var entityId = state.EntityId;
            if (entityId <= 0 || entityId >= factory.entityPool.Length || factory.entityPool[entityId].id != entityId)
            {
                continue;
            }

            var model = LDB.models.Select(factory.entityPool[entityId].modelIndex);
            var desc = model?.prefabDesc;
            if (desc == null || !DspCore.Components.TryGet(state.ComponentId, out var descriptor))
            {
                continue;
            }

            var component = CreateComponent(descriptor, factory, entityId, desc, 0);
            if (component == null)
            {
                continue;
            }

            var byEntity = ComponentsByPlanet[planetId];
            if (!byEntity.TryGetValue(entityId, out var byComponent))
            {
                byComponent = new Dictionary<string, CoreFactoryComponent>(StringComparer.Ordinal);
                byEntity[entityId] = byComponent;
            }

            byComponent[state.ComponentId] = component;
            ImportOrInitialize(planetId, entityId, state.ComponentId, component);
            InjectedFieldAccess.SetEntityMarker(factory, entityId, state.ComponentId);
            SafeCall(state.ComponentId, () => component.OnAdded(0));
        }
    }

    private static void ForEach(PlanetFactory factory, Action<CoreFactoryComponent> action)
    {
        if (factory?.planet == null ||
            !ComponentsByPlanet.TryGetValue(factory.planet.id, out var byEntity))
        {
            return;
        }

        foreach (var component in byEntity.Values.SelectMany(item => item.Values).ToArray())
        {
            SafeCall(component.ComponentId, () => action(component));
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
            DspCore.Logger?.LogError($"DSPCore entity component callback failed for {owner}: {ex}");
        }
    }

    private sealed class SaveHandler : ICoreSaveHandler
    {
        public void Export(BinaryWriter writer)
        {
            var states = ComponentsByPlanet
                .SelectMany(planet => planet.Value.SelectMany(entity => entity.Value.Values.Select(component => (PlanetId: planet.Key, EntityId: entity.Key, Component: component))))
                .ToArray();

            writer.Write(SaveVersion);
            writer.Write(states.Length);
            foreach (var state in states)
            {
                using var stream = new MemoryStream();
                using (var blockWriter = new BinaryWriter(stream))
                {
                    state.Component.Export(blockWriter);
                }

                var data = stream.ToArray();
                writer.Write(state.PlanetId);
                writer.Write(state.EntityId);
                writer.Write(state.Component.ComponentId);
                writer.Write(data.Length);
                writer.Write(data);
            }
        }

        public void Import(BinaryReader reader)
        {
            ComponentsByPlanet.Clear();
            PendingByPlanet.Clear();
            var version = reader.ReadInt32();
            if (version > SaveVersion)
            {
                DspCore.Logger?.LogWarning($"DSPCore entity lifecycle save version {version} is newer than runtime version {SaveVersion}.");
            }

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var state = new PendingComponentState(reader.ReadInt32(), reader.ReadInt32(), reader.ReadString(), reader.ReadBytes(reader.ReadInt32()));
                if (!PendingByPlanet.TryGetValue(state.PlanetId, out var pending))
                {
                    pending = new List<PendingComponentState>();
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
            ComponentsByPlanet.Clear();
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

    private sealed record PendingComponentState(int PlanetId, int EntityId, string ComponentId, byte[] Data);
}

internal static class EntityLifecycleRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.CreateEntityLogicComponents))]
    private static void CreateEntityLogicComponents(PlanetFactory __instance, int entityId, PrefabDesc desc, int prebuildId)
    {
        EntityLifecycleRuntime.AttachComponents(__instance, entityId, desc, prebuildId);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.RemoveEntityWithComponents))]
    private static void RemoveEntityWithComponents(PlanetFactory __instance, int id)
    {
        EntityLifecycleRuntime.RemoveComponents(__instance, id);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.GetOrCreateFactory))]
    private static void GetOrCreateFactory(PlanetFactory __result)
    {
        EntityLifecycleRuntime.EnsureFactory(__result);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PowerSystem), nameof(PowerSystem.GameTick))]
    private static void PowerSystemGameTick(PowerSystem __instance, long time, bool isActive)
    {
        EntityLifecycleRuntime.PowerUpdate(__instance.factory, time, isActive);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FactorySystem), nameof(FactorySystem.GameTick))]
    private static void FactorySystemGameTick(FactorySystem __instance, long time, bool isActive)
    {
        EntityLifecycleRuntime.Update(__instance.factory, time, isActive);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FactorySystem), nameof(FactorySystem.GameTickLabOutputToNext))]
    private static void FactorySystemGameTickLabOutputToNext(FactorySystem __instance, long time)
    {
        EntityLifecycleRuntime.PostUpdate(__instance.factory, time);
    }
}
