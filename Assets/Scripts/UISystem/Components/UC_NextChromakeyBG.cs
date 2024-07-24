using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_NextChromakeyBG : UC_BaseComponent
{
    [SerializeField]
    private TextMeshProUGUI _titleTxt;
    [SerializeField]
    private RawImage _bgImg;

    public override void InitComponent ()
    {

    }

    public void SetTitleText (string title)
    {
        _titleTxt.text = title;
    }

    public void SetBgImg (Texture img)
    {
        _bgImg.texture = img;
    }
}
