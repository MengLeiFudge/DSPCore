using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;

namespace DSPCore;

internal static class GlobalSaveRuntime
{
    private const int SaveFileVersion = 1;
    private const string RelativePath = "DSPCore/GlobalSaves.dspcore";
    private static readonly Encoding Utf8 = new UTF8Encoding();
    private static readonly Dictionary<string, byte[]> Blocks = new(StringComparer.Ordinal);
    private static bool loaded;

    public static void Initialize()
    {
        Load();
        foreach (var registration in OrderedRegistrations())
        {
            ImportOrInitialize(registration);
        }
    }

    public static void BindIfReady(string modGuid)
    {
        if (!loaded)
        {
            return;
        }

        foreach (var registration in DspCore.GlobalSaves.GetAll())
        {
            if (registration.ModGuid == modGuid)
            {
                ImportOrInitialize(registration);
                return;
            }
        }
    }

    public static void Save()
    {
        var registrations = OrderedRegistrations().ToArray();
        var path = GetPath();
        try
        {
            var writtenBlocks = new Dictionary<string, byte[]>(StringComparer.Ordinal);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(stream, Utf8, true);
            writer.Write('D');
            writer.Write('S');
            writer.Write('P');
            writer.Write('G');
            writer.Write(SaveFileVersion);
            writer.Write(registrations.Length);

            foreach (var registration in registrations)
            {
                writer.Write(registration.ModGuid);
                try
                {
                    using var buffer = new MemoryStream();
                    using (var blockWriter = new BinaryWriter(buffer, Utf8, true))
                    {
                        registration.Handler.Export(blockWriter);
                    }

                    var data = buffer.ToArray();
                    writer.Write(data.Length);
                    writer.Write(data);
                    writtenBlocks[registration.ModGuid] = data;
                }
                catch (Exception ex)
                {
                    DspCore.Errors.ReportException(registration.ModGuid, ex);
                    DspCore.Logger?.LogError($"{registration.ModGuid} global save export failed: {ex}");
                    writer.Write(0);
                    writtenBlocks[registration.ModGuid] = Array.Empty<byte>();
                }
            }

            Blocks.Clear();
            foreach (var block in writtenBlocks)
            {
                Blocks[block.Key] = block.Value;
            }
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.GlobalSaveRuntime", ex);
            DspCore.Logger?.LogError($"DSPCore global save failed: {ex}");
        }
    }

    public static GlobalSaveOverview CreateOverview()
    {
        var registered = OrderedRegistrations()
            .Select(registration =>
            {
                Blocks.TryGetValue(registration.ModGuid, out var data);
                return new GlobalSaveBlockSnapshot(
                    registration.ModGuid,
                    IsRegistered: true,
                    IsLoadedFromFile: data != null,
                    ByteCount: data?.Length ?? 0);
            })
            .ToList();

        var registeredIds = new HashSet<string>(registered.Select(item => item.ModGuid), StringComparer.Ordinal);
        foreach (var block in Blocks.OrderBy(item => item.Key, StringComparer.Ordinal))
        {
            if (registeredIds.Contains(block.Key))
            {
                continue;
            }

            registered.Add(new GlobalSaveBlockSnapshot(
                block.Key,
                IsRegistered: false,
                IsLoadedFromFile: true,
                ByteCount: block.Value.Length));
        }

        return new GlobalSaveOverview(
            GetPath(),
            loaded,
            registered.Count(item => item.IsRegistered),
            Blocks.Count,
            registered);
    }

    private static void Load()
    {
        loaded = true;
        Blocks.Clear();
        var path = GetPath();
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(stream, Utf8, true);
            if (reader.ReadChar() != 'D' || reader.ReadChar() != 'S' || reader.ReadChar() != 'P' || reader.ReadChar() != 'G')
            {
                throw new InvalidDataException("DSPCore global save header is invalid.");
            }

            var version = reader.ReadInt32();
            if (version > SaveFileVersion)
            {
                DspCore.Logger?.LogWarning($"DSPCore global save version {version} is newer than runtime version {SaveFileVersion}.");
            }

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var modGuid = reader.ReadString();
                var length = reader.ReadInt32();
                Blocks[modGuid] = reader.ReadBytes(Math.Max(0, length));
            }
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.GlobalSaveRuntime", ex);
            DspCore.Logger?.LogError($"DSPCore global save load failed: {ex}");
            Blocks.Clear();
        }
    }

    private static void ImportOrInitialize(SaveRegistration registration)
    {
        if (!Blocks.TryGetValue(registration.ModGuid, out var data))
        {
            SafeInitialize(registration);
            return;
        }

        try
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream, Utf8);
            registration.Handler.Import(reader);
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(registration.ModGuid, ex);
            DspCore.Logger?.LogError($"{registration.ModGuid} global save import failed: {ex}");
            SafeInitialize(registration);
        }
    }

    private static void SafeInitialize(SaveRegistration registration)
    {
        try
        {
            registration.Handler.IntoOtherSave();
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(registration.ModGuid, ex);
            DspCore.Logger?.LogError($"{registration.ModGuid} global save initialize failed: {ex}");
        }
    }

    private static IEnumerable<SaveRegistration> OrderedRegistrations()
    {
        return DspCore.GlobalSaves.GetAll().OrderBy(item => item.ModGuid, StringComparer.Ordinal);
    }

    private static string GetPath()
    {
        return Path.Combine(Paths.ConfigPath, RelativePath);
    }
}

internal sealed record GlobalSaveOverview(
    string Path,
    bool IsLoaded,
    int RegisteredCount,
    int FileBlockCount,
    IReadOnlyList<GlobalSaveBlockSnapshot> Blocks);

internal sealed record GlobalSaveBlockSnapshot(
    string ModGuid,
    bool IsRegistered,
    bool IsLoadedFromFile,
    int ByteCount);
