# 配方注册

Recipes 是配方原型注册入口。它只表达“注册 RecipeProto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

当前 `Recipes.Register(...)` 是 `ProtoRegistration.RegisterRecipe(...)` 的薄入口。配方类型或机器可用性限制不属于这里，放在 GameEnums。
