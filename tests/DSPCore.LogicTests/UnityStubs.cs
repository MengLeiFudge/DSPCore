namespace UnityEngine;

public sealed class Sprite
{
    public Texture2D Texture { get; }

    private Sprite(Texture2D texture)
    {
        Texture = texture;
    }

    public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit)
    {
        return new Sprite(texture);
    }
}

public sealed class Texture2D
{
    public Texture2D(int width, int height, TextureFormat format, bool mipChain)
    {
        this.width = width;
        this.height = height;
    }

    public int width { get; set; }

    public int height { get; set; }
}

public enum TextureFormat
{
    RGBA32
}

public readonly struct Rect
{
    public Rect(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public float X { get; }

    public float Y { get; }

    public float Width { get; }

    public float Height { get; }
}

public readonly struct Vector2
{
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; }

    public float Y { get; }
}

public static class ImageConversion
{
    public static bool LoadImage(Texture2D texture, byte[] data)
    {
        if (data.Length == 0)
        {
            return false;
        }

        texture.width = 16;
        texture.height = 16;
        return true;
    }
}

public static class Resources
{
    public static T? Load<T>(string path)
        where T : class
    {
        return null;
    }
}

public class Object
{
    public static void Destroy(object target)
    {
    }
}

public sealed class AssetBundle
{
    public string Path { get; private set; } = string.Empty;

    public static AssetBundle? LoadFromFile(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return null;
        }

        return new AssetBundle { Path = path };
    }

    public T? LoadAsset<T>(string name)
        where T : class
    {
        if (typeof(T) == typeof(Sprite))
        {
            return Sprite.Create(new Texture2D(8, 8, TextureFormat.RGBA32, false), new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f), 100f) as T;
        }

        if (typeof(T) == typeof(Texture2D))
        {
            return new Texture2D(8, 8, TextureFormat.RGBA32, false) as T;
        }

        return null;
    }
}
