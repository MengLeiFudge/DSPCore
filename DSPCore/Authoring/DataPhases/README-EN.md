# Data Phases

DataPhases owns the author-facing three-phase lifecycle: `Data`, `DataUpdates`, and `DataFinalFixes`.

- `Data`: initial declarations, usually for a mod's own item, recipe, tech, tutorial, and other protos.
- `DataUpdates`: cross-mod adjustments, where authors can inspect and modify data registered by earlier phases through `ProtoPhaseContext.FindItem(...)` / `FindRecipe(...)` or `data.Access`.
- `DataFinalFixes`: final fixes before system insertion and cache rebuilds.

For specific proto type registration, prefer Items, Recipes, Techs, and Tutorials. For cross-mod inspection/modification, use ProtoAccess. The low-level compatibility and aggregate entries remain under ProtoRegistration.
