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

    public Sprite cartoonPopupThumbnailSprite => _cartoonPopupThumbnailSprite;
    public Sprite profilePopupThumbnailSprite => _profilePopupThumbnailSprite;

    protected override void Init ()
    {

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
            Debug.Log($"Cannot find [{fontName}] : set default font");
            return _defaultFont;
        }
        else
        {
            return _fontNameDic[fontName][lang];
        }
    }

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
