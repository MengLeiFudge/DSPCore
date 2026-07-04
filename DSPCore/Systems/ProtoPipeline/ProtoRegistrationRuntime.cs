using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace DSPCore;

internal static class ProtoRegistrationRuntime
{
    private static readonly HashSet<CoreDataPhase> AppliedPhases = new();

    public static void ApplyPhase(CoreDataPhase phase)
    {
        if (!AppliedPhases.Add(phase))
        {
            return;
        }

        ExecutePhaseActions(phase);

        var registrations = DspCore.ProtoRegistration.GetByPhase(phase);
        if (registrations.Count == 0)
        {
            return;
        }

        var grouped = registrations
            .Where(item => item.Proto is Proto)
            .GroupBy(item => GetConcreteProtoType(item.ProtoType, item.Proto.GetType()));

        foreach (var group in grouped)
        {
            try
            {
                AddProtoGroup(group.Key, group.Select(item => (Proto)item.Proto).ToList(), phase);
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException("DSPCore.ProtoRegistrationRuntime", ex);
                DspCore.Logger?.LogError($"Failed to apply {phase} proto group {group.Key.FullName}: {ex}");
            }
        }
    }

    private static void ExecutePhaseActions(CoreDataPhase phase)
    {
        foreach (var action in DspCore.ProtoRegistration.GetActionsByPhase(phase))
        {
            try
            {
                var context = new ProtoPhaseContext(action.OwnerModGuid, phase, DspCore.ProtoRegistration);
                action.Configure(context);
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(action.OwnerModGuid, ex);
                DspCore.Logger?.LogError($"Failed to execute {phase} proto phase action from {action.OwnerModGuid}: {ex}");
            }
        }
    }

    public static void RebuildDerivedCaches()
    {
        RunCacheStep("LDB data indices", RebuildAllDataIndices);
        RunCacheStep("Item fuel needs", ItemProto.InitFuelNeeds);
        RunCacheStep("Item turret needs", ItemProto.InitTurretNeeds);
        RunCacheStep("Item fluids", ItemProto.InitFluids);
        RunCacheStep("Item turrets", ItemProto.InitTurrets);
        RunCacheStep("Item enemy drop tables", ItemProto.InitEnemyDropTables);
        RunCacheStep("Item constructable items", ItemProto.InitConstructableItems);
        RunCacheStep("Item IDs", ItemProto.InitItemIds);
        RunCacheStep("Item indices", ItemProto.InitItemIndices);
        RunCacheStep("Mecha materials", ItemProto.InitMechaMaterials);
        RunItemPrefabCacheStep("Fighter indices", ItemProto.InitFighterIndices);
        RunItemPrefabCacheStep("Power facility indices", ItemProto.InitPowerFacilityIndices);
        RunItemPrefabCacheStep("Production mask", ItemProto.InitProductionMask);
        RunCacheStep("Model descriptors", ModelRuntime.Apply);
        RunCacheStep("Max model index", ModelProto.InitMaxModelIndex);
        RunCacheStep("Model indices", ModelProto.InitModelIndices);
        RunCacheStep("Model orders", ModelProto.InitModelOrders);
        RunCacheStep("PrefabDesc array", ModelRuntime.RebuildPrefabDescArray);
        RunCacheStep("Recipe items", RecipeProto.InitRecipeItems);
        RunCacheStep("Fractionator needs", RecipeProto.InitFractionatorNeeds);
        RunCacheStep("Custom item types", ItemTypeRuntime.Apply);
        RunCacheStep("Custom recipe types", RecipeTypeRuntime.Apply);
        RunCacheStep("Recipe execute data", RebuildRecipeExecuteData);
        RunCacheStep("Signal key-id pairs", SignalProtoSet.InitSignalKeyIdPairs);
        RunCacheStep("Icon bindings", IconRuntime.ApplyIcons);
        RunCacheStep("Game icon set", RebuildGameIconSet);
    }

    private static void RunCacheStep(string name, Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(
                "DSPCore.ProtoRegistrationRuntime",
                ex,
                new ErrorDiagnosticContext(Note: $"Proto cache rebuild step failed: {name}"));
            DspCore.Logger?.LogWarning($"Failed to rebuild DSP proto cache step {name}; continuing with remaining steps: {ex}");
        }
    }

    private static void RunItemPrefabCacheStep(string name, Action action)
    {
        RunCacheStep(name, () =>
        {
            var items = LDB.items?.dataArray;
            if (items == null)
            {
                action();
                return;
            }

            List<ItemProto>? patchedItems = null;
            foreach (var item in items)
            {
                if (item != null && item.prefabDesc == null)
                {
                    patchedItems ??= new List<ItemProto>();
                    patchedItems.Add(item);
                    item.prefabDesc = PrefabDesc.none;
                }
            }

            if (patchedItems == null)
            {
                action();
                return;
            }

            try
            {
                action();
            }
            finally
            {
                foreach (var item in patchedItems)
                {
                    item.prefabDesc = null;
                }
            }
        });
    }

    private static void RebuildGameIconSet()
    {
        if (GameMain.iconSet == null)
        {
            return;
        }

        GameMain.iconSet.loaded = false;
        GameMain.iconSet.Create();
    }

    public static void RebuildAllDataIndices()
    {
        foreach (var property in typeof(LDB).GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            var propertyType = property.PropertyType;
            var protoType = GetProtoSetElementType(propertyType);
            if (protoType == null)
            {
                continue;
            }

            var protoSet = property.GetValue(null);
            if (protoSet == null)
            {
                continue;
            }

            var method = typeof(ProtoRegistrationRuntime).GetMethod(nameof(RebuildDataIndices), BindingFlags.NonPublic | BindingFlags.Static);
            method!.MakeGenericMethod(protoType).Invoke(null, new[] { protoSet });
        }
    }

    private static Type GetConcreteProtoType(Type declaredType, Type runtimeType)
    {
        return typeof(Proto).IsAssignableFrom(declaredType) && !declaredType.IsAbstract ? declaredType : runtimeType;
    }

    private static void AddProtoGroup(Type protoType, IReadOnlyList<Proto> protos, CoreDataPhase phase)
    {
        var property = FindLdbProtoSetProperty(protoType);
        if (property == null)
        {
            DspCore.Logger?.LogWarning($"No LDB ProtoSet found for {protoType.FullName}; skipped {protos.Count} {phase} registrations.");
            return;
        }

        var protoSet = property.GetValue(null);
        if (protoSet == null)
        {
            DspCore.Logger?.LogWarning($"LDB ProtoSet {property.Name} is null; skipped {protos.Count} {phase} registrations.");
            return;
        }

        var method = typeof(ProtoRegistrationRuntime).GetMethod(nameof(AddToSet), BindingFlags.NonPublic | BindingFlags.Static);
        method!.MakeGenericMethod(protoType).Invoke(null, new object[] { protoSet, protos, phase });
    }

    private static PropertyInfo? FindLdbProtoSetProperty(Type protoType)
    {
        var targetType = typeof(ProtoSet<>).MakeGenericType(protoType);
        return typeof(LDB).GetProperties(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(property => targetType.IsAssignableFrom(property.PropertyType));
    }

    private static Type? GetProtoSetElementType(Type type)
    {
        Type? current = type;
        while (current != null)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(ProtoSet<>))
            {
                return current.GetGenericArguments()[0];
            }

            current = current.BaseType;
        }

        return null;
    }

    private static void AddToSet<T>(ProtoSet<T> protoSet, IReadOnlyList<Proto> protos, CoreDataPhase phase)
        where T : Proto
    {
        var list = (protoSet.dataArray ?? Array.Empty<T>()).ToList();
        var phaseEntries = DspCore.ProtoRegistration.GetByPhase(phase)
            .Where(item => item.Proto is T)
            .ToArray();
        StableProtoIdRuntime.Resolve(phaseEntries, list.Where(item => item != null).Select(item => item!.ID).ToArray());
        EnsureNoImplicitDuplicateIds(list, phaseEntries, phase, typeof(T));

        foreach (var proto in protos)
        {
            if (proto is not T typedProto)
            {
                DspCore.Logger?.LogWarning($"Proto {proto.ID} {proto.Name} is not assignable to {typeof(T).Name}; skipped.");
                continue;
            }

            var index = list.FindIndex(item => item != null && item.ID == typedProto.ID);
            if (index >= 0)
            {
                list[index] = typedProto;
            }
            else
            {
                index = list.Count;
                list.Add(typedProto);
            }

            if (typedProto is ItemProto item)
            {
                SetIndex(item, index);
            }
            else if (typedProto is RecipeProto recipe)
            {
                SetIndex(recipe, index);
            }

            DspCore.Logger?.LogInfo($"Applied {phase} proto {typedProto.ID} {typedProto.Name} to {typeof(T).Name} at index {index}.");
        }

        protoSet.dataArray = list.ToArray();
        RebuildDataIndices(protoSet);
    }

    private static void EnsureNoImplicitDuplicateIds<T>(
        IReadOnlyList<T> existing,
        IReadOnlyList<ProtoRegistrationEntry> entries,
        CoreDataPhase phase,
        Type protoType)
        where T : Proto
    {
        var seen = new Dictionary<int, ProtoRegistrationEntry>();
        foreach (var entry in entries)
        {
            if (entry.Proto is not Proto proto || proto.ID <= 0)
            {
                throw new InvalidOperationException($"Registered {protoType.Name} from {entry.OwnerModGuid} has no positive ID.");
            }

            if (seen.TryGetValue(proto.ID, out var previous))
            {
                throw new InvalidOperationException(
                    $"Duplicate {protoType.Name} ID {proto.ID} in {phase}: {DescribeEntry(previous)} and {DescribeEntry(entry)}. Use ProtoStableId or choose a different int ID.");
            }

            seen[proto.ID] = entry;
        }

        if (phase != CoreDataPhase.Data)
        {
            return;
        }

        var stableIds = new HashSet<int>(entries.Where(item => item.StableId != null && item.Proto is Proto proto).Select(item => ((Proto)item.Proto).ID));
        foreach (var entry in entries)
        {
            if (entry.StableId != null || entry.Proto is not Proto proto || stableIds.Contains(proto.ID))
            {
                continue;
            }

            if (existing.Any(item => item != null && item.ID == proto.ID))
            {
                throw new InvalidOperationException(
                    $"Registered {protoType.Name} ID {proto.ID} from {entry.OwnerModGuid} conflicts with existing LDB data. Use ProtoStableId or choose a different int ID.");
            }
        }
    }

    private static string DescribeEntry(ProtoRegistrationEntry entry)
    {
        return $"{entry.OwnerModGuid}/{entry.Kind}/{(entry.Proto is Proto proto ? proto.ID.ToString() + ":" + proto.Name : entry.Proto.GetType().Name)}";
    }

    private static void RebuildDataIndices<T>(ProtoSet<T> protoSet)
        where T : Proto
    {
        var indices = new Dictionary<int, int>();
        var dataArray = protoSet.dataArray ?? Array.Empty<T>();
        for (var i = 0; i < dataArray.Length; i++)
        {
            var proto = dataArray[i];
            if (proto == null)
            {
                continue;
            }

            proto.name = proto.Name;
            proto.sid = proto.SID;
            indices[proto.ID] = i;
        }

        var indicesField = AccessTools.Field(typeof(ProtoSet<T>), "dataIndices");
        if (indicesField != null)
        {
            indicesField.SetValue(protoSet, indices);
        }
        else
        {
            DspCore.Logger?.LogWarning($"ProtoSet<{typeof(T).Name}>.dataIndices is not accessible; Select(id) may not see new protos.");
        }
    }

    private static void SetIndex(object proto, int index)
    {
        var type = proto.GetType();
        var property = AccessTools.Property(type, "index");
        if (property?.CanWrite == true)
        {
            property.SetValue(proto, index);
            return;
        }

        var field = AccessTools.Field(type, "index");
        field?.SetValue(proto, index);
    }

    private static void RebuildRecipeExecuteData()
    {
        RecipeProto.recipeExecuteData = new Dictionary<int, RecipeExecuteData>();
        foreach (var recipe in LDB.recipes.dataArray)
        {
            if (recipe == null || RecipeProto.recipeExecuteData.ContainsKey(recipe.ID))
            {
                continue;
            }

            RecipeProto.recipeExecuteData.Add(
                recipe.ID,
                new RecipeExecuteData(
                    recipe.Items,
                    recipe.ItemCounts,
                    recipe.Results,
                    recipe.ResultCounts,
                    recipe.TimeSpend * 10000,
                    recipe.TimeSpend * 100000,
                    recipe.productive));
        }
    }
}

[HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
internal static class ProtoRegistrationRuntimePatches
{
    private static void Prefix()
    {
        ProtoRegistrationRuntime.ApplyPhase(CoreDataPhase.Data);
        ProtoRegistrationRuntime.ApplyPhase(CoreDataPhase.DataUpdates);
    }

    private static void Postfix()
    {
        ProtoRegistrationRuntime.ApplyPhase(CoreDataPhase.DataFinalFixes);
        ProtoRegistrationRuntime.RebuildDerivedCaches();
        BuildBarRuntime.Apply();
    }
}
