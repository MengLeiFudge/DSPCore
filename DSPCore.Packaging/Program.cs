namespace DSPCore.Packaging;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

internal static class Program
{
    private const string PackageOwner = "MengLei";
    private const string PackageName = "DSPCore";
    private const string NebulaAdapterPackageName = "DSPCore_NebulaAdapter";
    private const string NebulaApiDependency = "nebula-NebulaMultiplayerModApi-2.1.0";
    private const string FrameworkConfiguration = "Debug";

    private static readonly IReadOnlyList<PackageFile> CoreFiles = new[]
    {
        new PackageFile("manifest.json", "manifest.json", required: true),
        new PackageFile("README.md", "README.md", required: true),
        new PackageFile("README-EN.md", "README-EN.md", required: false),
        new PackageFile("LICENSE", "LICENSE", required: false),
        new PackageFile("DSPCore/bin/" + FrameworkConfiguration + "/DSPCore.dll", "plugins/DSPCore/DSPCore.dll", required: true),
        new PackageFile("DSPCore.Preloader/bin/" + FrameworkConfiguration + "/DSPCore.Preloader.dll", "patchers/DSPCore.Preloader.dll", required: true)
    };

    private static readonly IReadOnlyList<PackageFile> NebulaAdapterFiles = new[]
    {
        new PackageFile("LICENSE", "LICENSE", required: false),
        new PackageFile("DSPCore.NebulaAdapter/bin/" + FrameworkConfiguration + "/DSPCore.NebulaAdapter.dll", "plugins/DSPCore/DSPCore.NebulaAdapter.dll", required: true)
    };

    private static int Main(string[] args)
    {
        try
        {
            var root = FindRepositoryRoot(AppDomain.CurrentDomain.BaseDirectory);
            var version = ReadManifestVersion(Path.Combine(root, "manifest.json"));
            var artifactsRoot = Path.Combine(root, "artifacts", "thunderstore");
            var coreStagingRoot = Path.Combine(artifactsRoot, PackageName);
            var coreZipPath = Path.Combine(artifactsRoot, PackageOwner + "-" + PackageName + "-" + version + ".zip");
            var nebulaStagingRoot = Path.Combine(artifactsRoot, NebulaAdapterPackageName);
            var nebulaZipPath = Path.Combine(artifactsRoot, PackageOwner + "-" + NebulaAdapterPackageName + "-" + version + ".zip");

            Directory.CreateDirectory(artifactsRoot);
            BuildPackage(root, coreStagingRoot, coreZipPath, CoreFiles, writeManifest: null, writeReadme: null);
            BuildPackage(
                root,
                nebulaStagingRoot,
                nebulaZipPath,
                NebulaAdapterFiles,
                writeManifest: path => WriteNebulaAdapterManifest(path, version),
                writeReadme: WriteNebulaAdapterReadme);

            Console.WriteLine("DSPCore Thunderstore package staged at: " + coreStagingRoot);
            Console.WriteLine("DSPCore Thunderstore package zip: " + coreZipPath);
            Console.WriteLine("DSPCore Nebula adapter package staged at: " + nebulaStagingRoot);
            Console.WriteLine("DSPCore Nebula adapter package zip: " + nebulaZipPath);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static void BuildPackage(
        string root,
        string stagingRoot,
        string zipPath,
        IReadOnlyList<PackageFile> files,
        Action<string>? writeManifest,
        Action<string>? writeReadme)
    {
        ResetDirectory(stagingRoot);

        foreach (var file in files)
        {
            CopyPackageFile(root, stagingRoot, file);
        }

        writeManifest?.Invoke(Path.Combine(stagingRoot, "manifest.json"));
        writeReadme?.Invoke(Path.Combine(stagingRoot, "README.md"));
        EnsureIcon(root, stagingRoot);

        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }

        ZipFile.CreateFromDirectory(stagingRoot, zipPath, CompressionLevel.Optimal, includeBaseDirectory: false, Encoding.UTF8);
    }

    private static string FindRepositoryRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "DSPCore.sln")) &&
                File.Exists(Path.Combine(directory.FullName, "manifest.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Cannot find DSPCore repository root from " + startDirectory);
    }

    private static string ReadManifestVersion(string manifestPath)
    {
        var text = File.ReadAllText(manifestPath, Encoding.UTF8);
        var match = Regex.Match(text, "\"version_number\"\\s*:\\s*\"(?<version>[^\"]+)\"");
        if (!match.Success)
        {
            throw new InvalidOperationException("manifest.json does not contain version_number.");
        }

        return match.Groups["version"].Value;
    }

    private static void ResetDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }

        Directory.CreateDirectory(path);
    }

    private static void CopyPackageFile(string root, string stagingRoot, PackageFile file)
    {
        var source = Path.Combine(root, file.Source);
        if (!File.Exists(source))
        {
            if (file.Required)
            {
                throw new FileNotFoundException("Required package file is missing: " + source);
            }

            return;
        }

        var destination = Path.Combine(stagingRoot, file.Destination);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(source, destination, overwrite: true);
    }

    private static void WriteNebulaAdapterManifest(string path, string version)
    {
        var text =
            "{\n" +
            "  \"name\": \"" + NebulaAdapterPackageName + "\",\n" +
            "  \"version_number\": \"" + version + "\",\n" +
            "  \"website_url\": \"\",\n" +
            "  \"description\": \"Optional Nebula transport adapter for DSPCore. DSPCore 的可选 Nebula 联机传输适配器。\",\n" +
            "  \"dependencies\": [\n" +
            "    \"xiaoye97-BepInEx-5.4.17\",\n" +
            "    \"" + PackageOwner + "-" + PackageName + "-" + version + "\",\n" +
            "    \"" + NebulaApiDependency + "\"\n" +
            "  ]\n" +
            "}\n";
        File.WriteAllText(path, text, Encoding.UTF8);
    }

    private static void WriteNebulaAdapterReadme(string path)
    {
        File.WriteAllText(
            path,
            "# DSPCore Nebula Adapter\n\n" +
            "Optional Nebula transport adapter for DSPCore. Install this package only when a mod needs DSPCore multiplayer packet transport through Nebula.\n\n" +
            "DSPCore 的可选 Nebula 联机传输适配器。只有需要通过 Nebula 发送 DSPCore 联机 packet 时才需要安装此包。\n",
            Encoding.UTF8);
    }

    private static void EnsureIcon(string root, string stagingRoot)
    {
        var source = Path.Combine(root, "icon.png");
        var destination = Path.Combine(stagingRoot, "icon.png");
        if (File.Exists(source))
        {
            File.Copy(source, destination, overwrite: true);
            return;
        }

        WriteDefaultIcon(destination);
    }

    private static void WriteDefaultIcon(string path)
    {
        const int size = 256;
        using var file = File.Create(path);
        file.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, 0, 8);

        using (var ihdr = new MemoryStream())
        {
            WriteUInt32BigEndian(ihdr, size);
            WriteUInt32BigEndian(ihdr, size);
            ihdr.WriteByte(8);
            ihdr.WriteByte(6);
            ihdr.WriteByte(0);
            ihdr.WriteByte(0);
            ihdr.WriteByte(0);
            WritePngChunk(file, "IHDR", ihdr.ToArray());
        }

        using (var raw = new MemoryStream())
        {
            for (var y = 0; y < size; y++)
            {
                raw.WriteByte(0);
                for (var x = 0; x < size; x++)
                {
                    raw.WriteByte(32);
                    raw.WriteByte(94);
                    raw.WriteByte(145);
                    raw.WriteByte(255);
                }
            }

            using var compressed = new MemoryStream();
            using (var zlib = new ZLibStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
            {
                raw.Position = 0;
                raw.CopyTo(zlib);
            }

            WritePngChunk(file, "IDAT", compressed.ToArray());
        }

        WritePngChunk(file, "IEND", Array.Empty<byte>());
    }

    private static void WritePngChunk(Stream output, string type, byte[] data)
    {
        var typeBytes = Encoding.ASCII.GetBytes(type);
        WriteUInt32BigEndian(output, (uint)data.Length);
        output.Write(typeBytes, 0, typeBytes.Length);
        output.Write(data, 0, data.Length);

        var crcInput = new byte[typeBytes.Length + data.Length];
        Buffer.BlockCopy(typeBytes, 0, crcInput, 0, typeBytes.Length);
        Buffer.BlockCopy(data, 0, crcInput, typeBytes.Length, data.Length);
        WriteUInt32BigEndian(output, Crc32(crcInput));
    }

    private static void WriteUInt32BigEndian(Stream output, int value)
    {
        WriteUInt32BigEndian(output, unchecked((uint)value));
    }

    private static void WriteUInt32BigEndian(Stream output, uint value)
    {
        output.WriteByte((byte)(value >> 24));
        output.WriteByte((byte)(value >> 16));
        output.WriteByte((byte)(value >> 8));
        output.WriteByte((byte)value);
    }

    private static uint Crc32(byte[] data)
    {
        var crc = 0xffffffffu;
        foreach (var value in data)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) == 0 ? crc >> 1 : (crc >> 1) ^ 0xedb88320u;
            }
        }

        return ~crc;
    }

    private sealed class PackageFile
    {
        public PackageFile(string source, string destination, bool required)
        {
            Source = source.Replace('/', Path.DirectorySeparatorChar);
            Destination = destination.Replace('/', Path.DirectorySeparatorChar);
            Required = required;
        }

        public string Source { get; }

        public string Destination { get; }

        public bool Required { get; }
    }
}
