using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UC_HighlightText : MonoBehaviour
{
    Image _image;
    TextMeshProUGUI _textMeshProUGUI;

    public TMP_FontAsset fontOn, fontOff;

    public void Setting()
    {
        _image = GetComponentInChildren<Image>();
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            _textMeshProUGUI.font = fontOn;
        }
        else
        {
            _textMeshProUGUI.font = fontOff;
        }

        _image.enabled = active;
    }
}
