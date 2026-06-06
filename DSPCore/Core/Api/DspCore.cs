namespace DSPCore;

using BepInEx.Logging;

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
    /// 初始化 DSPCore 的功能块和模块注册表。
    /// Initializes DSPCore feature blocks and module registries.
    /// </summary>
    public static void Initialize()
    {
        InitializeRuntime(null);
    }

    /// <summary>
    /// 指示 DSPCore 是否已经初始化。
    /// Indicates whether DSPCore has been initialized.
    /// </summary>
    public static bool IsInitialized { get; private set; }

    internal static ManualLogSource? Logger { get; private set; }

    internal static void InitializeRuntime(ManualLogSource? logger)
    {
        if (IsInitialized)
        {
            Logger ??= logger;
            return;
        }

        Logger = logger;
        RegisterBuiltInFeatures();
        foreach (var feature in Features.GetAll())
        {
            feature.Initialize();
        }

        foreach (var module in Modules.GetAll())
        {
            module.Initialize();
        }

        IsInitialized = true;
    }

    private static void RegisterBuiltInFeatures()
    {
        RegisterFeature("core.lifecycle", "Module lifecycle / 模块生命周期", 0);
        RegisterFeature("core.protos", "Proto runtime bridge / 原型运行时桥接", 10);
        RegisterFeature("core.resources", "Resource registry / 资源注册", 20);
        RegisterFeature("core.build-bar", "Build bar placement / 建造栏位置", 30);
        RegisterFeature("core.saves", "Save sidecar bridge / 存档桥接", 40);
        RegisterFeature("core.achievements", "Achievement policy / 成就策略", 50);
        RegisterFeature("core.errors", "Error diagnostics / 错误诊断", 60);
        RegisterFeature("core.compat", "Feature compatibility shims / 功能块兼容", 100);
    }

    private static void RegisterFeature(string id, string displayName, int priority)
    {
        if (!Features.TryGet(id, out _))
        {
            Features.Register(new FeatureDescriptor(id, displayName, priority, static () => { }));
        }
    }
}
