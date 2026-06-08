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

        Options.Register(
            section: "Example",
            key: "Mode",
            defaultValue: "Normal",
            description: "Example mode used by com.example.my-mod.",
            pageId: "com.example.settings");

        string mode = Options.GetString("Example", "Mode");
    }
}
