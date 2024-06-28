using MPUIKIT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
using Vivestudios.UI;

public class UC_InteractableMPImage : UC_BaseComponent, IPointerDownHandler, IPointerUpHandler
{
    MPImage _mpImage;

    [Header("SIZE")]
    public int DISABLE_OUTLINE_SIZE = 1;
    public int ENABLE_OUTLINE_SIZE = 4;
    public int HIGHLIGHT_OUTLINE_SIZE = 4;

    [Space(10f)]
    [Header("BACKGROUND")]
    public Color DISABLE_COLOR;
    public Color ENABLE_COLOR;
    public Color HIGHLIGHT_COLOR;

    [Space(10f)]
    [Header("OUTLINE")]
    public Color DISABLE_OUTLINE_COLOR;
    public Color ENABLE_OUTLINE_COLOR;
    public Color HIGHLIGHT_OUTLINE_COLOR;
    public Action pointerDownAction;

    public override void InitComponent()
    {
        _mpImage = GetComponent<MPImage>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        pointerDownAction?.Invoke();

        if (_mpImage)
        {
            _mpImage.color = HIGHLIGHT_COLOR;
            _mpImage.OutlineWidth = HIGHLIGHT_OUTLINE_SIZE;
            _mpImage.OutlineColor = HIGHLIGHT_OUTLINE_COLOR;
        }
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (_mpImage)
        {
            _mpImage.color = ENABLE_COLOR;
            _mpImage.OutlineWidth = ENABLE_OUTLINE_SIZE;
            _mpImage.OutlineColor = ENABLE_OUTLINE_COLOR;
        }
    }

    protected virtual void OnDisable()
    {
        if (_mpImage)
        {
            _mpImage.color = DISABLE_OUTLINE_COLOR;
            _mpImage.OutlineWidth = DISABLE_OUTLINE_SIZE;
            _mpImage.OutlineColor = DISABLE_OUTLINE_COLOR;
        }
    }

    protected virtual void OnEnable()
    {
        if (_mpImage)
        {
            _mpImage.color = ENABLE_COLOR;
            _mpImage.OutlineWidth = ENABLE_OUTLINE_SIZE;
            _mpImage.OutlineColor = ENABLE_OUTLINE_COLOR;
        }
    }
}
