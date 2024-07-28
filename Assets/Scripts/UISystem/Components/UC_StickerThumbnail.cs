using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_StickerThumbnail : UC_BaseComponent, IPointerClickHandler
{
    [SerializeField]
    private StickerOptionBase _stickerOption = null;
    [SerializeField]
    private RectTransform _scaler;
    [SerializeField]
    private MaskableGraphic _thumbnail = null;
    [SerializeField]
    private TextMeshProUGUI _text = null;

    public RectTransform rectTransform => transform as RectTransform;
    public string category => _stickerOption.category;

    public override void InitComponent ()
    {

    }

    public void SetOption (StickerOptionBase option)
    {
        _stickerOption = option;
        SetThumbnail();
        SetText();
        _scaler.localScale = new Vector3(_stickerOption.thumbnailScale, _stickerOption.thumbnailScale, 1);
    }

    public void SetThumbnail ()
    {
        if(string.IsNullOrEmpty(_stickerOption.image))
        {
            return;
        }

        RectTransform thumbnailObj = new GameObject().AddComponent<RectTransform>();
        thumbnailObj.SetParent(_scaler);
        thumbnailObj.name = "Thumbnail";
        thumbnailObj.sizeDelta = new Vector2(100, 100);
        thumbnailObj.localPosition = Vector3.zero;
        thumbnailObj.localScale = Vector3.one;
        thumbnailObj.localRotation = Quaternion.identity;
        thumbnailObj.anchoredPosition3D = Vector3.zero;

        switch(_stickerOption.imageType.ToLower())
        {
            case "svg":
                _thumbnail = thumbnailObj.gameObject.AddComponent<SVGImage>();
                (_thumbnail as SVGImage).sprite = _stickerOption.thumbnailSprite;
                break;
            case "png":
                _thumbnail = thumbnailObj.gameObject.AddComponent<Image>();
                (_thumbnail as Image).sprite = _stickerOption.thumbnailSprite;
                break;
        }

        _thumbnail.rectTransform.sizeDelta = new Vector2(_stickerOption.thumbnailSprite.rect.width, _stickerOption.thumbnailSprite.rect.height);
    }

    public void SetText ()
    {
        if(_stickerOption.kind == "ImageText" || _stickerOption.kind == "Text")
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
            text.font = ResourceCacheManager.inst.GetFont(_stickerOption.fontSet, LANGUAGE_TYPE.KOR);
            text.fontSize = _stickerOption.fontSize;
            text.color = UtilityExtensions.HexToColor(_stickerOption.fontColor);
            text.alignment = TextAlignmentOptions.Center;
            text.text = System.Text.RegularExpressions.Regex.Unescape(_stickerOption.kor);
            text.enableWordWrapping = false;
            _text = text;
        }
    }

    public void OnPointerClick (PointerEventData eventData)
    {

    }
}
