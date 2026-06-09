using DSPCore;

internal static class OptionRegistrationExample
{
    public static void Register()
    {
        Options.RegisterPage(
            pageId: "com.example.settings",
            ownerModGuid: "com.example.my-mod",
            title: "Example Settings");

        Options.RegisterVersion(
            ownerModGuid: "com.example.my-mod",
            version: "1");

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
}
