using System;
using System.Reflection;

namespace DSPCore;

/// <summary>
/// 描述错误诊断文本中的可选游戏对象上下文。
/// Describes optional game-object context included in diagnostic text.
/// </summary>
/// <param name="Note">上下文备注。Context note.</param>
/// <param name="PlanetId">星球 ID。Planet ID.</param>
/// <param name="PlanetName">星球名称。Planet name.</param>
/// <param name="EntityId">实体 ID。Entity ID.</param>
/// <param name="ProtoId">实体物品原型 ID。Entity item proto ID.</param>
/// <param name="ModelIndex">实体模型索引。Entity model index.</param>
/// <param name="FactoryEntityCursor">工厂实体游标。Factory entity cursor.</param>
public sealed record ErrorDiagnosticContext(
    string? Note = null,
    int? PlanetId = null,
    string? PlanetName = null,
    int? EntityId = null,
    int? ProtoId = null,
    int? ModelIndex = null,
    int? FactoryEntityCursor = null)
{
    /// <summary>
    /// 从星球工厂创建诊断上下文。
    /// Creates diagnostic context from a planet factory.
    /// </summary>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="note">可选备注。Optional note.</param>
    /// <returns>诊断上下文。Diagnostic context.</returns>
    public static ErrorDiagnosticContext ForFactory(PlanetFactory factory, string? note = null)
    {
        return new ErrorDiagnosticContext(
            Note: note,
            PlanetId: factory.planet?.id,
            PlanetName: ReadStringMember(factory.planet, "displayName") ?? ReadStringMember(factory.planet, "name"),
            FactoryEntityCursor: ReadIntMember(factory, "entityCursor"));
    }

    /// <summary>
    /// 从星球工厂和实体 ID 创建诊断上下文。
    /// Creates diagnostic context from a planet factory and entity ID.
    /// </summary>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="entityId">实体 ID。Entity ID.</param>
    /// <param name="note">可选备注。Optional note.</param>
    /// <returns>诊断上下文。Diagnostic context.</returns>
    public static ErrorDiagnosticContext ForEntity(PlanetFactory factory, int entityId, string? note = null)
    {
        var context = ForFactory(factory, note) with { EntityId = entityId };
        if (entityId <= 0 || factory.entityPool == null || entityId >= factory.entityPool.Length)
        {
            return context;
        }

        var entity = factory.entityPool[entityId];
        return context with
        {
            ProtoId = ReadIntMember(entity, "protoId"),
            ModelIndex = ReadIntMember(entity, "modelIndex")
        };
    }

    private static string? ReadStringMember(object? source, string name)
    {
        return ReadMember(source, name)?.ToString();
    }

    private static int? ReadIntMember(object? source, string name)
    {
        var value = ReadMember(source, name);
        if (value == null)
        {
            return null;
        }

        try
        {
            return Convert.ToInt32(value);
        }
        catch
        {
            return null;
        }
    }

    private static object? ReadMember(object? source, string name)
    {
        if (source == null)
        {
            return null;
        }

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var type = source.GetType();
        return type.GetField(name, flags)?.GetValue(source) ?? type.GetProperty(name, flags)?.GetValue(source, null);
    }
}
