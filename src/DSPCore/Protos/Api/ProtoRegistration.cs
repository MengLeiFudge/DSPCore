using System;

namespace DSPCore;

/// <summary>
/// 描述一个 Proto 注册请求。
/// Describes a Proto registration request.
/// </summary>
/// <param name="ProtoType">Proto 类型。Proto type.</param>
/// <param name="Proto">Proto 对象。Proto object.</param>
/// <param name="OwnerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
/// <param name="Phase">注册阶段。Registration phase.</param>
/// <param name="Kind">原型功能类型。Proto feature kind.</param>
/// <param name="Purpose">注册目的说明。Registration purpose.</param>
public sealed record ProtoRegistration(
    Type ProtoType,
    object Proto,
    string OwnerModGuid,
    CoreDataPhase Phase,
    ProtoKind Kind = ProtoKind.Unknown,
    string? Purpose = null);
