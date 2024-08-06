using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_ConfirmPopup : UC_BaseComponent
{
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private TextMeshProUGUI _descriptionText;
    [SerializeField]
    private Image _img;
    [SerializeField]
    private Button _confirmBtn = null;

    private delegate void OnConfirmDelegate();
    private OnConfirmDelegate onConfirmDelegate;

    public override void InitComponent()
    {
        canvasGroup.alpha = 0;

        _confirmBtn.onClick.AddListener(() =>
        {
            //OpenPopup(false);
            if(onConfirmDelegate != null)
            {
                onConfirmDelegate();
            }
            OpenPopup(false);
        });
    }

    public void SetImage(Sprite sprite)
    {
        if (!_img)
            return;
        _img.sprite = sprite;
    }

    public void OpenPopup(bool isEnable, Action onConfirm = null)
    {
        canvasGroup.alpha = isEnable ? 1 : 0;
        gameObject.SetActive(isEnable);
        onConfirmDelegate = () => { onConfirm?.Invoke(); };
    }

    public void SetTitle(string text)
    {
        if (!_titleText)
            return;
        _titleText.text = text;
    }

    public void SetDescription(string text)
    {
        if (!_descriptionText)
            return;
        _descriptionText.text = text.Replace("\\n", "\n");
    }
}
