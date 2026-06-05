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

Runtime wiring is not implemented yet.

运行时接入尚未实现。
