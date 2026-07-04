using System;
using System.Collections.Generic;
using System.IO;

namespace DSPCore;

public class Proto
{
    public int ID { get; set; }

    public string Name = string.Empty;
    public string name = string.Empty;
    public string sid = string.Empty;

    public string SID => Name;
}

public enum EItemType
{
    Unknown = 0
}

public enum ERecipeType
{
    None = 0,
    Assemble = 1,
    Custom = GameEnums.CustomRecipeTypeValue
}

#pragma warning disable CS0649
public sealed class ItemProto : Proto
{
    public static int InitPowerFacilityIndicesCalls;
    public static int InitProductionMaskCalls;
    public static bool ThrowOnInitFighterIndices;
    public static bool ThrowOnNullPrefabDescInCacheSteps;

    public EItemType Type { get; set; }
    public PrefabDesc? prefabDesc;
    public int consumptionMask;
    public int productionMask;
    private UnityEngine.Sprite? _iconSprite;

    public UnityEngine.Sprite? IconSprite => _iconSprite;

    public static void InitFuelNeeds() { }

    public static void InitTurretNeeds() { }

    public static void InitFluids() { }

    public static void InitTurrets() { }

    public static void InitEnemyDropTables() { }

    public static void InitConstructableItems() { }

    public static void InitItemIds() { }

    public static void InitItemIndices() { }

    public static void InitMechaMaterials() { }

    public static void InitFighterIndices()
    {
        if (ThrowOnInitFighterIndices)
        {
            throw new InvalidOperationException("fighter indices failed");
        }

        ThrowIfAnyPrefabDescIsNull();
    }

    public static void InitPowerFacilityIndices()
    {
        ThrowIfAnyPrefabDescIsNull();
        InitPowerFacilityIndicesCalls++;
    }

    public static void InitProductionMask()
    {
        foreach (var recipe in LDB.recipes.dataArray)
        {
            if (recipe.Type == ERecipeType.None)
            {
                continue;
            }

            foreach (var itemId in recipe.Items)
            {
                var item = LDB.items.Select(itemId);
                if (item != null)
                {
                    item.consumptionMask |= 1;
                }
            }

            foreach (var itemId in recipe.Results)
            {
                var item = LDB.items.Select(itemId);
                if (item != null)
                {
                    item.productionMask |= 1;
                }
            }
        }

        ThrowIfAnyPrefabDescIsNull();
        InitProductionMaskCalls++;
    }

    private static void ThrowIfAnyPrefabDescIsNull()
    {
        if (!ThrowOnNullPrefabDescInCacheSteps)
        {
            return;
        }

        foreach (var item in LDB.items.dataArray)
        {
            if (item == null || item.prefabDesc == null)
            {
                throw new InvalidOperationException("null prefabDesc was not filtered");
            }
        }
    }
}

public sealed class RecipeProto : Proto
{
    public static Dictionary<int, RecipeExecuteData> recipeExecuteData = new();
    public static int InitRecipeItemsCalls;
    public static int InitFractionatorNeedsCalls;

    public ERecipeType Type { get; set; }
    private UnityEngine.Sprite? _iconSprite;
    public int[] Items = Array.Empty<int>();
    public int[] ItemCounts = Array.Empty<int>();
    public int[] Results = Array.Empty<int>();
    public int[] ResultCounts = Array.Empty<int>();
    public int TimeSpend;
    public bool productive;

    public UnityEngine.Sprite? IconSprite => _iconSprite;

    public static void InitRecipeItems()
    {
        InitRecipeItemsCalls++;
    }

    public static void InitFractionatorNeeds()
    {
        InitFractionatorNeedsCalls++;
    }
}

public sealed class ModelProto : Proto
{
    public static int InitMaxModelIndexCalls;
    public static int InitModelIndicesCalls;
    public static int InitModelOrdersCalls;

    public int customValue;
    public PrefabDesc? prefabDesc;

    public static void InitMaxModelIndex()
    {
        InitMaxModelIndexCalls++;
    }

    public static void InitModelIndices()
    {
        InitModelIndicesCalls++;
    }

    public static void InitModelOrders()
    {
        InitModelOrdersCalls++;
    }
}

public sealed class TechProto : Proto
{
    private UnityEngine.Sprite? _iconSprite;

    public UnityEngine.Sprite? IconSprite => _iconSprite;
}

public sealed class TutorialProto : Proto
{
    private UnityEngine.Sprite? _iconSprite;

    public UnityEngine.Sprite? IconSprite => _iconSprite;
}

public sealed class SignalProto : Proto
{
    private UnityEngine.Sprite? _iconSprite;

    public UnityEngine.Sprite? IconSprite => _iconSprite;
}
#pragma warning restore CS0649

public sealed class RecipeExecuteData
{
    public RecipeExecuteData(
        int[] items,
        int[] itemCounts,
        int[] results,
        int[] resultCounts,
        int timeSpendTicks,
        int timeSpendScaled,
        bool productive)
    {
        Items = items;
        ItemCounts = itemCounts;
        Results = results;
        ResultCounts = resultCounts;
        TimeSpendTicks = timeSpendTicks;
        TimeSpendScaled = timeSpendScaled;
        Productive = productive;
    }

    public int[] Items { get; }

    public int[] ItemCounts { get; }

    public int[] Results { get; }

    public int[] ResultCounts { get; }

    public int TimeSpendTicks { get; }

    public int TimeSpendScaled { get; }

    public bool Productive { get; }
}

public static class SignalProtoSet
{
    public static int InitSignalKeyIdPairsCalls;

    public static void InitSignalKeyIdPairs()
    {
        InitSignalKeyIdPairsCalls++;
    }
}

public sealed class PrefabDesc
{
    public static PrefabDesc none = new();

    public int modelIndex;
    public int customValue;
}

public struct EntityData
{
    public int id;
    public int protoId;
    public int modelIndex;
    public int customId;
    public int customType;
}

public sealed class PlanetData
{
    public int id;
    public PlanetFactory? factory;
}

public sealed class StarData
{
    public int id;
}

public sealed class GalaxyData
{
    public StarData[]? stars;
}

public sealed class PlanetFactory
{
    public static PrefabDesc[]? PrefabDescByModelIndex;
    public static int InitPrefabDescArrayCalls;

    public PlanetData? planet;

    public EntityData[] entityPool = Array.Empty<EntityData>();

    public static void InitPrefabDescArray()
    {
        InitPrefabDescArrayCalls++;
        var maxModelIndex = 0;
        foreach (var model in LDB.models.dataArray)
        {
            if (model?.prefabDesc != null && model.ID > maxModelIndex)
            {
                maxModelIndex = model.ID;
            }
        }

        PrefabDescByModelIndex = new PrefabDesc[maxModelIndex + 1];
        foreach (var model in LDB.models.dataArray)
        {
            if (model?.prefabDesc != null && model.ID >= 0 && model.ID < PrefabDescByModelIndex.Length)
            {
                PrefabDescByModelIndex[model.ID] = model.prefabDesc;
            }
        }
    }

    public void CreateEntityLogicComponents(int entityId, PrefabDesc desc, int prebuildId)
    {
    }

    public void RemoveEntityWithComponents(int id)
    {
    }
}

public struct BuildingParameters
{
    public int[] parameters;

    public bool CopyFromFactoryObject(int objectId, PlanetFactory factory)
    {
        return true;
    }

    public void FromParamsArray(int[] _parameters)
    {
        parameters = _parameters;
    }

    public int[] ToParamsArray(out int _paramCount)
    {
        _paramCount = parameters?.Length ?? 0;
        return parameters ?? Array.Empty<int>();
    }

    public bool CanPasteToFactoryObject(int objectId, PlanetFactory factory)
    {
        return true;
    }

    public void PasteToFactoryObject(int objectId, PlanetFactory factory)
    {
    }

    public void ApplyPrebuildParametersToEntity(int entityId, PlanetFactory factory)
    {
    }
}

public sealed class ProtoSet<T>
    where T : Proto
{
    public T[] dataArray = Array.Empty<T>();

    public T? Select(int id)
    {
        foreach (var item in dataArray)
        {
            if (item != null && item.ID == id)
            {
                return item;
            }
        }

        return null;
    }
}

public static class LDB
{
    public static ProtoSet<ItemProto> items { get; } = new();

    public static ProtoSet<RecipeProto> recipes { get; } = new();

    public static ProtoSet<ModelProto> models { get; } = new();

    public static ProtoSet<TechProto> techs { get; } = new();

    public static ProtoSet<TutorialProto> tutorial { get; } = new();

    public static ProtoSet<SignalProto> signals { get; } = new();
}

public static class Localization
{
    public const int LCID_ENUS = 1033;
    public const int LCID_ZHCN = 2052;
    public const int LCID_FRFR = 1036;
    public const int LCID_DEDE = 1031;
    public const int LCID_ESES = 3082;
    public const int LCID_JAJA = 1041;
    public const int LCID_KOKO = 1042;

    public static Language? CurrentLanguage { get; set; }

    public static int CurrentLanguageLCID { get; set; }

    public sealed class Language
    {
        public int lcId;
    }
}

public static class DspCore
{
    public static BuildBarRegistry BuildBar { get; } = new();

    public static OptionRegistry Options { get; } = new();

    public static ResourceRegistry Resources { get; } = new();

    public static IconSetRegistry Icons { get; } = new();

    public static ProtoRegistryFacade ProtoRegistration { get; } = new();

    public static RecipeTypeRegistry GameEnums { get; } = new();

    public static BuildingParameterRegistry Blueprints { get; } = new();

    public static ComponentRegistry Components { get; } = new();

    public static ModelRegistry Models { get; } = new();

    public static PlanetSystemRegistry PlanetSystems { get; } = new();

    public static StarSystemRegistry StarSystems { get; } = new();

    public static GalaxySystemRegistry GalaxySystems { get; } = new();

    public static SaveRegistry Saves { get; } = new();

    public static SaveRegistry GlobalSaves { get; } = new();

    public static ErrorReporter Errors { get; } = new();

    internal static TestLogger? Logger { get; } = new();

    public static void ResetForTests()
    {
        Reset(BuildBar, new BuildBarRegistry());
        Reset(Options, new OptionRegistry());
        Reset(Resources, new ResourceRegistry());
        Reset(Icons, new IconSetRegistry());
        Reset(ProtoRegistration, new ProtoRegistryFacade());
        Reset(GameEnums, new RecipeTypeRegistry());
        Reset(Blueprints, new BuildingParameterRegistry());
        Reset(Components, new ComponentRegistry());
        Reset(Models, new ModelRegistry());
        Reset(PlanetSystems, new PlanetSystemRegistry());
        Reset(StarSystems, new StarSystemRegistry());
        Reset(GalaxySystems, new GalaxySystemRegistry());
        Reset(Saves, new SaveRegistry());
        Reset(GlobalSaves, new SaveRegistry());
        LDB.items.dataArray = Array.Empty<ItemProto>();
        LDB.recipes.dataArray = Array.Empty<RecipeProto>();
        LDB.models.dataArray = Array.Empty<ModelProto>();
        LDB.techs.dataArray = Array.Empty<TechProto>();
        LDB.tutorial.dataArray = Array.Empty<TutorialProto>();
        LDB.signals.dataArray = Array.Empty<SignalProto>();
        ItemProto.InitPowerFacilityIndicesCalls = 0;
        ItemProto.InitProductionMaskCalls = 0;
        ItemProto.ThrowOnInitFighterIndices = false;
        ItemProto.ThrowOnNullPrefabDescInCacheSteps = false;
        RecipeProto.recipeExecuteData = new Dictionary<int, RecipeExecuteData>();
        RecipeProto.InitRecipeItemsCalls = 0;
        RecipeProto.InitFractionatorNeedsCalls = 0;
        ModelProto.InitMaxModelIndexCalls = 0;
        ModelProto.InitModelIndicesCalls = 0;
        ModelProto.InitModelOrdersCalls = 0;
        SignalProtoSet.InitSignalKeyIdPairsCalls = 0;
        PlanetFactory.PrefabDescByModelIndex = null;
        PlanetFactory.InitPrefabDescArrayCalls = 0;
        GameMain.data = null;
        GameMain.localPlanet = null;
        GameMain.iconSet = null;
        Localization.CurrentLanguage = null;
        Localization.CurrentLanguageLCID = 0;
        OptionRuntime.ResetForTests();
        Errors.Clear();
        BepInEx.Paths.ConfigPath = Path.Combine(Path.GetTempPath(), "dspcore-logic-tests", Guid.NewGuid().ToString("N"));
    }

    private static void Reset<T>(T target, T source)
        where T : class
    {
        var fields = typeof(T).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            var value = field.GetValue(source);
            field.SetValue(target, value);
        }
    }
}

public sealed class TestLogger
{
    public void LogError(string message)
    {
    }

    public void LogWarning(string message)
    {
    }

    public void LogInfo(string message)
    {
    }
}

public sealed class GameData
{
    public PlanetFactory[]? factories;
    public int factoryCount;
    public GalaxyData? galaxy;

    public PlanetFactory GetOrCreateFactory()
    {
        return factories?[0] ?? new PlanetFactory();
    }

    public void SetForNewGame()
    {
    }
}

public static class GameMain
{
    public static GameData? data;
    public static PlanetData? localPlanet;
    public static GameIconSet? iconSet;
}

public sealed class GameIconSet
{
    public bool loaded = true;
    public int CreateCalls;

    public void Create()
    {
        CreateCalls++;
    }
}

public sealed class VFPreload
{
    public void InvokeOnLoadWorkEnded()
    {
    }
}

public sealed class PowerSystem
{
    public PlanetFactory factory = new();

    public void GameTick(long time, bool isActive)
    {
    }
}

public sealed class FactorySystem
{
    public PlanetFactory factory = new();
    public AssemblerComponent[] assemblerPool = Array.Empty<AssemblerComponent>();

    public void GameTick(long time, bool isActive)
    {
    }

    public void GameTickLabOutputToNext(long time)
    {
    }
}

public sealed class FactoryModel
{
    public PlanetData? planet;
}

public sealed class SpaceSector
{
    public GalaxyData? galaxy;

    public void GameTick(long time)
    {
    }
}

public struct AssemblerComponent
{
    public int id;
    public int entityId;

    public void SetRecipe(int recpId)
    {
    }
}

public sealed class UIAssemblerWindow
{
    public int assemblerId;
    public PlanetFactory? factory;
    public FactorySystem? factorySystem;

    public void OnSelectRecipeClick()
    {
    }

    public void OnRecipePickerReturn()
    {
    }
}

public sealed class ErrorReporter
{
    private readonly List<Exception> reports = new();

    public IReadOnlyList<Exception> Reports => reports;

    public void ReportException(string ownerModGuid, Exception exception)
    {
        reports.Add(exception);
    }

    public void ReportException(string ownerModGuid, Exception exception, ErrorDiagnosticContext? context)
    {
        reports.Add(exception);
    }

    public void Clear()
    {
        reports.Clear();
    }
}

internal static class OptionRuntime
{
    private static readonly Dictionary<string, string> Values = new(StringComparer.Ordinal);
    private static bool bound;

    public static void BindForTests()
    {
        bound = true;
        Values.Clear();
        foreach (var descriptor in DspCore.Options.GetAll())
        {
            Values[OptionRegistry.KeyOf(descriptor.Section, descriptor.Key)] = descriptor.DefaultValue;
        }
    }

    public static void ResetForTests()
    {
        bound = false;
        Values.Clear();
    }

    public static void BindIfReady(OptionDescriptor descriptor)
    {
        if (!bound)
        {
            return;
        }

        Values[OptionRegistry.KeyOf(descriptor.Section, descriptor.Key)] = descriptor.DefaultValue;
    }

    public static bool TryGetString(string section, string key, out string value)
    {
        return Values.TryGetValue(OptionRegistry.KeyOf(section, key), out value!);
    }

    public static bool SetString(string section, string key, string value)
    {
        var optionKey = OptionRegistry.KeyOf(section, key);
        if (!Values.ContainsKey(optionKey))
        {
            return false;
        }

        Values[optionKey] = value;
        return true;
    }

    public static void OpenWindow()
    {
    }

    public static void OpenGlobalSavesWindow()
    {
    }
}

internal static class BuildBarRuntime
{
    public static int ApplyCalls;

    public static void Apply()
    {
        ApplyCalls++;
    }

    public static void OpenOverrideWindow()
    {
    }
}
