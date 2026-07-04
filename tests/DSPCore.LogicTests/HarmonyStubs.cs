using System;

namespace HarmonyLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class HarmonyPatchAttribute : Attribute
{
    public HarmonyPatchAttribute()
    {
    }

    public HarmonyPatchAttribute(Type type, string methodName)
    {
    }

    public HarmonyPatchAttribute(Type type, string methodName, Type[] argumentTypes)
    {
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class HarmonyPostfixAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class HarmonyPrefixAttribute : Attribute
{
}

public static class AccessTools
{
    public static System.Reflection.FieldInfo? Field(Type type, string name)
    {
        return type.GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
    }

    public static System.Reflection.PropertyInfo? Property(Type type, string name)
    {
        return type.GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
    }
}
