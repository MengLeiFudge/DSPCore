namespace DSPCore.Preloader;

using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Mono.Cecil;

/// <summary>
/// DSPCore 的 BepInEx preloader 入口。
/// BepInEx preloader entry point for DSPCore.
/// </summary>
public static class Preloader
{
    private static ManualLogSource? logger;

    /// <summary>
    /// 获取需要在加载前处理的程序集。
    /// Gets assemblies that should be processed before loading.
    /// </summary>
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    /// <summary>
    /// 初始化 preloader 日志。
    /// Initializes the preloader logger.
    /// </summary>
    public static void Initialize()
    {
        logger = Logger.CreateLogSource("DSPCore Preloader");
    }

    /// <summary>
    /// 在游戏程序集加载前应用 DSPCore patch。
    /// Applies DSPCore patches before the game assembly is loaded.
    /// </summary>
    /// <param name="assembly">目标程序集。Target assembly.</param>
    public static void Patch(AssemblyDefinition assembly)
    {
        var changed = false;
        changed |= AddEntityDataFields(assembly);
        changed |= AddPrefabDescFields(assembly);
        changed |= AddRecipeTypeCustom(assembly);
        logger?.LogInfo(changed
            ? $"DSPCore preloader patched {assembly.Name.Name}."
            : $"DSPCore preloader found no missing fields in {assembly.Name.Name}.");
    }

    private static bool AddEntityDataFields(AssemblyDefinition assembly)
    {
        var entityData = FindType(assembly, "EntityData");
        if (entityData == null)
        {
            return false;
        }

        var changed = false;
        changed |= AddField(entityData, "customId", assembly.MainModule.TypeSystem.Int32);
        changed |= AddField(entityData, "customType", assembly.MainModule.TypeSystem.Int32);
        changed |= AddField(entityData, "customData", DictionaryStringObject(assembly));
        return changed;
    }

    private static bool AddPrefabDescFields(AssemblyDefinition assembly)
    {
        var prefabDesc = FindType(assembly, "PrefabDesc");
        if (prefabDesc == null)
        {
            return false;
        }

        return AddField(prefabDesc, "customData", DictionaryStringObject(assembly));
    }

    private static bool AddRecipeTypeCustom(AssemblyDefinition assembly)
    {
        var recipeType = FindType(assembly, "ERecipeType");
        if (recipeType == null || recipeType.Fields.Any(field => field.Name == "Custom"))
        {
            return false;
        }

        var field = new FieldDefinition("Custom", FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal | FieldAttributes.HasDefault, recipeType)
        {
            Constant = 20
        };
        recipeType.Fields.Add(field);
        return true;
    }

    private static bool AddField(TypeDefinition type, string name, TypeReference fieldType)
    {
        if (type.Fields.Any(field => field.Name == name))
        {
            return false;
        }

        type.Fields.Add(new FieldDefinition(name, FieldAttributes.Public, fieldType));
        return true;
    }

    private static TypeDefinition? FindType(AssemblyDefinition assembly, string name)
    {
        return assembly.MainModule.Types.FirstOrDefault(type => type.Name == name);
    }

    private static TypeReference DictionaryStringObject(AssemblyDefinition assembly)
    {
        var module = assembly.MainModule;
        var dictionary = module.ImportReference(typeof(Dictionary<,>));
        var instance = new GenericInstanceType(dictionary);
        instance.GenericArguments.Add(module.TypeSystem.String);
        instance.GenericArguments.Add(module.TypeSystem.Object);
        return module.ImportReference(instance);
    }
}
