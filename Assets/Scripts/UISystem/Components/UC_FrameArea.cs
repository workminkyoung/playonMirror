using FrameData;
using Klak.Ndi.Interop;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_FrameArea : UC_BaseComponent
{
    [SerializeField]
    private bool _isPrintSize = false;
    [SerializeField]
    private bool _isVideoSize = false;
    [SerializeField]
    private FRAME_TYPE _frameType;
    [SerializeField]
    private string _frameColor;
    [SerializeField]
    private bool _isFilterOn;

    [SerializeField]
    private Image[] _masks;
    [SerializeField]
    private Button[] _frameBtns;
    [SerializeField]
    private RawImage[] _VerticalPics;
    [SerializeField]
    private RawImage[] _HorizontalPics;
    [SerializeField]
    private TextMeshProUGUI[] _dateTexts;
    [SerializeField]
    private Image[] _logoImgs;
    [SerializeField]
    private Image _splitLine;

    //##############################크리스마스 이벤트####################################
    [SerializeField]
    private Image _upperLayers;
    [SerializeField]
    private ColorTypeTransformDicBase _dateTextTransformDic;
    [SerializeField]
    private ColorTypeTransformDicBase _qrTransformDic;

    [Serializable]
    private class ColorTypeImageDicBase : SerializableDictionaryBase<FRAME_COLOR_TYPE, Sprite> { }
    [Serializable]
    private class ColorTypeTransformDicBase : SerializableDictionaryBase<FRAME_COLOR_TYPE, RectTransform> { }
    //##############################크리스마스 이벤트####################################

    [SerializeField]
    private FRAME_RATIO_TYPE _ratioType;

    [SerializeField]
    private List<Texture2D> _photos = new List<Texture2D>();
    private List<RenderTexture> _renderTextures = new List<RenderTexture>();
    [SerializeField]
    private List<PHOTO_TYPE> _types = new List<PHOTO_TYPE>();
    //[SerializeField]
    //private List<Texture2D> _skinFilterPhotos = new List<Texture2D>();

    private Texture2D _lutTex;

    public Action<int> OnClickFrameAction;

    public override void InitComponent()
    {
        for (int i = 0; i < _frameBtns.Length; i++)
        {
            int index = i;
            _frameBtns[i].onClick.AddListener(() => { OnClickFrameAction?.Invoke(index); });
        }
    }

    private void Start()
    {
    }

    public int GetPicCount ()
    {
        switch(_frameType)
        {
            case FRAME_TYPE.FRAME_1:
                return 1;
            case FRAME_TYPE.FRAME_2:
                return 2;
            case FRAME_TYPE.FRAME_4:
                return 4;
            case FRAME_TYPE.FRAME_8:
                return 4;
            case FRAME_TYPE.FRAME_2_1:
                return 2;
            case FRAME_TYPE.FRAME_2_2:
                return 2;
            default:
                return 1;
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        UpdateFrame();
    //    }
    //}

    public List<Texture2D> GetResultPics()
    {
        List<Texture2D> result = new List<Texture2D>();

        if (_ratioType == FRAME_RATIO_TYPE.HORIZONTAL)
        {
            for (int i = 0; i < _HorizontalPics.Length; i++)
            {
                result.Add(ApplyMaterialToTexture(_HorizontalPics[i].texture as Texture2D, _HorizontalPics[i].material));
            }
        }
        else
        {
            for (int i = 0; i < _VerticalPics.Length; i++)
            {
                result.Add(ApplyMaterialToTexture(_VerticalPics[i].texture as Texture2D, _VerticalPics[i].material));
            }
        }

        return result;
    }

    private Texture2D ApplyMaterialToTexture(Texture2D tex, Material mat, int channel = -1)
    {
        //Create new texture and render texture:
        Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false, true);
        RenderTexture rendTex = new RenderTexture(tex.width, tex.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        rendTex.DiscardContents();

        //Set the materials channel and blit the material to the source texture:
        if (channel >= 0)
        {
            mat.SetInt("_Channel", channel);
        }
        GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
        //GL.Clear(true, true, Color.black);
        Graphics.Blit(tex, rendTex, mat, channel);
        GL.sRGBWrite = false;

        //Read out the render texure's pixels back to the source texture and release the render texture from memory:
        RenderTexture.active = rendTex;
        newTex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0, false);
        newTex.Apply();
        RenderTexture.active = null;
        rendTex.Release();
        return newTex;
    }

    public void UpdateFrame(bool _isProfileVid = false)
    {
        UpdateSkinFilter();
        UpdatePhotos();
        UpdateDateText();
        UpdateFrameColor(_isProfileVid);
        UpdateLutEffect();
    }

    public void SetPics(List<Texture2D> pics, List<PHOTO_TYPE> types = null)
    {
        if(_photos == null)
        {
            _photos = new List<Texture2D>();
        }

        if (types == null || types.Count == 0)
        {
            types = new List<PHOTO_TYPE>();
            for (int i = 0; i < _photos.Count; i++)
            {
                types.Add(PHOTO_TYPE.NONE);
            }
        }
        _photos = pics;
        _types = types;
    }

    public void SetRenderTexture(List<RenderTexture> renderTextures)
    {
        _renderTextures = renderTextures;
    }

    public void SetFrameColor(string type)
    {
        _frameColor = type;
    }

    public void SetLutEffect(string lutKey)
    {
        if(AdminManager.Instance.FilterData != null && AdminManager.Instance.FilterData.FilterTable.ContainsKey(lutKey))
        {
            _lutTex = AdminManager.Instance.FilterData.FilterTable[lutKey].LutFile_data;
        }
    }

    //public void SetLutEffect(LUT_EFFECT_TYPE type)
    //{
    //    _lutTex = ResourceCacheManager.inst.GetLutTexture(type);
    //}

    public void SetFilterOn(bool isOn)
    {
        _isFilterOn = isOn;
    }

    public void SetRatioType(FRAME_RATIO_TYPE ratioType)
    {
        _ratioType = ratioType;
    }

    public void SetSkinFilterOn(bool isOn)
    {
        _isFilterOn = isOn;
    }

    public void UpdatePhotos()
    {
        if (_photos.Count == 0 || _photos == null)
        {
            foreach (var elem in _VerticalPics)
            {
                elem.texture = null;
                elem.gameObject.SetActive(false);
            }

            foreach (var elem in _HorizontalPics)
            {
                elem.texture = null;
                elem.gameObject.SetActive(false);
            }
        }

        if (_frameType == FRAME_TYPE.FRAME_8)
        {
            for (int i = 0; i < _VerticalPics.Length / 2; i++)
            {
                if (_photos.Count > i)
                {
                    if (_ratioType == FRAME_RATIO_TYPE.VERTICAL)
                    {
                        _VerticalPics[i].texture = _photos[i]; // _skinFilterPhotos[i];
                        _VerticalPics[_VerticalPics.Length / 2 + i].texture = _photos[i]; //_skinFilterPhotos[i];
                        _VerticalPics[i].gameObject.SetActive(_photos[i] != null);
                        _VerticalPics[_VerticalPics.Length / 2 + i].gameObject.SetActive(_photos[i] != null);
                    }
                    else
                    {
                        _HorizontalPics[i].texture = _photos[i]; //_skinFilterPhotos[i];
                        _HorizontalPics[_VerticalPics.Length / 2 + i].texture = _photos[i]; //_skinFilterPhotos[i];
                        _HorizontalPics[i].gameObject.SetActive(_photos[i] != null);
                        _HorizontalPics[_VerticalPics.Length / 2 + i].gameObject.SetActive(_photos[i] != null);
                    }
                }
                else if (_renderTextures.Count > i)
                {
                    if (_ratioType == FRAME_RATIO_TYPE.VERTICAL)
                    {
                        _VerticalPics[i].texture = _renderTextures[i];
                        _VerticalPics[_VerticalPics.Length / 2 + i].texture = _renderTextures[i];
                        _VerticalPics[i].gameObject.SetActive(_renderTextures[i] != null);
                        _VerticalPics[_VerticalPics.Length / 2 + i].gameObject.SetActive(_renderTextures[i] != null);
                    }
                    else
                    {
                        _HorizontalPics[i].texture = _renderTextures[i];
                        _HorizontalPics[_VerticalPics.Length / 2 + i].texture = _renderTextures[i];
                        _HorizontalPics[i].gameObject.SetActive(_renderTextures[i] != null);
                        _HorizontalPics[_VerticalPics.Length / 2 + i].gameObject.SetActive(_renderTextures[i] != null);
                    }
                }
                else
                {
                    if (_ratioType == FRAME_RATIO_TYPE.VERTICAL)
                    {
                        _VerticalPics[i].gameObject.SetActive(false);
                        _VerticalPics[_VerticalPics.Length / 2 + i].gameObject.SetActive(false);
                    }
                    else
                    {
                        _HorizontalPics[i].gameObject.SetActive(false);
                        _HorizontalPics[_VerticalPics.Length / 2 + i].gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _VerticalPics.Length; i++)
            {
                if (_photos.Count > i)
                {
                    if (_ratioType == FRAME_RATIO_TYPE.VERTICAL)
                    {
                        _VerticalPics[i].texture = _photos[i]; //_skinFilterPhotos[i];
                        _VerticalPics[i].gameObject.SetActive(_photos[i] != null);
                    }
                    else
                    {
                        _HorizontalPics[i].texture = _photos[i]; //_skinFilterPhotos[i];
                        _HorizontalPics[i].gameObject.SetActive(_photos[i] != null);
                    }


                    if (_frameType == FRAME_TYPE.FRAME_2_2)
                    {
                        //_frameImgs[i].color = _photos[i] == null ? Color.white : Color.clear;
                        //_masks[i].color = _photos[i] == null ? Color.white : Color.clear;
                    }
                }
                else if (_renderTextures.Count > i)
                {
                    if (_ratioType == FRAME_RATIO_TYPE.VERTICAL)
                    {
                        _VerticalPics[i].texture = _renderTextures[i];
                        _VerticalPics[i].gameObject.SetActive(_renderTextures[i] != null);
                    }
                    else
                    {
                        _HorizontalPics[i].texture = _renderTextures[i];
                        _HorizontalPics[i].gameObject.SetActive(_renderTextures[i] != null);
                    }
                }
                else
                {
                    if (_ratioType == FRAME_RATIO_TYPE.VERTICAL)
                    {
                        _VerticalPics[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        _HorizontalPics[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void UpdateDateText()
    {
        foreach (var elem in _dateTexts)
        {
            elem.text = DateTime.Now.ToString("yyyy.MM.dd");
        }
    }

    private DefinitionEntry GetFrameDefinition()
    {
        if(AdminManager.Instance.FrameData == null)
        {
            return null;
        }


        FrameData.DefinitionEntry definition = null;
        try
        {
            string frameType = UserDataManager.Instance.selectedFrameKey;
            List<FrameData.DefinitionEntry> entries = AdminManager.Instance.FrameData.Definition.Code[frameType];

            foreach (var item in entries)
            {
                if (item.ColorCode == _frameColor)
                {
                    definition = item;
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            CustomLogger.LogException(ex);
        }

        return definition;
    }

    private void UpdateFrameColor(bool isProfileVid = false)
    {
        // adminManager에서 파싱할때 default 컬러 정하고 업데이트
        FrameData.DefinitionEntry entry = GetFrameDefinition();
        if(entry == null)
        {
            return;
        }

        if (isProfileVid)
        {
            Tuple<string, string> tupleKey = new Tuple<string, string>(UserDataManager.Instance.selectedContentKey, UserDataManager.Instance.selectedFrameColor);
            FrameData.DefinitionEntry entryCover = AdminManager.Instance.FrameData.DefinitionTuple["FR1X1001"][tupleKey];
            _upperLayers.sprite = entryCover.LayerImage_data;
        }
        else
        {
            _upperLayers.sprite = entry.LayerImage_data;
        }

        _upperLayers.color = Color.white;// ResourceCacheManager.inst.GetFrameColor(_frameColor);
        if(_splitLine != null)
        {
            _splitLine.color = Color.clear;
        }

        int printSize = 1;
        if (_isPrintSize)
        {
            printSize = 4;
        }else if (_isVideoSize)
        {
            printSize = 2;
        }

        foreach (Image logo in _logoImgs)
        {
            logo.color = Color.clear;
        }

        for (int i = 0; i < _dateTexts.Length; i++)
        {
            _dateTexts[i].color = UtilityExtensions.HexToColor(entry.DateColor);
            _dateTexts[i].font = ResourceCacheManager.inst.GetFrameFont(_frameColor);
            _dateTexts[i].alignment = TextAlignmentOptions.Midline;
            _dateTexts[i].rectTransform.pivot = entry.dateRects[i].pivot;
            _dateTexts[i].rectTransform.anchorMin = entry.dateRects[i].anchorMin;
            _dateTexts[i].rectTransform.anchorMax = entry.dateRects[i].anchorMax;
            _dateTexts[i].rectTransform.sizeDelta = entry.dateRects[i].sizeDelta * printSize;
            _dateTexts[i].rectTransform.anchoredPosition = entry.dateRects[i].anchoredPosition * printSize;
            _dateTexts[i].characterSpacing = 6;
        }
    }

    private void UpdateLutEffect()
    {
        foreach (var elem in _VerticalPics)
        {
            if (_lutTex != null)
            {
                if (elem.material.shader != Shader.Find("Nexweron/Builtin/Lut/Unlit_Lut2D"))
                {
                    elem.material = new Material(Shader.Find("Nexweron/Builtin/Lut/Unlit_Lut2D"));
                }
                elem.material.mainTexture = elem.texture;
                elem.material.SetTexture("_LutTex", _lutTex);
            }
            elem.gameObject.SetActive(false);
            if (elem.texture != null && _ratioType == FRAME_RATIO_TYPE.VERTICAL)
            {
                elem.gameObject.SetActive(true);
            }
        }

        foreach (var elem in _HorizontalPics)
        {
            if (_lutTex != null)
            {
                if (elem.material.shader != Shader.Find("Nexweron/Builtin/Lut/Unlit_Lut2D"))
                {
                    elem.material = new Material(Shader.Find("Nexweron/Builtin/Lut/Unlit_Lut2D"));
                }
                elem.material.mainTexture = elem.texture;
                elem.material.SetTexture("_LutTex", _lutTex);
            }
            elem.gameObject.SetActive(false);
            if (elem.texture != null && _ratioType == FRAME_RATIO_TYPE.HORIZONTAL)
            {
                elem.gameObject.SetActive(true);
            }

        }
    }

    private void UpdateSkinFilter()
    {
        if(_photos == null)
        {
            _photos = new List<Texture2D>();
        }

        if (_types == null || _types.Count == 0)
        {
            _types = new List<PHOTO_TYPE>();
            
            for (int i = 0; i < _photos.Count; i++)
            {
                _types.Add(PHOTO_TYPE.NONE);
            }
        }

        for (int i = 0; i < _photos.Count; i++)
        {
            if (_types[i] == PHOTO_TYPE.REAL && _isFilterOn)
            {

                switch (_ratioType)
                {
                    case FRAME_RATIO_TYPE.VERTICAL:
                        _VerticalPics[i].material.SetFloat("_Enable", 1);
                        if (_frameType == FRAME_TYPE.FRAME_8)
                        {
                            _VerticalPics[4 + i].material.SetFloat("_Enable", 1);
                        }
                        break;
                    case FRAME_RATIO_TYPE.HORIZONTAL:
                        _HorizontalPics[i].material.SetFloat("_Enable", 1);
                        if (_frameType == FRAME_TYPE.FRAME_8)
                        {
                            _HorizontalPics[4 + i].material.SetFloat("_Enable", 1);
                        }
                        break;
                }
            }
            else
            {
                switch (_ratioType)
                {
                    case FRAME_RATIO_TYPE.VERTICAL:
                        _VerticalPics[i].material.SetFloat("_Enable", 0);
                        if (_frameType == FRAME_TYPE.FRAME_8)
                        {
                            _VerticalPics[4 + i].material.SetFloat("_Enable", 0);
                        }
                        break;
                    case FRAME_RATIO_TYPE.HORIZONTAL:
                        _HorizontalPics[i].material.SetFloat("_Enable", 0);
                        if (_frameType == FRAME_TYPE.FRAME_8)
                        {
                            _HorizontalPics[4 + i].material.SetFloat("_Enable", 0);
                        }
                        break;
                }
            }
        }
    }
}
