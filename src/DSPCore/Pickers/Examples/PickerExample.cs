using DSPCore;
using static DSPCore.DspCore;

namespace ExampleMod;

public static class PickerExample
{
    public static void OpenItemPicker()
    {
        Pickers.Register(new PickerRequest(
            Kind: PickerKind.Item,
            OwnerModGuid: "com.example.my-mod",
            Filter: value => value is ItemProto item && item.ID >= 9000,
            ShowLocked: true,
            ShowAll: true,
            OnReturn: OnItemSelected));
    }

    private static void OnItemSelected(object? value)
    {
        var selectedItem = value as ItemProto;
        int itemId = selectedItem?.ID ?? 0;
    }
}
