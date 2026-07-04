namespace DSPCore;

internal static class OptionText
{
    public const string Title = "DSPCore.Options.Title";
    public const string Summary = "DSPCore.Options.Summary";
    public const string Footer = "DSPCore.Options.Footer";
    public const string Close = "DSPCore.Options.Close";
    public const string NoOptions = "DSPCore.Options.NoOptions";
    public const string General = "DSPCore.Options.General";
    public const string Capture = "DSPCore.Options.Capture";
    public const string Reset = "DSPCore.Options.Reset";
    public const string PressAKey = "DSPCore.Options.PressAKey";
    public const string EntryButton = "DSPCore.Options.EntryButton";
    public const string KeyEntryButton = "DSPCore.Options.KeyEntryButton";
    public const string GlobalSavesButton = "DSPCore.Options.GlobalSavesButton";
    public const string GlobalSavesTitle = "DSPCore.Options.GlobalSavesTitle";
    public const string GlobalSavesSummary = "DSPCore.Options.GlobalSavesSummary";
    public const string GlobalSavesFooter = "DSPCore.Options.GlobalSavesFooter";
    public const string GlobalSavesPath = "DSPCore.Options.GlobalSavesPath";
    public const string GlobalSavesCounts = "DSPCore.Options.GlobalSavesCounts";
    public const string GlobalSavesNoBlocks = "DSPCore.Options.GlobalSavesNoBlocks";
    public const string GlobalSavesRegistered = "DSPCore.Options.GlobalSavesRegistered";
    public const string GlobalSavesFileOnly = "DSPCore.Options.GlobalSavesFileOnly";
    public const string GlobalSavesInitialized = "DSPCore.Options.GlobalSavesInitialized";
    public const string GlobalSavesBytes = "DSPCore.Options.GlobalSavesBytes";

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
        Register(Capture, "zhCN", "捕获");
        Register(Capture, "enUS", "Capture");
        Register(Reset, "zhCN", "重置");
        Register(Reset, "enUS", "Reset");
        Register(PressAKey, "zhCN", "请按下一个按键...");
        Register(PressAKey, "enUS", "Press a key...");
        Register(EntryButton, "zhCN", "模组设置");
        Register(EntryButton, "enUS", "Mod Settings");
        Register(KeyEntryButton, "zhCN", "DSPCore 按键设置");
        Register(KeyEntryButton, "enUS", "DSPCore Key Bindings");
        Register(GlobalSavesButton, "zhCN", "全局数据");
        Register(GlobalSavesButton, "enUS", "Global Data");
        Register(GlobalSavesTitle, "zhCN", "跨存档全局数据");
        Register(GlobalSavesTitle, "enUS", "Cross-save Global Data");
        Register(GlobalSavesSummary, "zhCN", "只读查看已注册和已落盘的 Saves.Global 数据块。");
        Register(GlobalSavesSummary, "enUS", "Read-only view of registered and persisted Saves.Global blocks.");
        Register(GlobalSavesFooter, "zhCN", "这里不会编辑或导出数据；全局数据由对应模组代码读写。");
        Register(GlobalSavesFooter, "enUS", "This view does not edit or export data; global data is owned by each mod.");
        Register(GlobalSavesPath, "zhCN", "文件：{0}");
        Register(GlobalSavesPath, "enUS", "File: {0}");
        Register(GlobalSavesCounts, "zhCN", "注册项 {0}，文件块 {1}，已加载 {2}");
        Register(GlobalSavesCounts, "enUS", "Registered {0}, file blocks {1}, loaded {2}");
        Register(GlobalSavesNoBlocks, "zhCN", "当前没有跨存档全局数据。");
        Register(GlobalSavesNoBlocks, "enUS", "No cross-save global data exists.");
        Register(GlobalSavesRegistered, "zhCN", "已注册");
        Register(GlobalSavesRegistered, "enUS", "Registered");
        Register(GlobalSavesFileOnly, "zhCN", "仅文件存在");
        Register(GlobalSavesFileOnly, "enUS", "File only");
        Register(GlobalSavesInitialized, "zhCN", "新初始化");
        Register(GlobalSavesInitialized, "enUS", "Initialized");
        Register(GlobalSavesBytes, "zhCN", "{0} 字节");
        Register(GlobalSavesBytes, "enUS", "{0} bytes");
    }

    private static void Register(string key, string language, string value)
    {
        DspCore.Resources.RegisterLocalization(new LocalizationEntry(key, language, value, DSPCorePlugin.PluginGuid));
    }
}
