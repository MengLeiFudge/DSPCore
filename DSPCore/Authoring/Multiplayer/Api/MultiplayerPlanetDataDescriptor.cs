using System;

namespace DSPCore;

/// <summary>
/// 描述一个星球数据请求边界。
/// Describes a planet data request boundary.
/// </summary>
/// <param name="RequestId">请求稳定 ID。Stable request ID.</param>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="ExportPlanetData">主机导出星球数据。Host-side planet data export.</param>
/// <param name="ImportPlanetData">客户端导入星球数据。Client-side planet data import.</param>
public sealed record MultiplayerPlanetDataDescriptor(
    string RequestId,
    string OwnerModGuid,
    Func<int, byte[]> ExportPlanetData,
    Action<int, byte[]> ImportPlanetData);
