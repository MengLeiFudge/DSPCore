using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using UnityEngine;

namespace DSPCore;

#pragma warning disable CS0618

internal static class SaveRuntime
{
    private const int SaveFileVersion = 1;
    private const string SaveExt = ".dspcore";
    private static readonly Encoding Utf8 = new UTF8Encoding();
    private static FileStream? currentLoadStream;
    private static Dictionary<string, SaveEntry> saveEntries = new(StringComparer.Ordinal);

    public static void RegisterLegacyHandlers()
    {
        foreach (var plugin in Chainloader.PluginInfos.Values)
        {
            RegisterLegacyHandler(plugin.Instance);
        }
    }

    public static void RegisterLegacyHandler(object mod)
    {
        if (mod is not crecheng.DSPModSave.IModCanSave legacy)
        {
            return;
        }

        var modGuid = mod is BaseUnityPlugin plugin ? plugin.Info.Metadata.GUID : mod.GetType().FullName ?? mod.GetType().Name;
        var loadOrder = CoreLoadOrder.Postload;
        var attribute = mod.GetType().GetCustomAttributes(typeof(crecheng.DSPModSave.ModSaveSettingsAttribute), true)
            .OfType<crecheng.DSPModSave.ModSaveSettingsAttribute>()
            .FirstOrDefault();
        if (attribute?.LoadOrder == crecheng.DSPModSave.LoadOrder.Preload)
        {
            loadOrder = CoreLoadOrder.Preload;
        }

        DspCore.Saves.Register(modGuid, new LegacySaveHandlerAdapter(legacy), loadOrder);
    }

    public static void OnNewGame()
    {
        Lifecycle.RaiseNewGame();
        foreach (var registration in OrderedRegistrations())
        {
            SafeIntoOtherSave(registration);
        }
    }

    public static void OnSave(string saveName)
    {
        Lifecycle.RaiseBeforeSave(saveName);
        var registrations = OrderedRegistrations().ToArray();
        if (registrations.Length == 0)
        {
            return;
        }

        var path = GetSavePath(saveName);
        try
        {
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(stream, Utf8, true);
            writer.Write('D');
            writer.Write('S');
            writer.Write('P');
            writer.Write('C');
            writer.Write(SaveFileVersion);
            writer.Write(registrations.Length);

            var headerPositions = new Dictionary<string, long>(StringComparer.Ordinal);
            foreach (var registration in registrations)
            {
                writer.Write(registration.ModGuid);
                headerPositions[registration.ModGuid] = stream.Position;
                writer.Write(0L);
                writer.Write(0L);
            }

            foreach (var registration in registrations)
            {
                var begin = stream.Position;
                try
                {
                    using var buffer = new MemoryStream();
                    using (var blockWriter = new BinaryWriter(buffer, Utf8, true))
                    {
                        registration.Handler.Export(blockWriter);
                    }

                    var data = buffer.ToArray();
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    DspCore.Errors.ReportException(registration.ModGuid, ex);
                    DspCore.Logger?.LogError($"{registration.ModGuid} save export failed: {ex}");
                }

                var end = stream.Position;
                stream.Position = headerPositions[registration.ModGuid];
                writer.Write(begin);
                writer.Write(end);
                stream.Position = stream.Length;
            }
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.SaveRuntime", ex);
            DspCore.Logger?.LogError($"DSPCore sidecar save failed for {saveName}: {ex}");
        }
    }

    public static void OnAutoSave()
    {
        RotateAutoSave("_autosave_2", "_autosave_3");
        RotateAutoSave("_autosave_1", "_autosave_2");
        RotateAutoSave("_autosave_0", "_autosave_1");
        RotateAutoSave(GameSave.AutoSaveTmp, "_autosave_0");
    }

    public static void OnPreLoad(string saveName)
    {
        Lifecycle.RaiseBeforeLoad(saveName);
        CloseLoadStream();
        saveEntries.Clear();

        var path = GetSavePath(saveName);
        if (!File.Exists(path))
        {
            foreach (var registration in OrderedRegistrations().Where(item => item.LoadOrder == CoreLoadOrder.Preload))
            {
                SafeIntoOtherSave(registration);
            }

            return;
        }

        try
        {
            currentLoadStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            LoadHeader(currentLoadStream);
            CallImports(CoreLoadOrder.Preload);
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.SaveRuntime", ex);
            DspCore.Logger?.LogError($"DSPCore sidecar preload failed for {saveName}: {ex}");
            CloseLoadStream();
        }
    }

    public static void OnPostLoad()
    {
        if (currentLoadStream == null)
        {
            foreach (var registration in OrderedRegistrations().Where(item => item.LoadOrder == CoreLoadOrder.Postload))
            {
                SafeIntoOtherSave(registration);
            }

            Lifecycle.RaiseAfterLoad();
            return;
        }

        try
        {
            CallImports(CoreLoadOrder.Postload);
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.SaveRuntime", ex);
            DspCore.Logger?.LogError($"DSPCore sidecar postload failed: {ex}");
        }
        finally
        {
            CloseLoadStream();
            Lifecycle.RaiseAfterLoad();
        }
    }

    public static void OnDeleteSave(string saveName)
    {
        var path = GetSavePath(saveName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        Lifecycle.RaiseSaveDeleted(saveName);
    }

    private static IEnumerable<SaveRegistration> OrderedRegistrations()
    {
        return DspCore.Saves.GetAll().OrderBy(item => item.LoadOrder).ThenBy(item => item.ModGuid, StringComparer.Ordinal);
    }

    private static string GetSavePath(string saveName)
    {
        return Path.Combine(GameConfig.gameSaveFolder, CommonUtils.ValidFileName(saveName) + SaveExt);
    }

    private static void RotateAutoSave(string fromName, string toName)
    {
        var from = GetSavePath(fromName);
        var to = GetSavePath(toName);
        if (!File.Exists(from))
        {
            return;
        }

        if (File.Exists(to))
        {
            File.Delete(to);
        }

        File.Move(from, to);
    }

    private static void LoadHeader(FileStream stream)
    {
        using var reader = new BinaryReader(stream, Utf8, true);
        if (reader.ReadChar() != 'D' || reader.ReadChar() != 'S' || reader.ReadChar() != 'P' || reader.ReadChar() != 'C')
        {
            throw new InvalidDataException("DSPCore sidecar save header is invalid.");
        }

        var version = reader.ReadInt32();
        if (version > SaveFileVersion)
        {
            DspCore.Logger?.LogWarning($"DSPCore sidecar save version {version} is newer than runtime version {SaveFileVersion}.");
        }

        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var modGuid = reader.ReadString();
            var begin = reader.ReadInt64();
            var end = reader.ReadInt64();
            saveEntries[modGuid] = new SaveEntry(begin, end);
        }
    }

    private static void CallImports(CoreLoadOrder loadOrder)
    {
        foreach (var registration in OrderedRegistrations().Where(item => item.LoadOrder == loadOrder))
        {
            if (currentLoadStream == null || !saveEntries.TryGetValue(registration.ModGuid, out var entry))
            {
                SafeIntoOtherSave(registration);
                continue;
            }

            try
            {
                currentLoadStream.Position = entry.Begin;
                var data = new byte[entry.End - entry.Begin];
                currentLoadStream.Read(data, 0, data.Length);
                using var stream = new MemoryStream(data);
                using var reader = new BinaryReader(stream, Utf8);
                registration.Handler.Import(reader);
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(registration.ModGuid, ex);
                DspCore.Logger?.LogError($"{registration.ModGuid} save import failed: {ex}");
            }
        }
    }

    private static void SafeIntoOtherSave(SaveRegistration registration)
    {
        try
        {
            registration.Handler.IntoOtherSave();
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(registration.ModGuid, ex);
            DspCore.Logger?.LogError($"{registration.ModGuid} IntoOtherSave failed: {ex}");
        }
    }

    private static void CloseLoadStream()
    {
        currentLoadStream?.Dispose();
        currentLoadStream = null;
        saveEntries.Clear();
    }

    private readonly struct SaveEntry
    {
        public SaveEntry(long begin, long end)
        {
            Begin = begin;
            End = end;
        }

        public long Begin { get; }

        public long End { get; }
    }

    private sealed class LegacySaveHandlerAdapter : ICoreSaveHandler
    {
        private readonly crecheng.DSPModSave.IModCanSave legacy;

        public LegacySaveHandlerAdapter(crecheng.DSPModSave.IModCanSave legacy)
        {
            this.legacy = legacy;
        }

        public void Export(BinaryWriter writer)
        {
            legacy.Export(writer);
        }

        public void Import(BinaryReader reader)
        {
            legacy.Import(reader);
        }

        public void IntoOtherSave()
        {
            legacy.IntoOtherSave();
        }
    }
}

internal static class SaveRuntimePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.NewGame))]
    private static void GameDataNewGame()
    {
        SaveRuntime.OnNewGame();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSave), nameof(GameSave.SaveCurrentGame))]
    private static void SaveCurrentGame(bool __result, string saveName)
    {
        if (__result)
        {
            SaveRuntime.OnSave(saveName);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSave), nameof(GameSave.AutoSave))]
    private static void AutoSave(bool __result)
    {
        if (__result)
        {
            SaveRuntime.OnAutoSave();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameSave), nameof(GameSave.LoadCurrentGame))]
    private static void PreLoadCurrentGame(string saveName)
    {
        SaveRuntime.OnPreLoad(saveName);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSave), nameof(GameSave.LoadCurrentGame))]
    private static void PostLoadCurrentGame(bool __result)
    {
        if (__result)
        {
            SaveRuntime.OnPostLoad();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIGameSaveEntry), "DeleteSaveFile")]
    private static void PreDeleteSaveFile(UIGameSaveEntry __instance)
    {
        SaveRuntime.OnDeleteSave(__instance.saveName);
    }
}

#pragma warning restore CS0618
