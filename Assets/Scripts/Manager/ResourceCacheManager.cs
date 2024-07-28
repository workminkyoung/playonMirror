using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;
using UnityEngine.Video;
using Vivestudios.UI;
using static ZXing.Rendering.SvgRenderer;

public class ResourceCacheManager : SingletonBehaviour<ResourceCacheManager>
{
    [SerializeField]
    private LutTexDicBase _lutTexDic;

    [SerializeField]
    private Sprite _cartoonPopupThumbnailSprite;
    [SerializeField]
    private CartoonTexDicBase _cartoonThumbnailDic;

    [SerializeField]
    private Sprite _profilePopupThumbnailSprite;
    [SerializeField]
    private ProfileSpriteDicBase _profileThumbnailSprites;
    [SerializeField]
    private ProfileGenderDicBase _profileGenderDic;
    [SerializeField]
    private GenderSpriteDicBase _genderStickerSprites;
    [SerializeField]
    private ContentTypeVideoDicBase _videoDict;

    //frame data
    [SerializeField]
    private FrameUpperDicBase _whiteFrame;
    [SerializeField]
    private FrameUpperDicBase _greenFrame;
    [SerializeField]
    private FrameUpperDicBase _jtncWhFrame;
    [SerializeField]
    private FrameUpperDicBase _jtncBlFrame;
    [SerializeField]
    private FrameUpperDicBase _jtncSiFrame;
    [SerializeField]
    private FrameColorCodeDicBase _frameColorCode;
    [SerializeField]
    private ColorTypeFontDicBase _fontDic;
    [SerializeField]
    private TMP_FontAsset _defaultFont;

    [Header("Sticker Option")]
    [SerializeField]
    private FontNameDicBase _fontNameDic;
    [SerializeField]
    private StickerOptionBase[] _stickerOptions;

    public Sprite cartoonPopupThumbnailSprite => _cartoonPopupThumbnailSprite;
    public Sprite profilePopupThumbnailSprite => _profilePopupThumbnailSprite;

    public StickerOptionBase[] stickerOptions => _stickerOptions;

    protected override void Init ()
    {
        StartCoroutine(LoadSVGSprites());

        for(int i = 0; i < _stickerOptions.Length; i++)
        {
            int index = i;

            if(string.IsNullOrEmpty(_stickerOptions[index].image))
            {
                continue;
            }

            if(_stickerOptions[index].imageType.ToLower() == "png")
            {
                StartCoroutine(LoadPNGSprite("Sticker", _stickerOptions[index].image, (sprite) => _stickerOptions[index].thumbnailSprite = sprite));
            }
        }
    }

    public Sprite GetFrameSprite (FRAME_COLOR_TYPE colorType, FRAME_TYPE frameType)
    {
        if(colorType == FRAME_COLOR_TYPE.FRAME_GREEN)
        {
            return _greenFrame[frameType];
        }
        else if(colorType == FRAME_COLOR_TYPE.FRAME_JTBC_WH)
        {
            return _jtncWhFrame[frameType];
        }
        else if(colorType == FRAME_COLOR_TYPE.FRAME_JTBC_BL)
        {
            return _jtncBlFrame[frameType];
        }
        else if(colorType == FRAME_COLOR_TYPE.FRAME_JTBC_SI)
        {
            return _jtncSiFrame[frameType];
        }
        else
        {
            return _whiteFrame[frameType];
        }
    }

    public Color GetFrameColor (FRAME_COLOR_TYPE type)
    {
        if(_frameColorCode == null || _frameColorCode.ContainsKey(type) == false)
        {
            //error color
            return Color.white;
        }
        else
        {
            Color color = ColorExtension.HexToColor(_frameColorCode[type]);
            return color;
        }
    }

    public TMP_FontAsset GetFrameFont (FRAME_COLOR_TYPE type)
    {
        if(_fontDic == null || _fontDic.ContainsKey(type) == false)
        {
            return _defaultFont;
        }
        else
        {
            return _fontDic[type];
        }
    }

    public Texture2D GetLutTexture (LUT_EFFECT_TYPE type)
    {
        return _lutTexDic[type];
    }

    public Sprite GetCartoonThumbnailSprite (CARTOON_TYPE type)
    {
        return _cartoonThumbnailDic[type];
    }

    public Sprite GetProfileThumbnailSprite (PROFILE_TYPE type)
    {
        return _profileThumbnailSprites[type];
    }

    public GENDER_TYPE GetProfileGenderType (PROFILE_TYPE type)
    {
        return _profileGenderDic[type];
    }

    public Sprite GetGenderStickerSprite (GENDER_TYPE type)
    {
        return _genderStickerSprites[type];
    }

    public VideoClip GetContentVideoThumbnail (CONTENT_TYPE type)
    {
        return _videoDict[type];
    }

    public TMP_FontAsset GetFont (string fontName, LANGUAGE_TYPE lang)
    {
        if(_fontNameDic.ContainsKey(fontName) == false)
        {
            return _defaultFont;
        }
        else
        {
            return _fontNameDic[fontName][lang];
        }
    }

    #region Temp : 스티커 불러오는 코루틴 (추후 관리자 기능 추가 시 삭제)
    IEnumerator LoadPNGSprite (string folderPath, string fileName, Action<Sprite> onDone)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, folderPath, fileName);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(filePath);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            onDone.Invoke(sprite);
        }
    }

    IEnumerator LoadSVGSprite (string folderPath, string fileName, Action<Sprite> onDone)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, folderPath, fileName);
        Debug.Log(filePath);
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(filePath + www.error);
        }
        else
        {
            string svgText = www.downloadHandler.text;

            var TessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = 100.0f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            };

            var SceneInfo = SVGParser.ImportSVG(new StringReader(svgText));
            var TessGeo = VectorUtils.TessellateScene(SceneInfo.Scene, TessOptions);
            var spriteSvg = VectorUtils.BuildSprite(TessGeo, 10.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
            onDone.Invoke(spriteSvg);
        }
    }

    IEnumerator LoadSVGSprites ()
    {
        List<StickerOptionBase> options = _stickerOptions.Where(option => option.imageType.ToLower() == "svg").ToList();

        for(int i = 0; i < options.Count; i++)
        {
            yield return new WaitForEndOfFrame();
            int index = i;
            if(string.IsNullOrEmpty(options[index].image))
            {
                continue;
            }
            yield return StartCoroutine(LoadSVGSprite("Sticker", options[index].image, (sprite) => options[index].thumbnailSprite = sprite));
        }
    }

    #endregion

    [Serializable]
    private class CartoonTexDicBase : SerializableDictionaryBase<CARTOON_TYPE, Sprite> { }
    [Serializable]
    private class LutTexDicBase : SerializableDictionaryBase<LUT_EFFECT_TYPE, Texture2D> { }
    [Serializable]
    private class ProfileSpriteDicBase : SerializableDictionaryBase<PROFILE_TYPE, Sprite> { }
    [Serializable]
    private class ProfileGenderDicBase : SerializableDictionaryBase<PROFILE_TYPE, GENDER_TYPE> { }
    [Serializable]
    private class GenderSpriteDicBase : SerializableDictionaryBase<GENDER_TYPE, Sprite> { }
    [Serializable]
    private class FrameUpperDicBase : SerializableDictionaryBase<FRAME_TYPE, Sprite> { }
    [Serializable]
    private class FrameColorCodeDicBase : SerializableDictionaryBase<FRAME_COLOR_TYPE, string> { }
    [Serializable]
    private class ColorTypeFontDicBase : SerializableDictionaryBase<FRAME_COLOR_TYPE, TMP_FontAsset> { }
    [Serializable]
    private class ContentTypeVideoDicBase : SerializableDictionaryBase<CONTENT_TYPE, VideoClip> { }
    [Serializable]
    private class FontNameDicBase : SerializableDictionaryBase<string, LangFontDicBase> { }

    [Serializable]
    private class LangFontDicBase : SerializableDictionaryBase<LANGUAGE_TYPE, TMP_FontAsset> { }
}

[Serializable]
public class StickerOptionBase
{
    public string key;
    public int group;
    public int sequence;
    public string category;
    public float thumbnailScale;
    public float startScale;
    public int margin;
    public string textArea;
    public string imageType;
    public string image;
    public int fontSize;
    public string kind;
    public string fontSet;
    public string fontColor;
    public string kor;
    public string eng;
    public string chn;
    public Texture thumbnailTex;
    public Sprite thumbnailSprite;
}
