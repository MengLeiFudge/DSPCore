# Item Registration

Items is the item proto registration entry point. It only represents the author-facing capability of registering `ItemProto`; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

`Items.Register(...)` is currently a thin entry over `ProtoRegistration.RegisterItem(...)` so item registration is no longer hidden inside the broad ProtoRegistration bucket. Existing `ProtoRegistration.RegisterItem(...)` remains available.
