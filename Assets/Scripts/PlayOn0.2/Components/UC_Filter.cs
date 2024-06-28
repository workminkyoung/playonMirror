using MPUIKIT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UC_Filter : UC_SelectableContentOutline
{
    [SerializeField]
    private MPImage _checker = null;
    [SerializeField]
    private Texture2D _lutTexture;

    public Action<Texture2D> SendLut;
    //public Func<>

    public Action<bool> OnToggleValueChanged;

    private Toggle _toggle;
    [SerializeField]
    private bool _isToggleOn = false;

    public Toggle toggle
    {
        get { return _toggle; }
    }

    //public textire

    public override void InitComponent()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(OnToggle);
        ToggleGroup group = GetComponentInParent<ToggleGroup>();
        if (group != null)
            _toggle.group = group;
    }

    public override void SetActivate(bool state)
    {
        _isToggleOn = state;
        //base.SetActivate(state);
    }

    void OnToggle(bool state)
    {
        Select(state);
        SendLut?.Invoke(_lutTexture);
        OnToggleValueChanged?.Invoke(toggle.isOn);

        if (!_isToggleOn)
            return;

        
    }

    public override void Select(bool isSelected)
    {
        base.Select(isSelected);

        _checker.gameObject.SetActive(isSelected);
    }
}
