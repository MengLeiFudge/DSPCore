using System;

namespace DSPCore;

/// <summary>
/// 描述一个原型注册请求。
/// Describes a proto registration request.
/// </summary>
/// <param name="ProtoType">Proto 类型。Proto type.</param>
/// <param name="Proto">Proto 对象。Proto object.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="Phase">注册阶段。Registration phase.</param>
/// <param name="Kind">原型功能类型。Proto feature kind.</param>
/// <param name="Purpose">注册目的说明。Registration purpose.</param>
/// <param name="StableId">稳定 Proto 身份；为空时沿用传入的 int ID。Stable proto identity; null keeps the supplied int ID.</param>
public sealed record ProtoRegistrationEntry(
    Type ProtoType,
    object Proto,
    string OwnerModGuid,
    CoreDataPhase Phase,
    ProtoKind Kind = ProtoKind.Unknown,
    string? Purpose = null,
    ProtoStableId? StableId = null);
