using Nexweron.FragFilter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChromaKeyModule : SingletonBehaviour<ChromaKeyModule>
{
    [Header("RT")]
    [SerializeField]
    private RenderTexture m_chromaKeyRT;

    [Header("UI")]
    [SerializeField]
    private Camera m_chromaKeyCamera;
    [SerializeField]
    private RawImage m_bg;
    [SerializeField]
    private RawImage m_camImg;
    [SerializeField]
    private FFController m_chromaKeyController;

    [Header("Options")]
    [SerializeField]
    private ChromaKeyOptions[] m_options;

    public ChromaKeyOptions[] options => m_options;


    protected override void Init ()
    {
        CreateRT();

    }

    private void CreateRT ()
    {
        m_chromaKeyRT = new RenderTexture(1920, 1080, 24);
        m_chromaKeyRT.name = "ChromaKeyRenderTexture";
        m_chromaKeyCamera.targetTexture = m_chromaKeyRT;
    }

    public void SetCamImg (Texture tex)
    {
        m_camImg.texture = tex;
    }

    private void Update ()
    {
        
    }
}

[Serializable]
public class ChromaKeyOptions
{
    public string key;
    public string kategory;
    public string name_kor;
    public string name_eng;
    public string name_chn;

    public Sprite thumbnail;
    public Texture2D[] images;
}
