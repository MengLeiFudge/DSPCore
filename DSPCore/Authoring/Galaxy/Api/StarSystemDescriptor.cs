using System;

namespace DSPCore;

/// <summary>
/// 描述一个恒星级系统。
/// Describes a star-level system.
/// </summary>
/// <param name="SystemId">系统稳定 ID。Stable system ID.</param>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="Factory">系统工厂。System factory.</param>
public sealed record StarSystemDescriptor(string SystemId, string OwnerModGuid, Func<StarData, CoreStarSystem> Factory);
