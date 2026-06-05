using System;
using UnityEngine;

namespace DSPCore;

internal static class PickerRuntime
{
    public static void Update()
    {
        foreach (var request in DspCore.Pickers.ConsumeAll())
        {
            Open(request);
        }
    }

    private static void Open(PickerRequest request)
    {
        var position = new Vector2(0f, 0f);
        try
        {
            switch (request.Kind)
            {
                case PickerKind.Item:
                    UIItemPicker.showAll = request.ShowAll || request.ShowLocked;
                    UIItemPicker.Popup(position, item =>
                    {
                        if (item != null && request.Filter != null && !request.Filter(item))
                        {
                            request.OnReturn?.Invoke(null);
                            return;
                        }

                        request.OnReturn?.Invoke(item);
                    });
                    break;
                case PickerKind.Recipe:
                    UIRecipePicker.Popup(position, recipe =>
                    {
                        if (recipe != null && request.Filter != null && !request.Filter(recipe))
                        {
                            request.OnReturn?.Invoke(null);
                            return;
                        }

                        request.OnReturn?.Invoke(recipe);
                    });
                    break;
                case PickerKind.Signal:
                    UISignalPicker.Popup(position, signalId =>
                    {
                        if (signalId != 0 && request.Filter != null && !request.Filter(signalId))
                        {
                            request.OnReturn?.Invoke(null);
                            return;
                        }

                        request.OnReturn?.Invoke(signalId);
                    });
                    break;
            }
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException(request.OwnerModGuid, ex);
            DspCore.Logger?.LogError($"Picker request {request.Kind} from {request.OwnerModGuid} failed: {ex}");
            request.OnReturn?.Invoke(null);
        }
    }
}
