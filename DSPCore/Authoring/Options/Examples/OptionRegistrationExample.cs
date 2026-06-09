using DSPCore;

internal static class OptionRegistrationExample
{
    public static void Register()
    {
        // RegisterPage groups rows in the DSPCore unified settings window. The page id must
        // stay stable because each option stores it as its grouping key.
        Options.RegisterPage(
            pageId: "com.example.settings",
            ownerModGuid: "com.example.my-mod",
            title: "Example Settings");

        // RegisterVersion lets save or multiplayer compatibility checks compare settings
        // schemas without reading every individual option.
        Options.RegisterVersion(
            ownerModGuid: "com.example.my-mod",
            version: "1");

        // Short entries register the descriptor and return the current BepInEx config value.
        // Use them for ordinary bool, int, float, and string settings.
        bool enabled = Options.Bool(
            section: "Example",
            key: "Enabled",
            defaultValue: true,
            description: "Enable example behavior.",
            ui: new OptionUi(
                PageId: "com.example.settings",
                DisplayName: "Enable Example")
            {
                Order = 10,
                CanReset = true
            });

        int rowCount = Options.Int(
            section: "Example",
            key: "Rows",
            defaultValue: 2,
            description: "Example row count.",
            pageId: "com.example.settings");

        float uiScale = Options.Float(
            section: "Example",
            key: "UiScale",
            defaultValue: 1.0f,
            description: "Example UI scale.",
            pageId: "com.example.settings");

        ExampleDisplayMode displayMode = Options.Enum(
            section: "Example",
            key: "DisplayMode",
            defaultValue: ExampleDisplayMode.Normal,
            description: "Example display mode.",
            pageId: "com.example.settings");

        int maxRows = Options.IntRange(
            section: "Example",
            key: "MaxRows",
            defaultValue: 3,
            description: "Maximum example rows shown in the UI.",
            minimum: 1,
            maximum: 6,
            ui: new OptionUi(
                PageId: "com.example.settings",
                DisplayName: "Maximum Rows")
            {
                Order = 20,
                CanReset = true
            });

        float opacity = Options.FloatRange(
            section: "Example",
            key: "Opacity",
            defaultValue: 0.8f,
            description: "Example panel opacity.",
            minimum: 0.2f,
            maximum: 1.0f,
            step: 0.05f,
            pageId: "com.example.settings");

        string mode = Options.String(
            section: "Example",
            key: "Mode",
            defaultValue: "Normal",
            description: "Example mode used by com.example.my-mod.",
            pageId: "com.example.settings");
    }

    public static void OpenFromButtonOrKey()
    {
        // Call OpenWindow from a button, key bind, or custom UI callback after UIRoot exists.
        // Calling it during early plugin startup logs a warning and does not create a window.
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
