using DSPCore;

namespace ExampleMod;

public static class TabsExample
{
    public static void Register()
    {
        Tabs.AddTab(new CoreTabDescriptor(
            Id: "example-machines",
            OwnerModGuid: "com.example.my-mod",
            Title: "Example Machines",
            IconId: "example-tab-icon",
            Order: 100));
    }
}
