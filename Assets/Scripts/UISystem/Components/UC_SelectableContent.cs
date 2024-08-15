using MPUIKIT;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_SelectableContent : UC_BaseComponent, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField]
    private string _key;
    [SerializeField]
    private Image _touchFeedback = null;
    [SerializeField]
    private Image _stroke = null;
    [SerializeField]
    private bool isViewOnly = false;
    [SerializeField]
    protected Image _thumbnailImg = null;
    [SerializeField]
    protected Sprite _thumbnailUnselectImg = null;
    [SerializeField]
    protected Sprite _thumbnailSelectImg = null;
    [SerializeField]
    protected TextMeshProUGUI _nameText = null;

    public Image thumbnailImg => _thumbnailImg;
    public string Key => _key;

    protected const int DISABLE_STROKE_SIZE = 1;
    protected const int ENABLE_STROKE_SIZE = 4;
    private readonly Color DISABLE_STROKE_COLOR = new Color(0.4588235f, 0.4588235f, 0.4588235f);
    private readonly Color ENABLE_STROKE_COLOR = new Color(0.8078431f, 0.2f, 0.5372549f);

    public Action pointerDownAction;
    public Action pointerClickAction;

    public override void InitComponent()
    {
        Select(false);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (isViewOnly)
            return;

        pointerDownAction?.Invoke();

        Select(true);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        pointerClickAction?.Invoke();

    }

    protected virtual void OnDisable()
    {
        Select(false);
    }

    public virtual void Select(bool isSelected)
    {
        if (isSelected)
        {
            _touchFeedback?.gameObject.SetActive(true);

            if (_thumbnailSelectImg != null)
            {
                _thumbnailImg.sprite = _thumbnailSelectImg;
            }
            else if (_stroke)
            {
                (_stroke as MPImage).color = ENABLE_STROKE_COLOR;
                (_stroke as MPImage).StrokeWidth = ENABLE_STROKE_SIZE;
            }
        }
        else
        {
            _touchFeedback?.gameObject.SetActive(false);

            if (_thumbnailUnselectImg != null)
            {
                _thumbnailImg.sprite = _thumbnailUnselectImg;
            }
            else if (_stroke)
            {
                (_stroke as MPImage).color = DISABLE_STROKE_COLOR;
                (_stroke as MPImage).StrokeWidth = DISABLE_STROKE_SIZE;
            }
        }
    }

    public virtual void SetThumbnail(Sprite thumbnail)
    {
        if (_thumbnailImg == null || thumbnail == null)
            return;
        _thumbnailImg.sprite = thumbnail;
    }

    public void SetThumbnail(Sprite select, Sprite unselect)
    {
        if (_thumbnailImg == null || select == null)
            return;

        _thumbnailImg.sprite = select;
        _stroke.enabled = false;

        _thumbnailSelectImg = select;
        _thumbnailUnselectImg = unselect;
    }

    public void SetThumbnailClear(Color color)
    {
        _thumbnailImg.color = color;
    }

    public void SetNameText(string name)
    {
        _nameText.text = name;
    }

    public void SetKey(string key)
    {
        _key = key;
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    _touchFeedback?.gameObject.SetActive(false);

    //    if (_stroke)
    //    {
    //        _stroke.color = DISABLE_STROKE_COLOR;
    //        _stroke.StrokeWidth = 1;
    //    }
    //}
}
