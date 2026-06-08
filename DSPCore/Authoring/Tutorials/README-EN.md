# Tutorial Registration

Tutorials is the guide/tutorial proto registration entry point. It only represents the author-facing capability of registering `TutorialProto` or equivalent guide protos; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

`Tutorials.Register(...)` is currently a thin entry over `ProtoRegistration.RegisterTutorial(...)`.
