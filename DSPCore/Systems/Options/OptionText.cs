namespace DSPCore;

internal static class OptionText
{
    public const string Title = "DSPCore.Options.Title";
    public const string Summary = "DSPCore.Options.Summary";
    public const string Footer = "DSPCore.Options.Footer";
    public const string Close = "DSPCore.Options.Close";
    public const string NoOptions = "DSPCore.Options.NoOptions";
    public const string General = "DSPCore.Options.General";
    public const string Reset = "DSPCore.Options.Reset";

    public static void RegisterBuiltInLocalizations()
    {
        Register(Title, "zhCN", "DSPCore 设置");
        Register(Title, "enUS", "DSPCore Settings");
        Register(Summary, "zhCN", "由模组注册的统一配置项。");
        Register(Summary, "enUS", "Unified options registered by mods.");
        Register(Footer, "zhCN", "修改会写入 DSPCore 的 BepInEx 配置。");
        Register(Footer, "enUS", "Changes are written to the DSPCore BepInEx config.");
        Register(Close, "zhCN", "关闭");
        Register(Close, "enUS", "Close");
        Register(NoOptions, "zhCN", "当前没有已注册配置项。");
        Register(NoOptions, "enUS", "No options registered.");
        Register(General, "zhCN", "通用");
        Register(General, "enUS", "General");
        Register(Reset, "zhCN", "重置");
        Register(Reset, "enUS", "Reset");
    }

    private static void Register(string key, string language, string value)
    {
        DspCore.Resources.RegisterLocalization(new LocalizationEntry(key, language, value, DSPCorePlugin.PluginGuid));
    }
}
