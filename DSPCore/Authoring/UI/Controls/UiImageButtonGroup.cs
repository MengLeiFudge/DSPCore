#nullable disable
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 按钮组，用于管理按钮的选中状态
/// </summary>
public class UiImageButtonGroup(bool singleSelection = true) {
    private readonly List<UiImageButton> _buttons = new();
    // true: 单选组，false: 多选组

    public void AddButton(UiImageButton button) {
        if (!_buttons.Contains(button)) {
            _buttons.Add(button);
        }
    }

    public void RemoveButton(UiImageButton button) {
        _buttons.Remove(button);
    }

    public void OnButtonSelected(UiImageButton selectedButton) {
        if (singleSelection) {
            // 单选组：取消其他按钮的选中状态
            foreach (var button in _buttons) {
                if (button != selectedButton && button.Selected) {
                    button.Selected = false;
                }
            }
        }
    }

    public void ToggleButton(UiImageButton button) {
        if (singleSelection) {
            if (button.Selected) {
                // 单选组中，如果当前按钮已选中，不允许取消选中（保证至少有一个选中）
                return;
            } else {
                button.Selected = true;
            }
        } else {
            // 多选组：直接切换状态
            button.Selected = !button.Selected;
        }
    }

    public List<UiImageButton> GetSelectedButtons() {
        return _buttons.FindAll(b => b.Selected);
    }

    public UiImageButton GetSelectedButton() {
        return _buttons.Find(b => b.Selected);
    }

    public void ClearSelection() {
        foreach (var button in _buttons) {
            button.Selected = false;
        }
    }

    public void SelectButton(UiImageButton button) {
        if (_buttons.Contains(button)) {
            button.Selected = true;
        }
    }
}
