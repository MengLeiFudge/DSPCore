namespace DSPCore;

internal static class BuildBarText
{
    public const string Title = "DSPCore.BuildBar.Title";
    public const string Summary = "DSPCore.BuildBar.Summary";
    public const string Footer = "DSPCore.BuildBar.Footer";
    public const string OpenEditor = "DSPCore.BuildBar.OpenEditor";
    public const string Close = "DSPCore.BuildBar.Close";
    public const string ClearSlot = "DSPCore.BuildBar.ClearSlot";
    public const string UseDefault = "DSPCore.BuildBar.UseDefault";
    public const string SelectItem = "DSPCore.BuildBar.SelectItem";
    public const string RowLabel = "DSPCore.BuildBar.RowLabel";
    public const string EmptySlot = "DSPCore.BuildBar.EmptySlot";
    public const string ExplicitEmpty = "DSPCore.BuildBar.ExplicitEmpty";
    public const string CategoryPage = "DSPCore.BuildBar.CategoryPage";

    public static void RegisterBuiltInLocalizations()
    {
        Register(Title, "zhCN", "建造栏绑定");
        Register(Title, "enUS", "Build Bar Bindings");
        Register(Summary, "zhCN", "为当前存档覆盖快捷建造栏槽位。");
        Register(Summary, "enUS", "Override quick build bar slots for the current save.");
        Register(Footer, "zhCN", "玩家覆盖会写入当前存档的 .dspcore 数据。");
        Register(Footer, "enUS", "Player overrides are written to the current save's .dspcore data.");
        Register(OpenEditor, "zhCN", "建造栏绑定");
        Register(OpenEditor, "enUS", "Build Bar");
        Register(Close, "zhCN", "关闭");
        Register(Close, "enUS", "Close");
        Register(ClearSlot, "zhCN", "清空");
        Register(ClearSlot, "enUS", "Empty");
        Register(UseDefault, "zhCN", "默认");
        Register(UseDefault, "enUS", "Default");
        Register(SelectItem, "zhCN", "选择物品");
        Register(SelectItem, "enUS", "Select Item");
        Register(RowLabel, "zhCN", "第 {0} 行");
        Register(RowLabel, "enUS", "Row {0}");
        Register(EmptySlot, "zhCN", "空");
        Register(EmptySlot, "enUS", "Empty");
        Register(ExplicitEmpty, "zhCN", "已清空");
        Register(ExplicitEmpty, "enUS", "Emptied");
        Register(CategoryPage, "zhCN", "分类 {0}");
        Register(CategoryPage, "enUS", "Category {0}");
    }

    private static void Register(string key, string language, string value)
    {
        DspCore.Resources.RegisterLocalization(new LocalizationEntry(key, language, value, DSPCorePlugin.PluginGuid));
    }
}
