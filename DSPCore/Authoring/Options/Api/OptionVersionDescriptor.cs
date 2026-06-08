namespace DSPCore;

/// <summary>
/// 描述一个模组设置版本，用于联机或存档兼容检查。
/// Describes a mod settings version for multiplayer or save compatibility checks.
/// </summary>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="Version">设置版本。Settings version.</param>
public sealed record OptionVersionDescriptor(string OwnerModGuid, string Version);
