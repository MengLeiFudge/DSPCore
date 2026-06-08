#nullable disable
﻿using System;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using static DSPCore.UiRectTransformUtils;

namespace DSPCore;

/// <summary>
/// 可复用文本下拉框控件。
/// </summary>
public class UiComboBox : MonoBehaviour {
    private RectTransform _rectTrans;
    private UIComboBox _comboBox;
    public Action<int> OnSelChanged;

    private static GameObject _baseObject;

    public static void InitBaseObject() {
        if (_baseObject) return;
        var fontSource = UIRoot.instance.uiGame.buildMenu.uxFacilityCheck.transform.Find("text")?.GetComponent<Text>();
        var go = Instantiate(UIRoot.instance.optionWindow.resolutionComp.gameObject);
        go.name = "dspcore-combo-box";
        go.SetActive(false);

        var rect = (RectTransform)go.transform;
        var cbctrl = rect.GetComponent<UIComboBox>();
        cbctrl.Items.Clear();
        cbctrl.UpdateItems();
        if (fontSource) {
            var txtComp = cbctrl.m_ListItemRes.GetComponentInChildren<Text>();
            if (txtComp) {
                txtComp.font = fontSource.font;
                txtComp.fontSize = fontSource.fontSize;
                txtComp.fontStyle = fontSource.fontStyle;
            }
            txtComp = rect.Find("Main Button/Text")?.GetComponent<Text>();
            if (txtComp) {
                txtComp.font = fontSource.font;
                txtComp.fontSize = fontSource.fontSize;
                txtComp.fontStyle = fontSource.fontStyle;
            }
        }
        cbctrl.onSubmit.RemoveAllListeners();
        cbctrl.onItemIndexChange.RemoveAllListeners();
        _baseObject = go;
    }

    public static UiComboBox CreateComboBox(float x, float y, RectTransform parent) {
        var gameObject = Instantiate(_baseObject);
        gameObject.name = "dspcore-combo-box";
        gameObject.SetActive(true);
        var cb = gameObject.AddComponent<UiComboBox>();
        var rtrans = NormalizeRectWithMidLeft(cb, x, y, parent);
        cb._rectTrans = rtrans;
        var box = rtrans.GetComponent<UIComboBox>();
        cb._comboBox = box;
        box.onItemIndexChange.AddListener(() => { cb.OnSelChanged?.Invoke(box.itemIndex); });
        cb.UpdateComboBoxPosition();

        return cb;
    }

    protected void OnDestroy() {
        if (_config != null) {
            _config.SettingChanged -= _configChanged;
        }
    }

    private void UpdateComboBoxPosition() {
        var rtrans = (RectTransform)_comboBox.transform;
        _rectTrans.sizeDelta = new(rtrans.sizeDelta.x, _rectTrans.sizeDelta.y);
    }

    public void SetFontSize(int size) {
        foreach (var text in _comboBox.m_DropDownContent.GetComponentsInChildren<Text>(true)) {
            text.fontSize = size;
        }

        _comboBox.m_ListItemRes.GetComponentInChildren<Text>().fontSize = size;
        var txtComp = _comboBox.transform.Find("Main Button")?.GetComponentInChildren<Text>();
        if (txtComp) txtComp.fontSize = size;
        UpdateComboBoxPosition();
    }

    public void SetItems(params string[] items) {
        _comboBox.Items = [.. items.Select(s => s.Translate())];
        _comboBox.StartItemIndex = 0;
        _comboBox.DropDownCount = Math.Min(items.Length, 8);
    }

    public void SetIndex(int index) => _comboBox.itemIndex = index;

    public void SetSize(float width, float height) {
        var rtrans = (RectTransform)_comboBox.transform;
        rtrans.sizeDelta =
            new(width > 0f ? width : rtrans.sizeDelta.x, height > 0f ? height : rtrans.sizeDelta.y);
        _rectTrans.sizeDelta = new(rtrans.sizeDelta.x, _rectTrans.sizeDelta.y);
    }

    public void AddOnSelChanged(Action<int> action) => OnSelChanged += action;

    private EventHandler _configChanged;
    private Action<int> _selChanged;
    private ConfigEntry<int> _config;

    public void SetConfigEntry(ConfigEntry<int> config) {
        if (_selChanged != null) OnSelChanged -= _selChanged;
        if (_configChanged != null) config.SettingChanged -= _configChanged;

        _comboBox.itemIndex = config.Value;
        _config = config;
        _selChanged = value => config.Value = value;
        OnSelChanged += _selChanged;
        _configChanged = (_, _) => SetIndex(config.Value);
        config.SettingChanged += _configChanged;
    }

    public UiComboBox WithFontSize(int size) {
        SetFontSize(size);
        return this;
    }

    public UiComboBox WithItems(params string[] items) {
        SetItems(items);
        return this;
    }

    public UiComboBox WithIndex(int index) {
        SetIndex(index);
        return this;
    }

    public UiComboBox WithSize(float width, float height) {
        SetSize(width, height);
        return this;
    }

    public UiComboBox WithOnSelChanged(params Action<int>[] action) {
        foreach (var act in action)
            AddOnSelChanged(act);
        return this;
    }

    public UiComboBox WithConfigEntry(ConfigEntry<int> config) {
        SetConfigEntry(config);
        return this;
    }

    public float Width => _rectTrans.sizeDelta.x;
    public float Height => _rectTrans.sizeDelta.y;
}
