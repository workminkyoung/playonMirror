using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleDesign : Toggle
{
    //[SerializeField]
    //Image cover;
    List<Image> images = new List<Image>();

    protected override void Awake()
    {
        base.Awake();
        //cover = transform.GetChild(transform.childCount - 1).GetComponent<Image>();
        images.AddRange(GetComponentsInChildren<Image>());
        onValueChanged.AddListener((state) => CheckState());
    }

    public void CheckState()
    {
        if (isOn)
        {
            images[(int)eImg.Cover].enabled = true;
            images[(int)eImg.Toggle].rectTransform.sizeDelta = Vector2.one * 180;
        }
        else
        {
            images[(int)eImg.Cover].enabled = false;
            images[(int)eImg.Toggle].rectTransform.sizeDelta = Vector2.one * 140;
        }
    }

    enum eImg
    {
        Toggle = 0,
        Cover = 4
    }
}
