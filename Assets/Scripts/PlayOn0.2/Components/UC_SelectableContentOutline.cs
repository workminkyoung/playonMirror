using MPUIKIT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vivestudios.UI;
using System;
using UnityEngine.UI;

public class UC_SelectableContentOutline : UC_BaseComponent, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField]
    private MPImage _touchFeedback = null;
    [SerializeField]
    private MPImage _stroke = null;
    [SerializeField]
    private bool isViewOnly = false;

    private const int DISABLE_STROKE_SIZE = 1;
    private const int ENABLE_STROKE_SIZE = 4;
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
        //Select(false);
    }

    public virtual void Select(bool isSelected)
    {
        if (isSelected)
        {
            _touchFeedback?.gameObject.SetActive(true);

            if (_stroke)
            {
                _stroke.OutlineColor = ENABLE_STROKE_COLOR;
                _stroke.OutlineWidth = 4;
            }
        }
        else
        {
            _touchFeedback?.gameObject.SetActive(false);

            if (_stroke)
            {
                _stroke.OutlineColor = DISABLE_STROKE_COLOR;
                _stroke.OutlineWidth = 1;
            }
        }
    }
}
