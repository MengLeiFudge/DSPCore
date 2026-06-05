# Build Bar / 建造栏

BuildBar only binds an item id or `ItemProto` to a shortcut slot. The slot is `tab`, `row`, and `index`.

BuildBar 只负责把物品 ID 或 `ItemProto` 绑定到快捷栏槽位。槽位由 `tab`、`row` 和 `index` 组成。

```csharp
using DSPCore;

DspCore.BuildBar.BindItem(tab: 3, row: 2, index: 4, itemId: 9554);
DspCore.BuildBar.BindItem(tab: 3, row: 2, index: 5, item: myItemProto);
```

Other feature blocks should call this API when they create or modify an item that needs a shortcut entry.

其他功能块在创建或修改需要快捷栏入口的物品时，调用这个 API 即可。

Legacy BuildBarTool calls are still accepted but obsolete.

旧 BuildBarTool 调用仍可使用，但已废弃。

```csharp
#pragma warning disable CS0618
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
```
