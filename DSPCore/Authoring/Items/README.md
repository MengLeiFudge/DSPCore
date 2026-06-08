# 物品注册

Items 是物品原型注册入口。它只表达“注册 ItemProto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

当前 `Items.Register(...)` 是 `ProtoRegistration.RegisterItem(...)` 的薄入口，用于把物品注册从大的 ProtoRegistration 概念中拆出来。已有 `ProtoRegistration.RegisterItem(...)` 继续保留。
