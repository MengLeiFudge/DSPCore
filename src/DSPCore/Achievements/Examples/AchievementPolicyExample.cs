using DSPCore;

namespace ExampleMod;

public static class AchievementPolicyExample
{
    public static void Register()
    {
        Achievements.Declare(new AchievementPolicyDeclaration(
            ModGuid: "com.example.balance",
            DisableAchievements: true,
            Reason: "Changes balance",
            SourceVersion: "1.0.0"));

        Achievements.AllowPlatformAchievements = false;
        Achievements.AllowMilkyWayUpload = false;
        Achievements.MetadataMode = AchievementMetadataMode.DeclarationsOnly;
    }
}
