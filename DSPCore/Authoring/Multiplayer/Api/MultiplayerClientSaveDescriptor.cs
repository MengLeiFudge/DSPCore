using System;

namespace DSPCore;

/// <summary>
/// 描述客户端缺失联机存档数据时的初始化边界。
/// Describes the initialization boundary used when a client has no multiplayer save data.
/// </summary>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="IntoOtherSave">初始化回调。Initialization callback.</param>
public sealed record MultiplayerClientSaveDescriptor(string OwnerModGuid, Action IntoOtherSave);
