# Soft Packet Example

Use this when a feature has multiplayer state but should not make DSPCore depend directly on Nebula.

The mod declares packet, host relay, planet-data, and missing-save boundaries. A separate adapter can read `Multiplayer.GetAdapterSnapshot()` or use the `TryGet...` methods and connect those declarations to its own network transport.

Use parameter overloads for ordinary declarations. Use descriptor overloads only for configuration-driven or batch registration.
