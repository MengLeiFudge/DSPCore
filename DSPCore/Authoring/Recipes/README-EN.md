# Recipe Registration

Recipes is the recipe proto registration entry point. It only represents the author-facing capability of registering `RecipeProto`; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

`Recipes.Register(...)` is currently a thin entry over `ProtoRegistration.RegisterRecipe(...)`. Recipe type or machine-use restrictions are not owned here; they belong to GameEnums.
