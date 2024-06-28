using MPUIKIT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UC_InteractableMPImageIcon : UC_InteractableMPImage
{
    MPImage _mpIcon;

    [Space(10f)]
    [Header("ICON")]
    public Color DISABLE_ICON_COLOR;
    public Color ENABLE_ICON_COLOR;
    public Color HIGHLIGHT_ICON_COLOR;

    public override void InitComponent()
    {
        base.InitComponent();
        _mpIcon = UtilityExtensions.GetComponentOnlyInChildren_NonRecursive<MPImage>(transform);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (_mpIcon != null)
        {
            _mpIcon.color = HIGHLIGHT_ICON_COLOR;
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (_mpIcon != null)
        {
            _mpIcon.color = HIGHLIGHT_ICON_COLOR;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_mpIcon != null)
        {
            _mpIcon.color = DISABLE_ICON_COLOR;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_mpIcon != null)
        {
            _mpIcon.color = ENABLE_ICON_COLOR;
        }
    }
}
