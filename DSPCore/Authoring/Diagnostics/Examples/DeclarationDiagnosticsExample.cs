using System.Collections.Generic;
using DSPCore;

internal static class DeclarationDiagnosticsExample
{
    public static void CheckAuthorRules()
    {
        // DSPCore runs built-in checks at startup. Authors can add mod-specific checks
        // to the same diagnostic stream when the rule is not visible from DSPCore registries.
        bool recipeHasMachine = false;
        if (!recipeHasMachine)
        {
            Diagnostics.Warn(
                ownerModGuid: "com.example.my-mod",
                code: "example.recipe.unreachable",
                message: "Recipe is registered but no machine can craft it.",
                subject: "recipe=9554");
        }

        IReadOnlyList<DiagnosticIssue> issues = Diagnostics.GetIssues();
        foreach (var issue in issues)
        {
            // Tests or debug UI can inspect the snapshot. Normal gameplay can rely on
            // BepInEx logs and Errors.BuildDiagnosticText().
            string searchableLine = issue.Code + " " + issue.Subject;
        }
    }

    public static IReadOnlyList<DiagnosticIssue> RunChecksInTests()
    {
        // Use this in author-side tests after registering protos, tabs, options, and resources.
        return Diagnostics.RunBuiltInChecks();
    }
}
