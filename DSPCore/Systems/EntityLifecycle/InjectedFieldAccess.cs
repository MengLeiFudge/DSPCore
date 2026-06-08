using System;
using System.Reflection;

namespace DSPCore;

internal static class InjectedFieldAccess
{
    private static readonly FieldInfo? EntityCustomId = typeof(EntityData).GetField("customId", BindingFlags.Instance | BindingFlags.Public);
    private static readonly FieldInfo? EntityCustomType = typeof(EntityData).GetField("customType", BindingFlags.Instance | BindingFlags.Public);
    private static readonly FieldInfo? PrefabCustomData = typeof(PrefabDesc).GetField("customData", BindingFlags.Instance | BindingFlags.Public);

    public static void SetEntityMarker(PlanetFactory factory, int entityId, string marker)
    {
        if (EntityCustomId == null && EntityCustomType == null)
        {
            return;
        }

        try
        {
            var boxed = (object)factory.entityPool[entityId];
            EntityCustomId?.SetValue(boxed, marker.GetHashCode());
            EntityCustomType?.SetValue(boxed, 1);
            factory.entityPool[entityId] = (EntityData)boxed;
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.InjectedFieldAccess", ex);
        }
    }

    public static bool HasPrefabCustomData(PrefabDesc desc)
    {
        return PrefabCustomData != null && desc != null;
    }
}
