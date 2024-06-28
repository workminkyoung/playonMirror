using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;
using UnityEngine.EventSystems;

public class UC_ToggleSelect : UC_SelectableContentOutline
{
    private Toggle _toggle;
    [SerializeField]
    private bool _isSelect = false;

    public Action<bool> OnToggleValueChanged;

    public Toggle toggle
    {
        get { return _toggle; }
    }

    public override void InitComponent()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener((state) =>
        {
            Select(state);
            OnToggleValueChanged?.Invoke(toggle.isOn);
        });
        ToggleGroup group = GetComponentInParent<ToggleGroup>();
        if (group != null)
            _toggle.group = group;

    }
    public override void OnPointerDown(PointerEventData eventData)
    {

    }
    public override void OnPointerClick(PointerEventData eventData)
    {

    }
}
