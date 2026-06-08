# 科技注册

Techs 是科技原型注册入口。它只表达“注册 TechProto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

当前 `Techs.Register(...)` 是 `ProtoRegistration.RegisterTech(...)` 的薄入口。
