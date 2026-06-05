using DSPCore;

namespace ExampleMod;

public static class CompatibilityPatchExample
{
    public static void Register()
    {
        Compatibility.Register(new CompatibilityPatchDescriptor(
            Id: "tutorial-fractionation-compat",
            TargetModGuid: "com.menglei.dsp.fe",
            TargetVersionRange: ">=2.3.0",
            Reason: "Tutorial flow needs a shared compatibility patch",
            Apply: ApplyTutorialPatch));
    }

    private static void ApplyTutorialPatch()
    {
    }
}
