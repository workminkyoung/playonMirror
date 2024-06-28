using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UC_SelectableFilter : UC_SelectableContent
{
    [SerializeField]
    private Texture2D _lutTex;
    public Texture2D lutTex { get { return _lutTex; } }
}
