# Tabs / 分页

Authors declare one tab. The current runtime projects tabs to item picker, recipe picker, and replicator surfaces through the vanilla GridIndex category flow.

作者只声明一个分页。当前运行时通过原版 GridIndex 分类流程，把分页投射到物品选择器、配方选择器和制造器界面。

```csharp
using DSPCore;

DspCore.Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "Example Machines",
    IconId: "example-tab-icon",
    Order: 100));
```

Signal picker, beacon, blueprint, and other UI surfaces need a richer tab-content model before DSPCore can support them correctly.

信号选择器、全息信标、蓝图等界面需要更完整的分页内容模型后，DSPCore 才能正确支持。
