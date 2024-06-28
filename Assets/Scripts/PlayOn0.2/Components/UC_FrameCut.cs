using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UC_FrameCut : UC_Frame
{
    [SerializeField]
    List<RawImage> _cutRawimage = new List<RawImage>();

    public override void SetTexture(int index, Texture2D texture)
    {
        base.SetTexture(index, texture);
        if (index < _cutRawimage.Count)
        {
            _cutRawimage[index].texture = texture;
        }
    }
    public override void SetTexture(int index, Texture texture)
    {
        base.SetTexture(index, texture);
        if (index < _cutRawimage.Count)
        {
            _cutRawimage[index].texture = texture;
        }
    }

    public override void RemoveTexture(int index)
    {
        base.RemoveTexture(index);
        if (index < _cutRawimage.Count)
        {
            _cutRawimage[index].texture = null;
        }
    }

    public override void SetLut(Material matPrefab, Texture2D lut)
    {
        base.SetLut(matPrefab, lut);
        for (int i = 0; i < _cutRawimage.Count; i++)
        {
            Material mat = Instantiate(matPrefab);
            mat.SetTexture("_MainTex", _cutRawimage[i].texture);
            mat.SetTexture("_LutTex", lut);

            _cutRawimage[i].material = mat;
        }
    }
}
