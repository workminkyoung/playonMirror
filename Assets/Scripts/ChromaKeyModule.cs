using Nexweron.FragFilter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChromaKeyModule : SingletonBehaviour<ChromaKeyModule>
{
    [SerializeField]
    private Material m_combineMaterial;

    [Header("RT")]
    [SerializeField]
    private RenderTexture m_chromaKeyRT;
    [SerializeField]
    private RenderTexture m_resultRT;

    [Header("UI")]
    [SerializeField]
    private Camera m_chromaKeyCamera;
    [SerializeField]
    private RawImage m_bg;
    [SerializeField]
    private RawImage m_camImg;
    [SerializeField]
    private FFTargetRender m_chromaKeyTargetRender;
    [SerializeField]
    private FFController m_chromaKeyController;
    [SerializeField]
    private Texture m_cameraTex;

    [Header("Options")]
    [SerializeField]
    private ChromaKeyOptions[] m_options;

    [Header("Combine")]
    [SerializeField]
    private FFTargetRender m_combine;

    [Header("Debug")]
    [SerializeField]
    private Texture m_bgTex;
    [SerializeField]
    private Texture m_combinedTex;

    public ChromaKeyOptions[] options => m_options;
    public RenderTexture resultRT => m_resultRT;
    public Texture bgTex => m_bg.texture;


    protected override void Init ()
    {
        CreateRT();
        CreateCombineMaterial();
    }

    private void CreateRT ()
    {
        m_resultRT = new RenderTexture(1920, 1080, 24);
        m_resultRT.name = "ResultRenderTexture";
        m_chromaKeyCamera.targetTexture = m_resultRT;
    }

    private void CreateCombineMaterial ()
    {
        m_combineMaterial = new Material(Shader.Find("Custom/CombineImagesShader"));
    }

    public void SetCamImg (Texture tex)
    {
        if(m_chromaKeyRT == null || m_chromaKeyRT.width != tex.width || m_chromaKeyRT.height != tex.height)
        {
            if(m_chromaKeyRT != null)
            {
                Destroy(m_chromaKeyRT);
            }

            m_chromaKeyRT = new RenderTexture(tex.width, tex.height, 24);
            m_chromaKeyRT.name = "ChromaKeyRenderTexture";
            m_chromaKeyTargetRender.targetRenderTexture = m_chromaKeyRT;
        }

        m_cameraTex = tex;
        m_chromaKeyTargetRender.sourceTexture = m_cameraTex;

        m_camImg.texture = m_chromaKeyRT;
    }

    public void SetBg (Texture tex)
    {
        m_bg.texture = tex;
        m_bgTex = tex;
    }

    public Texture2D CombineImage (Texture background, Texture origin)
    {
        m_combine.sourceTexture = origin;
        RenderTexture result = new RenderTexture(origin.width, origin.height, 24);
        m_combine.targetRenderTexture = result;
        m_combine.Render();

        m_combineMaterial.SetTexture("_BackgroundTex", background);
        m_combineMaterial.SetTexture("_PersonTex", result);

        RenderTexture rt = new RenderTexture(origin.width, origin.height, 24);
        RenderTexture.active = rt;

        Graphics.Blit(null, rt, m_combineMaterial);

        Texture2D combinedTex = new Texture2D(origin.width, origin.height, TextureFormat.ARGB32, false);
        combinedTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        combinedTex.Apply();

        RenderTexture.active = null;
        rt.Release();
        m_combinedTex = combinedTex;

        return combinedTex;
    }

    private void Update ()
    {
        if(m_cameraTex != null)
        {
            m_chromaKeyTargetRender.sourceTexture = null;
            m_chromaKeyTargetRender.sourceTexture = m_cameraTex;
        }
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
