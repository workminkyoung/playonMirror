using Nexweron.FragFilter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Texture m_cameraTex;

    [Header("Options")]
    [SerializeField]
    private List<ChromaKeyOptions> m_options;

    [Header("Combine")]
    [SerializeField]
    private FFTargetRender m_combine;

    [Header("Setting")]
    [SerializeField]
    private FFChromaKeyAlpha[] m_chromaKeyAlphas;
    [SerializeField]
    private FFBlur[] m_blurs;
    [SerializeField]
    private FFMaskSource[] m_maskSources;

    [Header("Debug")]
    [SerializeField]
    private Texture m_bgTex;
    [SerializeField]
    private Texture m_combinedTex;

    public List<ChromaKeyOptions> options => m_options;
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
        if(tex == null)
        {
            m_cameraTex = null;
            return;
        }

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

    public void ChangeChromaKeyConfig (float d, float t, int blur, float alphaPow, float alphaEdge, string color)
    {
        ConfigData.config.chromaKey.d = d;
        ConfigData.config.chromaKey.t = t;
        ConfigData.config.chromaKey.blur = blur;
        ConfigData.config.chromaKey.alphaPow = alphaPow;
        ConfigData.config.chromaKey.alphaEdge = alphaEdge;
        ConfigData.config.chromaKey.color = color;

        ConfigLoadManager.inst.SaveConfig();
        ApplySetting();
    }

    public void ResetChromaKeySetting ()
    {
        ApplySetting();
    }

    public void TemporaryApplySetting (float d, float t, int blur, float alphaPow, float alphaEdge, string color)
    {
        foreach(var elem in m_chromaKeyAlphas)
        {
            elem.keyColor = UtilityExtensions.HexToColor(color);
            elem.dChroma = d;
            elem.dChromaT = t;
        }

        foreach(var elem in m_blurs)
        {
            elem.blurOffset = blur;
        }

        foreach(var elem in m_maskSources)
        {
            elem.alphaPow = alphaPow;
            elem.alphaEdge = alphaEdge;
        }
    }

    private void ApplySetting ()
    {
        foreach(var elem in m_chromaKeyAlphas)
        {
            elem.keyColor = UtilityExtensions.HexToColor(ConfigData.config.chromaKey.color);
            elem.dChroma = ConfigData.config.chromaKey.d;
            elem.dChromaT = ConfigData.config.chromaKey.t;
        }

        foreach(var elem in m_blurs)
        {
            elem.blurOffset = ConfigData.config.chromaKey.blur;
        }

        foreach(var elem in m_maskSources)
        {
            elem.alphaPow = ConfigData.config.chromaKey.alphaPow;
            elem.alphaEdge = ConfigData.config.chromaKey.alphaEdge;
        }
    }

    public void UpdateOption (CONTENT_TYPE type)
    {
        int i = 0;
        switch(type)
        {
            case CONTENT_TYPE.AI_CARTOON:
                foreach (var elem in AdminManager.Instance.ChromakeyFrame.ChromakeyFrameTable)
                {
                    if(m_options.Count > i && m_options[i] != null)
                    {
                        m_options[i].key = elem.Value.Key;
                        m_options[i].category = elem.Value.Category;
                        m_options[i].name_kor = elem.Value.Korean;
                        m_options[i].name_eng = elem.Value.English;
                        m_options[i].name_chn = elem.Value.Chinese;
                        m_options[i].thumbnail = elem.Value.Thumbnail_data;
                        m_options[i].images = elem.Value.Image_data;
                    }
                    else
                    {
                        ChromaKeyOptions option = new ChromaKeyOptions();
                        option.key = elem.Value.Key;

                        m_options.Add(option);
                    }
                    i++;
                }
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                foreach (var elem in AdminManager.Instance.ChromakeyFrame.ChromakeyToneTable)
                {
                    if (m_options.Count > i && m_options[i] != null)
                    {
                        m_options[i].key = elem.Value.Key;
                        m_options[i].category = elem.Value.Category;
                        m_options[i].name_kor = elem.Value.Korean;
                        m_options[i].name_eng = elem.Value.English;
                        m_options[i].name_chn = elem.Value.Chinese;
                        m_options[i].thumbnail = elem.Value.Thumbnail_data;
                        m_options[i].images = elem.Value.Image_data;
                    }
                    else
                    {
                        ChromaKeyOptions option = new ChromaKeyOptions();
                        option.key = elem.Value.Key;

                        m_options.Add(option);
                    }
                    i++;
                }
                break;
            default:

                break;
        }
    }
}

[Serializable]
public class ChromaKeyOptions
{
    public string key;
    public string category;
    public string name_kor;
    public string name_eng;
    public string name_chn;

    public Sprite thumbnail;
    public List<Texture2D> images;
}
