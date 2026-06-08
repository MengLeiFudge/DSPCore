# Proto Access

ProtoAccess owns the author-facing capability of inspecting and modifying data registered by other mods during the second and third data phases.

The current `VanillaDataView` is still a descriptive read view, not a full query/mutation API for cross-mod registered data. This directory is split out now so access does not stay buried inside ProtoRegistration; future work should add mutable phase context or query models here.
