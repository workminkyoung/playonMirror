using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UC_GuideGrid : MonoBehaviour
{
    private RectTransform _gridRect;
    public RectTransform gridRect {  get { return _gridRect; } }

    private Vector2 _landscapeSize = new Vector2(1200, 755);
    private Vector2 _portraitSize = new Vector2(688, 880);

    [SerializeField]
    private List<Image> _guides = new List<Image>();
    [SerializeField]
    private List<Image> _dimsPortrait = new List<Image>();
    [SerializeField]
    private List<Image> _dimsLandscape = new List<Image>();

    public void Setting()
    {
        _gridRect = GetComponent<RectTransform>();
    }

    public void SetSize(bool isLandscape)
    {
        if (isLandscape)
        {
            _gridRect.sizeDelta = _landscapeSize;
        }
        else
        {
            _gridRect.sizeDelta = _portraitSize;
        }
        SetActivateDim(isLandscape);
    }

    //public void SetActivate(bool activate)
    //{
    //    for (int i = 0; i < _guides.Count; i++)
    //    {
    //        _guides[i].enabled = activate;
    //    }
    //}
    void SetActivateDim(bool isLandscape)
    {
        if (isLandscape)
        {
            for (int i = 0; i < _dimsLandscape.Count; i++)
            {
                _dimsLandscape[i].enabled = true;
                _dimsPortrait[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < _dimsPortrait.Count; i++)
            {
                _dimsPortrait[i].enabled = true;
                _dimsLandscape[i].enabled = false;
            }
        }
    }
}
