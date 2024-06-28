using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_Toast : UC_BaseComponent
{
    [SerializeField]
    private TextMeshProUGUI _text;

    public bool isOn { get { return canvasGroup.alpha > 0; } }

    public override void InitComponent()
    {
        canvasGroup.alpha = 0f;
    }

    public void OpenToast(bool isEnable)
    {
        //canvasGroup.DOFade(isEnable ? 1 : 0, 0.5f);
        canvasGroup.alpha = isEnable ? 1 : 0;
    }

    public void SetText(string text)
    {
        _text.text = text;

        foreach(var elem in GetComponentsInChildren<RectTransform>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(elem);
        } 
    }
}
