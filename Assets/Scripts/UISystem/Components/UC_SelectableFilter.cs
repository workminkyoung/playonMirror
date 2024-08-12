using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UC_SelectableFilter : UC_SelectableContent
{
    [SerializeField]
    private Texture2D _lutTex;
    private string _filterKey;

    public Texture2D lutTex { get { return _lutTex; } }
    public string FilterKey => _filterKey;

    public void SetlutTex(Texture2D lut)
    {
        _lutTex = lut;
    }

    public void SetFilterKey(string key)
    {
        _filterKey = key;
    }
}
