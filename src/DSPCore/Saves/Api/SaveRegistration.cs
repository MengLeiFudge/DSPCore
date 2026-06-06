namespace DSPCore;

/// <summary>
/// 描述一个存档处理器注册。
/// Describes a save handler registration.
/// </summary>
/// <param name="ModGuid">模组 GUID。Mod GUID.</param>
/// <param name="Handler">存档处理器。Save handler.</param>
/// <param name="LoadOrder">加载顺序。Load order.</param>
public sealed record SaveRegistration(string ModGuid, ICoreSaveHandler Handler, CoreLoadOrder LoadOrder);
