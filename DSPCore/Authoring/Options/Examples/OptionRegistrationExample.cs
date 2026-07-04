using DSPCore;

internal static class OptionRegistrationExample
{
    public static void Register()
    {
        // Page registers a DSPCore page inside the vanilla option window and returns a context. The page id must stay
        // stable because each option stores it as its grouping key.
        OptionSection settings = Options.Page(
            pageId: "com.example.settings",
            ownerModGuid: "com.example.my-mod",
            title: "Example Settings")
            .Section("Example");

        // RegisterVersion lets save or multiplayer compatibility checks compare settings
        // schemas without reading every individual option.
        Options.RegisterVersion(
            ownerModGuid: "com.example.my-mod",
            version: "1");

        // Short entries register the descriptor and return the current BepInEx config value.
        // Use them for ordinary bool, int, float, and string settings.
        bool enabled = settings.Bool(
            key: "Enabled",
            defaultValue: true,
            description: "Enable example behavior.",
            ui: new OptionUi(
                DisplayName: "Enable Example")
            {
                Order = 10,
                CanReset = true
            });

        int rowCount = settings.Int(
            key: "Rows",
            defaultValue: 2,
            description: "Example row count.");

        float uiScale = settings.Float(
            key: "UiScale",
            defaultValue: 1.0f,
            description: "Example UI scale.");

        ExampleDisplayMode displayMode = settings.Enum(
            key: "DisplayMode",
            defaultValue: ExampleDisplayMode.Normal,
            description: "Example display mode.");

        int maxRows = settings.IntRange(
            key: "MaxRows",
            defaultValue: 3,
            description: "Maximum example rows shown in the UI.",
            minimum: 1,
            maximum: 6,
            ui: new OptionUi(
                DisplayName: "Maximum Rows")
            {
                Order = 20,
                CanReset = true
            });

        float opacity = settings.FloatRange(
            key: "Opacity",
            defaultValue: 0.8f,
            description: "Example panel opacity.",
            minimum: 0.2f,
            maximum: 1.0f,
            step: 0.05f);

        string mode = settings.String(
            key: "Mode",
            defaultValue: "Normal",
            description: "Example mode used by com.example.my-mod.");
    }

    public static void OpenFromButtonOrKey()
    {
        // Call OpenWindow only from a button, key bind, or custom UI callback after UIRoot exists.
        // Calling it during early plugin startup logs a warning and does not open the vanilla option window.
        Options.OpenWindow();
    }

    public static string ExportForClipboard()
    {
        // ExportText returns a stable machine-readable text block that can be stored,
        // copied, or passed back to Options.ImportText later.
        return Options.ExportText();
    }

    public static OptionImportReport ImportFromClipboard(string exportedText)
    {
        // ImportText only applies values for currently registered and bound options.
        // Unknown lines and unavailable config entries are reported instead of being ignored.
        return Options.ImportText(exportedText);
    }

    private enum ExampleDisplayMode
    {
        Compact,
        Normal,
        Detailed
    }
}
