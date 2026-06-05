# Proto Phases / 原型阶段

DSPCore uses three data phases for new APIs.

DSPCore 的新 API 使用三层数据阶段。

```csharp
using DSPCore;

DspCore.Protos.RegisterItem(
    proto: itemProto,
    ownerModGuid: "com.example.my-mod",
    phase: CoreDataPhase.Data,
    purpose: "Declare the base item");

DspCore.Protos.RegisterRecipe(
    proto: recipeProto,
    ownerModGuid: "com.example.my-mod",
    phase: CoreDataPhase.DataUpdates,
    purpose: "Attach recipe after item declarations");

DspCore.Protos.RegisterTutorial(
    proto: tutorialProto,
    ownerModGuid: "com.example.my-mod",
    phase: CoreDataPhase.DataFinalFixes,
    purpose: "Final tutorial chain fix");
```

Runtime bridge note: the first implementation applies `Data` and `DataUpdates` in the prefix of `VFPreload.InvokeOnLoadWorkEnded`, then applies `DataFinalFixes` in the postfix and rebuilds key proto caches.

运行时说明：第一版会在 `VFPreload.InvokeOnLoadWorkEnded` prefix 执行 `Data` 和 `DataUpdates`，在 postfix 执行 `DataFinalFixes`，随后重建关键 Proto 缓存。
