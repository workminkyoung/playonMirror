using ChromakeyFrameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UC_FrameMask : MonoBehaviour
{
    private List<RawImage> _rawImages = new List<RawImage>();

    public void Init()
    {
        _rawImages.AddRange(GetComponentsInChildren<RawImage>());
    }

    public void SetTextures(ImageOrderedDic texs)
    {
        for (int i = 0; i < _rawImages.Count; i++)
        {
            _rawImages[i].texture = texs[i];
        }
    }
}
