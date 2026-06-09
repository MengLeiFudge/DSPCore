using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DSPCore;

internal sealed class AutoSaveHandler<TState> : ICoreSaveHandler where TState : class
{
    private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private readonly string modGuid;
    private readonly TState state;
    private readonly int version;
    private readonly Action<int, TState>? migrate;
    private readonly Action<TState>? intoOtherSave;
    private readonly List<MemberBinding> members;
    private readonly Dictionary<string, object?> defaults;

    public AutoSaveHandler(
        string modGuid,
        TState state,
        int version,
        Action<int, TState>? migrate,
        Action<TState>? intoOtherSave)
    {
        this.modGuid = modGuid;
        this.state = state ?? throw new ArgumentNullException(nameof(state));
        if (version < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version), "Save schema version cannot be negative.");
        }

        this.version = version;
        this.migrate = migrate;
        this.intoOtherSave = intoOtherSave;
        members = DiscoverMembers(typeof(TState));
        defaults = members.ToDictionary(member => member.Tag, member => member.GetValue(state), StringComparer.Ordinal);
    }

    public void Export(BinaryWriter writer)
    {
        writer.Write(version);
        SaveBlockFormat.WriteBlocks(writer, CreateBlocks());
    }

    public void Import(BinaryReader reader)
    {
        RestoreDefaults();
        int savedVersion = reader.ReadInt32();
        SaveBlockFormat.ReadBlocks(reader, CreateBlocks(), ReportBlockError);
        migrate?.Invoke(savedVersion, state);
    }

    public void IntoOtherSave()
    {
        RestoreDefaults();
        intoOtherSave?.Invoke(state);
    }

    private IEnumerable<SaveBlock> CreateBlocks()
    {
        foreach (var member in members)
        {
            yield return new SaveBlock(
                member.Tag,
                writer => WriteValue(writer, member.ValueType, member.GetValue(state)),
                reader => member.SetValue(state, ReadValue(reader, member.ValueType)));
        }
    }

    private void RestoreDefaults()
    {
        foreach (var member in members)
        {
            member.SetValue(state, defaults[member.Tag]);
        }
    }

    private void ReportBlockError(string tag, Exception exception)
    {
        DspCore.Errors.ReportException(modGuid, exception);
        DspCore.Logger?.LogError($"{modGuid} auto save field {tag} import failed: {exception}");
    }

    private static List<MemberBinding> DiscoverMembers(Type stateType)
    {
        var bindings = new List<MemberBinding>();
        foreach (var field in stateType.GetFields(MemberFlags))
        {
            var attribute = field.GetCustomAttribute<CoreSaveFieldAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (field.IsInitOnly)
            {
                throw new InvalidOperationException($"Auto save field {stateType.FullName}.{field.Name} is readonly.");
            }

            bindings.Add(MemberBinding.ForField(attribute, field));
        }

        foreach (var property in stateType.GetProperties(MemberFlags))
        {
            var attribute = property.GetCustomAttribute<CoreSaveFieldAttribute>();
            if (attribute == null)
            {
                continue;
            }

            if (property.GetIndexParameters().Length != 0)
            {
                throw new InvalidOperationException($"Auto save property {stateType.FullName}.{property.Name} cannot be indexed.");
            }

            if (property.GetGetMethod(true) == null || property.GetSetMethod(true) == null)
            {
                throw new InvalidOperationException($"Auto save property {stateType.FullName}.{property.Name} must have getter and setter.");
            }

            bindings.Add(MemberBinding.ForProperty(attribute, property));
        }

        var duplicates = bindings.GroupBy(member => member.Tag, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicates != null)
        {
            throw new InvalidOperationException($"Auto save tag {duplicates.Key} is declared more than once on {stateType.FullName}.");
        }

        if (bindings.Count == 0)
        {
            throw new InvalidOperationException($"Auto save state {stateType.FullName} does not declare any [CoreSaveField] members.");
        }

        return bindings.OrderBy(member => member.Order).ThenBy(member => member.Tag, StringComparer.Ordinal).ToList();
    }

    private static void EnsureSupportedType(Type type, string memberName)
    {
        var valueType = Nullable.GetUnderlyingType(type) ?? type;
        if (valueType == typeof(bool) ||
            valueType == typeof(int) ||
            valueType == typeof(long) ||
            valueType == typeof(float) ||
            valueType == typeof(double) ||
            valueType == typeof(string) ||
            valueType.IsEnum)
        {
            return;
        }

        throw new InvalidOperationException($"Auto save member {memberName} has unsupported type {type.FullName}.");
    }

    private static void WriteValue(BinaryWriter writer, Type type, object? value)
    {
        var valueType = Nullable.GetUnderlyingType(type) ?? type;
        bool isNullable = Nullable.GetUnderlyingType(type) != null;
        if (isNullable)
        {
            writer.Write(value != null);
            if (value == null)
            {
                return;
            }
        }

        if (valueType == typeof(bool))
        {
            writer.Write((bool)value!);
        }
        else if (valueType == typeof(int))
        {
            writer.Write((int)value!);
        }
        else if (valueType == typeof(long))
        {
            writer.Write((long)value!);
        }
        else if (valueType == typeof(float))
        {
            writer.Write((float)value!);
        }
        else if (valueType == typeof(double))
        {
            writer.Write((double)value!);
        }
        else if (valueType == typeof(string))
        {
            writer.Write((string?)value ?? string.Empty);
        }
        else if (valueType.IsEnum)
        {
            writer.Write(Convert.ToInt64(value, CultureInfo.InvariantCulture));
        }
        else
        {
            throw new InvalidOperationException($"Unsupported auto save value type {type.FullName}.");
        }
    }

    private static object? ReadValue(BinaryReader reader, Type type)
    {
        var valueType = Nullable.GetUnderlyingType(type) ?? type;
        bool isNullable = Nullable.GetUnderlyingType(type) != null;
        if (isNullable && !reader.ReadBoolean())
        {
            return null;
        }

        if (valueType == typeof(bool))
        {
            return reader.ReadBoolean();
        }

        if (valueType == typeof(int))
        {
            return reader.ReadInt32();
        }

        if (valueType == typeof(long))
        {
            return reader.ReadInt64();
        }

        if (valueType == typeof(float))
        {
            return reader.ReadSingle();
        }

        if (valueType == typeof(double))
        {
            return reader.ReadDouble();
        }

        if (valueType == typeof(string))
        {
            return reader.ReadString();
        }

        if (valueType.IsEnum)
        {
            return Enum.ToObject(valueType, reader.ReadInt64());
        }

        throw new InvalidOperationException($"Unsupported auto save value type {type.FullName}.");
    }

    private sealed class MemberBinding
    {
        private readonly Func<TState, object?> getValue;
        private readonly Action<TState, object?> setValue;

        private MemberBinding(
            string tag,
            int order,
            Type valueType,
            Func<TState, object?> getValue,
            Action<TState, object?> setValue)
        {
            Tag = tag;
            Order = order;
            ValueType = valueType;
            this.getValue = getValue;
            this.setValue = setValue;
        }

        public string Tag { get; }

        public int Order { get; }

        public Type ValueType { get; }

        public static MemberBinding ForField(CoreSaveFieldAttribute attribute, FieldInfo field)
        {
            EnsureSupportedType(field.FieldType, field.Name);
            return new MemberBinding(
                attribute.Tag,
                attribute.Order,
                field.FieldType,
                state => field.GetValue(state),
                (state, value) => field.SetValue(state, value));
        }

        public static MemberBinding ForProperty(CoreSaveFieldAttribute attribute, PropertyInfo property)
        {
            EnsureSupportedType(property.PropertyType, property.Name);
            return new MemberBinding(
                attribute.Tag,
                attribute.Order,
                property.PropertyType,
                state => property.GetValue(state, null),
                (state, value) => property.SetValue(state, value, null));
        }

        public object? GetValue(TState state)
        {
            return getValue(state);
        }

        public void SetValue(TState state, object? value)
        {
            setValue(state, value);
        }
    }
}
