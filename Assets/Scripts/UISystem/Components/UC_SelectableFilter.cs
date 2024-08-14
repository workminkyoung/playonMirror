using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UC_SelectableFilter : UC_SelectableContent
{
    [SerializeField]
    private Texture2D _lutTex;

    public Texture2D lutTex { get { return _lutTex; } }

    public void SetlutTex(Texture2D lut)
    {
        byte[] bytes = lut.EncodeToPNG();
        Texture2D linearLut = new Texture2D(lut.width, lut.height, TextureFormat.ARGB32, false, false);
        linearLut.LoadImage(bytes);
        _lutTex = linearLut;
    }

}
