using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_PolicyPopup : UC_BaseComponent
{
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private Image _contentImg;
    [SerializeField]
    private Button _confirmBtn;

    [SerializeField]
    private PolicySpriteDicBase _policySpriteDic;
    [SerializeField]
    private TitleTextDicBase _titleTextDic;

    public Action OnConfirmAction;

    public override void InitComponent()
    {
        _confirmBtn?.onClick.AddListener(() => OnConfirmAction?.Invoke());
    }

    private void SetTitle(string title)
    {
        _titleText.text = title;
    }

    private void SetContent(Sprite sprite)
    {
        _contentImg.sprite = sprite;
        _contentImg.SetNativeSize();

        foreach (var elem in _scrollRect.GetComponentsInChildren<RectTransform>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(elem);
        }
        _scrollRect.content.anchoredPosition = Vector2.zero;
    }

    public void SetContent(POLICY_TYPE type)
    {
        SetTitle(_titleTextDic[type]);
        SetContent(_policySpriteDic[type]);
    }

    [Serializable]
    private class PolicySpriteDicBase : SerializableDictionaryBase<POLICY_TYPE, Sprite> { }
    [Serializable]
    private class TitleTextDicBase : SerializableDictionaryBase<POLICY_TYPE, string> { }
}
