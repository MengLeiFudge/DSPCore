using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace DSPCore;

internal static class BuildingParameterRuntime
{
    private const int MagicStart = 0x44535043;
    private const int MagicEnd = 0x43505344;
    private const int FormatVersion = 1;

    public static void AddCopiedBlocks(ref BuildingParameters parameters, int objectId, PlanetFactory factory)
    {
        var blocks = new List<BuildingParameterBlock>();
        foreach (var descriptor in DspCore.Blueprints.GetAll())
        {
            try
            {
                var block = descriptor.Copy(factory, objectId);
                if (block != null)
                {
                    blocks.Add(block);
                }
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            }
        }

        if (blocks.Count > 0)
        {
            parameters.parameters = AppendBlocks(parameters.parameters, blocks);
        }
    }

    public static void PreserveBlocksFromParamsArray(ref BuildingParameters parameters, int[] rawParameters)
    {
        var blocks = ReadBlocks(rawParameters);
        if (blocks.Count > 0)
        {
            parameters.parameters = AppendBlocks(parameters.parameters, blocks);
        }
    }

    public static void AppendBlocksToParamsArray(BuildingParameters parameters, ref int[] rawParameters, ref int paramCount)
    {
        var blocks = ReadBlocks(parameters.parameters);
        if (blocks.Count == 0)
        {
            return;
        }

        rawParameters = AppendBlocks(rawParameters, blocks);
        paramCount = rawParameters.Length;
    }

    public static bool CanPaste(BuildingParameters parameters, int objectId, PlanetFactory factory)
    {
        foreach (var block in ReadBlocks(parameters.parameters))
        {
            if (!DspCore.Blueprints.TryGet(block.BlockId, out var descriptor))
            {
                continue;
            }

            try
            {
                if (descriptor.CanPaste != null && !descriptor.CanPaste(factory, objectId, block))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
                return false;
            }
        }

        return true;
    }

    public static void Paste(BuildingParameters parameters, int objectId, PlanetFactory factory)
    {
        foreach (var block in ReadBlocks(parameters.parameters))
        {
            if (!DspCore.Blueprints.TryGet(block.BlockId, out var descriptor))
            {
                continue;
            }

            try
            {
                descriptor.Paste(factory, objectId, block);
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            }
        }
    }

    public static void ApplyPrebuild(int entityId, int[] parameters, PlanetFactory factory)
    {
        foreach (var block in ReadBlocks(parameters))
        {
            if (!DspCore.Blueprints.TryGet(block.BlockId, out var descriptor) || descriptor.ApplyPrebuild == null)
            {
                continue;
            }

            try
            {
                descriptor.ApplyPrebuild(factory, entityId, block);
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            }
        }
    }

    private static int[] AppendBlocks(int[]? source, IReadOnlyCollection<BuildingParameterBlock> blocks)
    {
        var baseData = StripBlocks(source);
        var payload = EncodePayload(blocks);
        var result = new int[baseData.Length + payload.Length + 2];
        Array.Copy(baseData, result, baseData.Length);
        Array.Copy(payload, 0, result, baseData.Length, payload.Length);
        result[result.Length - 2] = payload.Length;
        result[result.Length - 1] = MagicEnd;
        return result;
    }

    private static IReadOnlyList<BuildingParameterBlock> ReadBlocks(int[]? source)
    {
        if (source == null || source.Length < 5 || source[source.Length - 1] != MagicEnd)
        {
            return Array.Empty<BuildingParameterBlock>();
        }

        var payloadLength = source[source.Length - 2];
        var start = source.Length - 2 - payloadLength;
        if (payloadLength <= 0 || start < 0 || source[start] != MagicStart)
        {
            return Array.Empty<BuildingParameterBlock>();
        }

        var index = start + 1;
        var version = source[index++];
        if (version > FormatVersion)
        {
            DspCore.Logger?.LogWarning($"DSPCore blueprint parameter format {version} is newer than runtime format {FormatVersion}.");
        }

        var count = source[index++];
        var blocks = new List<BuildingParameterBlock>(count);
        for (var i = 0; i < count && index < source.Length - 2; i++)
        {
            var idLength = source[index++];
            if (idLength < 0 || index + idLength > source.Length - 2)
            {
                break;
            }

            var chars = new char[idLength];
            for (var c = 0; c < idLength; c++)
            {
                chars[c] = (char)source[index++];
            }

            var dataLength = source[index++];
            if (dataLength < 0 || index + dataLength > source.Length - 2)
            {
                break;
            }

            var data = new int[dataLength];
            Array.Copy(source, index, data, 0, dataLength);
            index += dataLength;
            blocks.Add(new BuildingParameterBlock(new string(chars), data));
        }

        return blocks;
    }

    private static int[] StripBlocks(int[]? source)
    {
        if (source == null)
        {
            return Array.Empty<int>();
        }

        if (source.Length < 5 || source[source.Length - 1] != MagicEnd)
        {
            return source.ToArray();
        }

        var payloadLength = source[source.Length - 2];
        var start = source.Length - 2 - payloadLength;
        if (payloadLength <= 0 || start < 0 || source[start] != MagicStart)
        {
            return source.ToArray();
        }

        var stripped = new int[start];
        Array.Copy(source, stripped, stripped.Length);
        return stripped;
    }

    private static int[] EncodePayload(IReadOnlyCollection<BuildingParameterBlock> blocks)
    {
        var values = new List<int> { MagicStart, FormatVersion, blocks.Count };
        foreach (var block in blocks)
        {
            values.Add(block.BlockId.Length);
            values.AddRange(block.BlockId.Select(ch => (int)ch));
            values.Add(block.Data?.Length ?? 0);
            if (block.Data != null)
            {
                values.AddRange(block.Data);
            }
        }

        return values.ToArray();
    }
}

internal static class BuildingParameterRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingParameters), nameof(BuildingParameters.CopyFromFactoryObject))]
    private static void CopyFromFactoryObject(ref BuildingParameters __instance, bool __result, int objectId, PlanetFactory factory)
    {
        if (__result)
        {
            BuildingParameterRuntime.AddCopiedBlocks(ref __instance, objectId, factory);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingParameters), nameof(BuildingParameters.FromParamsArray))]
    private static void FromParamsArray(ref BuildingParameters __instance, int[] _parameters)
    {
        BuildingParameterRuntime.PreserveBlocksFromParamsArray(ref __instance, _parameters);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingParameters), nameof(BuildingParameters.ToParamsArray))]
    private static void ToParamsArray(BuildingParameters __instance, ref int[] _parameters, ref int _paramCount)
    {
        BuildingParameterRuntime.AppendBlocksToParamsArray(__instance, ref _parameters, ref _paramCount);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingParameters), nameof(BuildingParameters.CanPasteToFactoryObject))]
    private static void CanPasteToFactoryObject(BuildingParameters __instance, ref bool __result, int objectId, PlanetFactory factory)
    {
        if (__result)
        {
            __result = BuildingParameterRuntime.CanPaste(__instance, objectId, factory);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingParameters), nameof(BuildingParameters.PasteToFactoryObject))]
    private static void PasteToFactoryObject(BuildingParameters __instance, bool __result, int objectId, PlanetFactory factory)
    {
        if (__result)
        {
            BuildingParameterRuntime.Paste(__instance, objectId, factory);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingParameters), nameof(BuildingParameters.ApplyPrebuildParametersToEntity))]
    private static void ApplyPrebuildParametersToEntity(int entityId, int[] parameters, PlanetFactory factory)
    {
        BuildingParameterRuntime.ApplyPrebuild(entityId, parameters, factory);
    }
}
