namespace DSPCore;

/// <summary>
/// DSPCore 的全局入口，提供框架版本、服务注册表和一次性初始化入口。
/// Global entry point for DSPCore, providing framework version, service registries, and one-time initialization.
/// </summary>
public static class DspCore
{
    /// <summary>
    /// 当前 DSPCore 标准版本。
    /// Current DSPCore standard version.
    /// </summary>
    public const string Version = "0.1.0";

    /// <summary>
    /// 全局功能块注册表。
    /// Global feature block registry.
    /// </summary>
    public static FeatureRegistry Features { get; } = new();

    /// <summary>
    /// 全局模块注册表。
    /// Global module registry.
    /// </summary>
    public static ModuleRegistry Modules { get; } = new();

    /// <summary>
    /// 全局补丁注册表。
    /// Global patch registry.
    /// </summary>
    public static PatchRegistry Patches { get; } = new();

    /// <summary>
    /// 全局存档注册表。
    /// Global save registry.
    /// </summary>
    public static SaveRegistry Saves { get; } = new();

    /// <summary>
    /// 全局 Proto 注册门面。
    /// Global Proto registry facade.
    /// </summary>
    public static ProtoRegistryFacade Protos { get; } = new();

    /// <summary>
    /// 全局原版数据读取视图。
    /// Global vanilla data read view.
    /// </summary>
    public static VanillaDataView Vanilla { get; } = new();

    /// <summary>
    /// 全局建造栏注册表。
    /// Global build bar registry.
    /// </summary>
    public static BuildBarRegistry BuildBar { get; } = new();

    /// <summary>
    /// 全局资源和本地化注册表。
    /// Global resource and localization registry.
    /// </summary>
    public static ResourceRegistry Resources { get; } = new();

    /// <summary>
    /// 全局图标注册表。
    /// Global icon set registry.
    /// </summary>
    public static IconSetRegistry Icons { get; } = new();

    /// <summary>
    /// 全局分页注册表。
    /// Global tab registry.
    /// </summary>
    public static TabRegistry Tabs { get; } = new();

    /// <summary>
    /// 全局选择器注册表。
    /// Global picker registry.
    /// </summary>
    public static PickerRegistry Pickers { get; } = new();

    /// <summary>
    /// 全局自定义配方类型注册表。
    /// Global custom recipe type registry.
    /// </summary>
    public static RecipeTypeRegistry RecipeTypes { get; } = new();

    /// <summary>
    /// 全局按键注册表。
    /// Global key binding registry.
    /// </summary>
    public static KeyBindRegistry KeyBinds { get; } = new();

    /// <summary>
    /// 全局成就策略注册表。
    /// Global achievement policy registry.
    /// </summary>
    public static AchievementPolicyRegistry Achievements { get; } = new();

    /// <summary>
    /// 全局错误报告器。
    /// Global error reporter.
    /// </summary>
    public static ErrorReporter Errors { get; } = new();

    /// <summary>
    /// 全局兼容补丁注册表。
    /// Global compatibility patch registry.
    /// </summary>
    public static CompatibilityPatchRegistry Compatibility { get; } = new();

    /// <summary>
    /// 初始化 DSPCore。初版中此方法只完成幂等标记，真实 BepInEx/Harmony 接入将在后续阶段填充。
    /// Initializes DSPCore. In this first version this only marks initialization idempotently; real BepInEx/Harmony wiring will be added later.
    /// </summary>
    public static void Initialize()
    {
        IsInitialized = true;
    }

    /// <summary>
    /// 指示 DSPCore 是否已经初始化。
    /// Indicates whether DSPCore has been initialized.
    /// </summary>
    public static bool IsInitialized { get; private set; }
}
