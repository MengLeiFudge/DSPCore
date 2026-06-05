using DSPCore;

namespace ExampleMod;

public static class BuildBarExample
{
    public static void Register(ItemProto myItemProto)
    {
        BuildBar.BindItem(tab: 3, row: 2, index: 4, itemId: 9554);
        BuildBar.BindItem(tab: 3, row: 2, index: 5, item: myItemProto);
    }

    public static void RegisterLegacy()
    {
#pragma warning disable CS0618
        BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
    }
}
