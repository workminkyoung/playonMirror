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
    private Image _touchFeedback = null;
    [SerializeField]
    private Image _stroke = null;
    [SerializeField]
    private bool isViewOnly = false;
    [SerializeField]
    protected Image _thumbnailImg = null;

    public Image thumbnailImg => _thumbnailImg;

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

            if (_stroke)
            {
                (_stroke as MPImage).color = ENABLE_STROKE_COLOR;
                (_stroke as MPImage).StrokeWidth = ENABLE_STROKE_SIZE;
            }
        }
        else
        {
            _touchFeedback?.gameObject.SetActive(false);

            if (_stroke)
            {
                (_stroke as MPImage).color = DISABLE_STROKE_COLOR;
                (_stroke as MPImage).StrokeWidth = DISABLE_STROKE_SIZE;
            }
        }
    }

    public virtual void SetThumbnail(Sprite thumbnail)
    {
        _thumbnailImg.sprite = thumbnail;
    }

    public void SetThumbnailClear(Color color)
    {
        _thumbnailImg.color = color;
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
