using MPUIKIT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UC_Color : UC_SelectableContentOutline
{
    public Action<Color> SendColor;
    [SerializeField]
    private MPImage _checker = null;
    [SerializeField]
    private MPImage _base = null;

    private Toggle _toggle;

    public Toggle toggle
    {
        get { return _toggle; }
    }

    //public textire

    public override void InitComponent()
    {
        _toggle = GetComponent<Toggle>();
        ToggleGroup group = GetComponentInParent<ToggleGroup>();

        _toggle.onValueChanged.AddListener(OnToggle);
        if (group != null)
            _toggle.group = group;

    }

    void OnToggle(bool state)
    {
        Select(state);
        _checker.gameObject.SetActive(state);
        SendColor(_base.color);
    }
}
