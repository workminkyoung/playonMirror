using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_CategoryToggle : UC_BaseComponent, IPointerClickHandler
{
    [SerializeField]
    private HorizontalLayoutGroup _layoutGroup;
    [SerializeField]
    private Image _bgImg;
    [SerializeField]
    private Image _strokeImg;
    [SerializeField]
    private Image _checkImg;
    [SerializeField]
    private TextMeshProUGUI _text;

    [Header("Setting")]
    [SerializeField]
    private Color _originalBgColor;
    [SerializeField]
    private Color _selectedBgColor;
    [SerializeField]
    private Color _originalStrokeColor, _selectedStrokeColor;
    [SerializeField]
    private Color _originalTextColor, _selectedTextColor;
    [SerializeField]
    private int _originalPaddingL, _selectedPaddingL;
    [SerializeField]
    private int _originalPaddingR, _selectedPaddingR;

    public Action pointerClickAction;
    public string category => _text.text;

    public override void InitComponent ()
    {
        Select(false);
    }
    public void OnPointerClick (PointerEventData eventData)
    {
        pointerClickAction?.Invoke();
        Select(true);
    }

    public void Select (bool isSelected)
    {
        _bgImg.color = isSelected ? _selectedBgColor : _originalBgColor;
        _strokeImg.color = isSelected ? _selectedStrokeColor : _originalStrokeColor;
        _text.color = isSelected ? _selectedTextColor : _originalTextColor;
        _checkImg.gameObject.SetActive(isSelected);
        _layoutGroup.padding.left = isSelected ? _selectedPaddingL : _originalPaddingL;
        _layoutGroup.padding.right = isSelected ? _selectedPaddingR : _originalPaddingR;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    private void OnDisable ()
    {
        Select(false);
    }
}
