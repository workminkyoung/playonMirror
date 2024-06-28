using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_SelectablePhoto : Toggle
{
    [SerializeField]
    RawImage _rawImage;
    Image _imgCover;
    TextMeshProUGUI _text;
    Texture2D _texture;
    PHOTO_TYPE _type;
    int _index;
    public int Index {  get { return _index; } }

    public PHOTO_TYPE type { set { _type = value; } get { return _type; } }

    //Vector2 _landSize = new Vector2 (320, 240);
    //Vector2 _potSize = new Vector2 (190, 240);

    public Texture2D texture
    {
        get { return _texture; }
        set 
        { 
            _texture = value;
            //if (_texture.width > _texture.height)
            //    _rawImage.rectTransform.sizeDelta = _landSize;
            //else
            //    _rawImage.rectTransform.sizeDelta = _potSize;
            _rawImage.texture = _texture;
        }
    }

    public void Setting()
    {
        _rawImage = GetComponentInChildren<RawImage>();
        _imgCover = UtilityExtensions.GetComponentOnlyInChildrenWithTag_Recursive<Image>(transform, "cover");
        _text = GetComponentInChildren<TextMeshProUGUI>();

        _imgCover.gameObject.SetActive(false);
        //onValueChanged.AddListener(OnValueChanged);
    }

    public void SetIndex(int index)
    {
        _index = index;
        _text.text = index.ToString();
    }

    public void OnValueChanged()
    {
        _imgCover.gameObject.SetActive(isOn);
    }
}
