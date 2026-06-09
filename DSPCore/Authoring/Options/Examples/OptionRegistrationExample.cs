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
            pageId: "com.example.settings");

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
}
