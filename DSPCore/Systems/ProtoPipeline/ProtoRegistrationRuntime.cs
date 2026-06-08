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
        try
        {
            RebuildAllDataIndices();
            ItemProto.InitFuelNeeds();
            ItemProto.InitTurretNeeds();
            ItemProto.InitFluids();
            ItemProto.InitTurrets();
            ItemProto.InitEnemyDropTables();
            ItemProto.InitConstructableItems();
            ItemProto.InitItemIds();
            ItemProto.InitItemIndices();
            ItemProto.InitMechaMaterials();
            ItemProto.InitFighterIndices();
            ItemProto.InitPowerFacilityIndices();
            ItemProto.InitProductionMask();
            ModelProto.InitMaxModelIndex();
            ModelProto.InitModelIndices();
            ModelProto.InitModelOrders();
            RecipeProto.InitRecipeItems();
            RecipeProto.InitFractionatorNeeds();
            RecipeTypeRuntime.Apply();
            RebuildRecipeExecuteData();
            SignalProtoSet.InitSignalKeyIdPairs();
            IconRuntime.ApplyIcons();

            if (GameMain.iconSet != null)
            {
                GameMain.iconSet.loaded = false;
                GameMain.iconSet.Create();
            }
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.ProtoRegistrationRuntime", ex);
            DspCore.Logger?.LogError($"Failed to rebuild DSP proto caches: {ex}");
        }
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
        while (type != null)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ProtoSet<>))
            {
                return type.GetGenericArguments()[0];
            }

            type = type.BaseType;
        }

        return null;
    }

    private static void AddToSet<T>(ProtoSet<T> protoSet, IReadOnlyList<Proto> protos, CoreDataPhase phase)
        where T : Proto
    {
        var list = (protoSet.dataArray ?? Array.Empty<T>()).ToList();
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
