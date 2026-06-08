#nullable disable
using System;
using UnityEngine;
using UnityEngine.UI;
using static DSPCore.UiRectTransformUtils;

namespace DSPCore;

/// <summary>
/// 轻量文本按钮控件。
/// </summary>
public class UiFlatButton : MonoBehaviour {
    public RectTransform rectTrans;
    public UIButton uiButton;
    public Text labelText;

    private static GameObject _baseObject;

    public static void InitBaseObject() {
        if (_baseObject) return;
        var panel = UIRoot.instance.uiGame.dysonEditor.controlPanel.hierarchy.layerPanel;
        var go = Instantiate(panel.layerButtons[0].gameObject);
        var btn = go.GetComponent<UIButton>();
        btn.gameObject.name = "dspcore-flat-button";
        btn.highlighted = false;
        var img = btn.GetComponent<Image>();
        if (img != null) {
            img.sprite = panel.buttonDefaultSprite;
            img.color = new(img.color.r, img.color.g, img.color.b, 13f / 255f);
        }

        img = btn.gameObject.transform.Find("frame")?.GetComponent<Image>();
        if (img != null) {
            img.color = new(img.color.r, img.color.g, img.color.b, 0f);
        }

        btn.button.onClick.RemoveAllListeners();
        _baseObject = go;
    }

    public static UiFlatButton CreateFlatButton(float x, float y, RectTransform parent, string label = "",
        int fontSize = 15, Action<int> onClick = null) {
        return CreateFlatButton(x, y, parent, fontSize, onClick).WithLabelText(label);
    }

    public static UiFlatButton CreateFlatButton(float x, float y, RectTransform parent, int fontSize = 15,
        Action<int> onClick = null) {
        var go = Instantiate(_baseObject);
        go.name = "dspcore-flat-button";
        go.SetActive(true);
        var cb = go.AddComponent<UiFlatButton>();
        var rect = NormalizeRectWithMidLeft(cb, x, y, parent);

        cb.rectTrans = rect;
        cb.uiButton = go.GetComponent<UIButton>();

        cb.labelText = go.transform.Find("Text")?.GetComponent<Text>();
        if (cb.labelText != null) {
            cb.labelText.fontSize = Math.Max(15, fontSize);
        }
        cb.uiButton.onClick += onClick;
        return cb;
    }

    public void SetLabelText(string val) {
        if (labelText != null) {
            labelText.text = val.Translate();
        }
    }

    public UiFlatButton WithLabelText(string val) {
        SetLabelText(val);
        return this;
    }

    public UiFlatButton WithSize(float width, float height) {
        rectTrans.sizeDelta = new(width, height);
        return this;
    }

    public UiFlatButton WithTip(string tip, float delay = 1f) {
        uiButton.tips.type = UIButton.ItemTipType.Other;
        uiButton.tips.topLevel = true;
        uiButton.tips.tipTitle = tip;
        uiButton.tips.tipText = null;
        uiButton.tips.delay = delay;
        uiButton.tips.corner = 2;
        uiButton.UpdateTip();
        return this;
    }

    public float Width => rectTrans.sizeDelta.x + labelText.rectTransform.sizeDelta.x;
    public float Height => Math.Max(rectTrans.sizeDelta.y, labelText.rectTransform.sizeDelta.y);
}
