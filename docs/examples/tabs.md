# Tabs / 分页

Authors declare one tab. DSPCore runtime adapters will project it to supported UI surfaces.

作者只声明一个分页。DSPCore 运行时适配层会把它投射到受支持的 UI 表面。

```csharp
using DSPCore;

DspCore.Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "Example Machines",
    IconId: "example-tab-icon",
    Order: 100));
```

Target UI surfaces include item picker, recipe picker, replicator, signal picker, beacon and blueprint icon selection where supported.

目标 UI 表面包括物品选择器、配方选择器、制造器、信号选择器、信标和蓝图图标选择等受支持场景。
