using System.IO;

namespace BepInEx;

public static class Paths
{
    public static string ConfigPath { get; set; } = Path.GetTempPath();
}
