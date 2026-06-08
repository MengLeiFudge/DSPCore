# Tech Registration

Techs is the tech proto registration entry point. It only represents the author-facing capability of registering `TechProto`; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

`Techs.Register(...)` is currently a thin entry over `ProtoRegistration.RegisterTech(...)`.
