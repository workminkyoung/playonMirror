using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Vivestudios.UI;

public class UC_SelectFrame : UC_BaseComponent
{
    [SerializeField]
    private List<GameObject> _container = new List<GameObject>();
    private List<UC_Color> _colors = new List<UC_Color>();
    private List<UC_Filter> _frames = new List<UC_Filter>();

    public Action<FRAME_TYPE> SetFrame;
    public Action<Color> SetColor;

    bool _isFrameChangable = false;
    FRAME_TYPE _type;

    public FRAME_TYPE type { set { _type = value; } }

    public override void InitComponent()
    {
        //throw new System.NotImplementedException();

        _colors.AddRange(GetComponentsInChildren<UC_Color>());
        _frames.AddRange(GetComponentsInChildren<UC_Filter>());

        for (int i = 0; i < _colors.Count; i++)
        {
            _colors[i].InitComponent();
            _colors[i].SendColor = SelectColor;
        }
        for (int i = 0; i < _frames.Count; i++)
        {
            int index = i;
            _frames[i].InitComponent();
            _frames[i].toggle.onValueChanged.AddListener((state) =>
            {
                if (state)
                    SelectFrame(index);
            });
        }

        SetActivate(false);
    }

    public override void SetActivate(bool state)
    {
        base.SetActivate(state);
        _isFrameChangable = false;

        if (state)
        {
            _container[(int)CONTAINER.FRAME].SetActive(false);
            if (_type == FRAME_TYPE.FRAME_2)
            {
                _isFrameChangable = true;
                _container[(int)CONTAINER.FRAME].SetActive(true);
            }
        }
    }

    void SelectColor(Color color)
    {
        SetColor(color);
    }

    void SelectFrame(int index)
    {
        if (!_isFrameChangable)
            return;

        FRAME_TYPE type = FRAME_TYPE.FRAME_2;

        switch (index)
        {
            case 0:
                type = FRAME_TYPE.FRAME_2;
                break;
            case 1:
                type = FRAME_TYPE.FRAME_2_1;
                break;
            case 2:
                type = FRAME_TYPE.FRAME_2_2;
                break;
            default:
                break;
        }

        SetFrame(type);
        CustomLogger.Log("Change Frame");
    }

    enum CONTAINER
    {
        COLOR,
        FRAME
    }
}
