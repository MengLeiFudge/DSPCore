# ProtoAccess Example

Use this when a mod needs to inspect or adjust protos declared by the game, by DSPCore, or by another mod in an earlier data phase.

Register the adjustment in `DataUpdates` when it depends on normal data declarations. Use `DataFinalFixes` only for final cross-mod cleanup before derived caches are rebuilt.

Common lookups use `data.FindItem(...)`, `data.FindRecipe(...)`, `data.FindTech(...)`, and `data.FindTutorial(...)`. Full enumeration is available through `data.Access.Items()` and the matching recipe/tech/tutorial methods.

Returned objects are live proto references. Mutating them changes the object DSPCore will write into LDB or the object that is already in LDB, so keep changes narrow and explain why they belong in that phase.
