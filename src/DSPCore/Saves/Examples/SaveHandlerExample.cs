using System.IO;
using DSPCore;

namespace ExampleMod;

public sealed class SaveHandlerExample : ICoreSaveHandler
{
    public static void Register()
    {
        Saves.Register("com.example.my-mod", new SaveHandlerExample());
    }

    public void Export(BinaryWriter writer)
    {
        writer.Write(1);
    }

    public void Import(BinaryReader reader)
    {
        int version = reader.ReadInt32();
    }

    public void IntoOtherSave()
    {
    }
}
