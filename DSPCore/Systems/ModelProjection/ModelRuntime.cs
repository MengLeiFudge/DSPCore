using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace DSPCore;

internal static class ModelRuntime
{
    private static bool applied;

    public static void Apply()
    {
        if (applied)
        {
            return;
        }

        applied = true;
        foreach (var descriptor in DspCore.Models.GetAll())
        {
            try
            {
                ApplyDescriptor(descriptor);
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
                DspCore.Logger?.LogError($"Failed to apply DSPCore model {descriptor.ModelIndex}: {ex}");
            }
        }
    }

    private static void ApplyDescriptor(ModelDescriptor descriptor)
    {
        var source = LDB.models.Select(descriptor.SourceModelIndex);
        if (source == null)
        {
            DspCore.Logger?.LogWarning($"DSPCore model source {descriptor.SourceModelIndex} was not found.");
            return;
        }

        var clone = CloneModel(source);
        clone.ID = descriptor.ModelIndex;
        clone.name = clone.Name;
        clone.sid = clone.SID;
        clone.prefabDesc = ClonePrefab(source.prefabDesc);
        if (clone.prefabDesc != null)
        {
            clone.prefabDesc.modelIndex = descriptor.ModelIndex;
        }

        descriptor.ConfigureModel?.Invoke(clone);
        if (clone.prefabDesc != null)
        {
            descriptor.ConfigurePrefab?.Invoke(clone.prefabDesc);
        }

        var list = (LDB.models.dataArray ?? Array.Empty<ModelProto>()).ToList();
        var index = list.FindIndex(item => item != null && item.ID == descriptor.ModelIndex);
        if (index >= 0)
        {
            list[index] = clone;
        }
        else
        {
            list.Add(clone);
        }

        LDB.models.dataArray = list.ToArray();
        DspCore.Logger?.LogInfo($"Applied DSPCore model clone {descriptor.SourceModelIndex} -> {descriptor.ModelIndex}.");
    }

    private static ModelProto CloneModel(ModelProto source)
    {
        var clone = new ModelProto();
        CopyPublicFields(source, clone);
        return clone;
    }

    private static PrefabDesc? ClonePrefab(PrefabDesc? source)
    {
        if (source == null)
        {
            return null;
        }

#pragma warning disable SYSLIB0050
        var clone = (PrefabDesc)FormatterServices.GetUninitializedObject(typeof(PrefabDesc));
#pragma warning restore SYSLIB0050
        CopyPublicFields(source, clone);
        return clone;
    }

    private static void CopyPublicFields(object source, object target)
    {
        foreach (var field in source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            field.SetValue(target, field.GetValue(source));
        }
    }

    public static void RebuildPrefabDescArray()
    {
        PlanetFactory.PrefabDescByModelIndex = null;
        PlanetFactory.InitPrefabDescArray();
    }
}
