using BubbleData;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_StickerThumbnail : UC_BaseComponent, IPointerClickHandler
{
    [SerializeField]
    private BubbleTableEntry _stickerOption = null;
    [SerializeField]
    private RectTransform _scaler;
    [SerializeField]
    private MaskableGraphic _thumbnail = null;
    [SerializeField]
    private TextMeshProUGUI _text = null;

    public RectTransform rectTransform => transform as RectTransform;
    public float scale => _stickerOption.StartScale;
    public int margin => _stickerOption.Margin;
    public string category => _stickerOption.Category;
    public RectTransform scaler => _scaler;
    public MaskableGraphic thumbnail => _thumbnail;
    public Action<BubbleTableEntry> OnClickAction;

    public override void InitComponent ()
    {

    }

    public void SetOption (BubbleTableEntry option)
    {
        _stickerOption = option;
        SetThumbnail();
        SetText();
        _scaler.localScale = new Vector3(_stickerOption.ThumbnailScale, _stickerOption.ThumbnailScale, 1);
    }

    private void SetThumbnail ()
    {
        RectTransform thumbnailObj = new GameObject().AddComponent<RectTransform>();
        thumbnailObj.SetParent(_scaler);
        thumbnailObj.name = "Thumbnail";
        thumbnailObj.sizeDelta = new Vector2(100, 100);
        thumbnailObj.localPosition = Vector3.zero;
        thumbnailObj.localScale = Vector3.one;
        thumbnailObj.localRotation = Quaternion.identity;
        thumbnailObj.anchoredPosition3D = Vector3.zero;

        switch(_stickerOption.ImageType.ToLower())
        {
            case "svg":
                _thumbnail = thumbnailObj.gameObject.AddComponent<SVGImage>();
                (_thumbnail as SVGImage).sprite = _stickerOption.Image_sprite;
                break;
            case "png":
                _thumbnail = thumbnailObj.gameObject.AddComponent<Image>();
                (_thumbnail as Image).sprite = _stickerOption.Image_sprite;
                break;
        }

        if(string.IsNullOrEmpty(_stickerOption.Image))
        {
            return;
        }

        _thumbnail.rectTransform.sizeDelta = new Vector2(_stickerOption.Image_sprite.rect.width, _stickerOption.Image_sprite.rect.height);
    }

    private void SetText ()
    {
        if(_stickerOption.Kind == "ImageText" || _stickerOption.Kind == "Text")
        {
            RectTransform textObj = new GameObject().AddComponent<RectTransform>();
            textObj.SetParent(_scaler);
            textObj.name = "Text";
            textObj.sizeDelta = new Vector2(100, 100);
            textObj.localPosition = Vector3.zero;
            textObj.localScale = Vector3.one;
            textObj.localRotation = Quaternion.identity;
            textObj.anchoredPosition3D = Vector3.zero;

            TextMeshProUGUI text = textObj.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = ResourceCacheManager.inst.GetFont(_stickerOption.FontSet, AdminManager.Instance.Language);
            text.fontSize = _stickerOption.FontSize;
            text.color = UtilityExtensions.HexToColor(_stickerOption.FontColor);
            text.alignment = TextAlignmentOptions.Center;
            switch(AdminManager.Instance.Language)
            {
                case LANGUAGE_TYPE.KOR:
                    text.text = System.Text.RegularExpressions.Regex.Unescape(_stickerOption.Korean);
                    break;
                case LANGUAGE_TYPE.ENG:
                    text.text = System.Text.RegularExpressions.Regex.Unescape(_stickerOption.English);
                    break;
                case LANGUAGE_TYPE.CHN:
                    text.text = System.Text.RegularExpressions.Regex.Unescape(_stickerOption.Chinese);
                    break;
                default:
                    text.text = System.Text.RegularExpressions.Regex.Unescape(_stickerOption.Korean);
                    break;
            }

            text.enableWordWrapping = false;

            _text = text;

            if(string.IsNullOrEmpty(_stickerOption.Image))
            {
                ContentSizeFitter fitter = _text.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                LayoutRebuilder.ForceRebuildLayoutImmediate(_text.rectTransform);

                _thumbnail.color = Color.clear;
                _thumbnail.rectTransform.sizeDelta = _text.rectTransform.sizeDelta;
            }
        }
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        OnClickAction?.Invoke(_stickerOption);
    }
}
