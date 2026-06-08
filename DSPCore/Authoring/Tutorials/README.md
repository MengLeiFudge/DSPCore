# 指引注册

Tutorials 是指引或教程原型注册入口。它只表达“注册 TutorialProto 或同类指引 proto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

当前 `Tutorials.Register(...)` 是 `ProtoRegistration.RegisterTutorial(...)` 的薄入口。
