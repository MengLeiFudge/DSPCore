using NebulaAPI.Interfaces;

namespace DSPCore.NebulaAdapter;

/// <summary>
/// 在 Nebula 中承载 DSPCore 稳定 ID 和字节负载的信封 packet。
/// Envelope packet that carries DSPCore stable ids and byte payloads through Nebula.
/// </summary>
public sealed class DSPCoreEnvelopePacket : INetSerializable
{
    /// <summary>
    /// 信封业务类型。
    /// Envelope business kind.
    /// </summary>
    public DSPCoreEnvelopeKind Kind { get; set; }

    /// <summary>
    /// packet 或请求的稳定 ID。
    /// Stable id for the packet or request.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 星球 ID；非星球数据包时为 0。
    /// Planet id; zero for non-planet-data packets.
    /// </summary>
    public int PlanetId { get; set; }

    /// <summary>
    /// DSPCore 负载字节。
    /// DSPCore payload bytes.
    /// </summary>
    public byte[] Payload { get; set; } = System.Array.Empty<byte>();

    /// <summary>
    /// 创建空信封，供 Nebula 反序列化使用。
    /// Creates an empty envelope for Nebula deserialization.
    /// </summary>
    public DSPCoreEnvelopePacket()
    {
    }

    /// <summary>
    /// 创建带稳定 ID 和负载的信封。
    /// Creates an envelope with a stable id and payload.
    /// </summary>
    public DSPCoreEnvelopePacket(DSPCoreEnvelopeKind kind, string id, byte[] payload, int planetId = 0)
    {
        Kind = kind;
        Id = id ?? string.Empty;
        Payload = payload ?? System.Array.Empty<byte>();
        PlanetId = planetId;
    }

    /// <summary>
    /// 写入 Nebula 二进制流。
    /// Writes the packet to a Nebula binary stream.
    /// </summary>
    public void Serialize(INetDataWriter writer)
    {
        writer.Put((int)Kind);
        writer.Put(Id);
        writer.Put(PlanetId);
        writer.PutBytesWithLength(Payload);
    }

    /// <summary>
    /// 从 Nebula 二进制流读取。
    /// Reads the packet from a Nebula binary stream.
    /// </summary>
    public void Deserialize(INetDataReader reader)
    {
        Kind = (DSPCoreEnvelopeKind)reader.GetInt();
        Id = reader.GetString();
        PlanetId = reader.GetInt();
        Payload = reader.GetBytesWithLength() ?? System.Array.Empty<byte>();
    }
}
