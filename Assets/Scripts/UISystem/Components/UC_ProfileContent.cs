using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UC_ProfileContent : UC_StyleContent
{
    [SerializeField]
    private TextMeshProUGUI _genderText;
    [SerializeField]
    private Image _genderImg;
    [SerializeField]
    private GameObject _genderObj;
    [SerializeField]
    private Image _thumbnailImgFull = null;

    private ServiceData.ContentsDetailEntry _contentDetail;

    public ServiceData.ContentsDetailEntry ContentDetail => _contentDetail;

    public void SetContentDetail (ServiceData.ContentsDetailEntry contentDetail)
    {
        _contentDetail = contentDetail;
    }

    private readonly Dictionary<GENDER_TYPE, string> GENDER_STRING_DIC = new Dictionary<GENDER_TYPE, string>
    {
        {GENDER_TYPE.MALE, "남성" },
        {GENDER_TYPE.FEMALE, "여성" }
    };

    private readonly Dictionary<GENDER_TYPE, Color> GENDER_COLOR_DIC = new Dictionary<GENDER_TYPE, Color>
    {
        { GENDER_TYPE.MALE, new Color(0.27f,0.49f,0.93f) },
        { GENDER_TYPE.FEMALE, new Color(0.81f,0.2f,0.35f) },
    };

    public void SetGender(GENDER_TYPE type)
    {
        _genderImg.sprite = ResourceCacheManager.inst.GetGenderStickerSprite(type);
        _genderText.text = GENDER_STRING_DIC[type];
        _genderText.color = GENDER_COLOR_DIC[type];
    }

    public void SetGenderActive(bool state)
    {
        _genderObj.SetActive(state);
    }

    public void SetThumbnail(Sprite thumbnail, bool isFull = false)
    {
        if (isFull)
        {
            _thumbnailImg.gameObject.SetActive(false);
            _thumbnailImgFull.gameObject.SetActive(true);
            _thumbnailImgFull.sprite = thumbnail;
        }
        else
        {
            _thumbnailImg.gameObject.SetActive(true);
            _thumbnailImgFull.gameObject.SetActive(false);
            base.SetThumbnail(thumbnail);
        }
    }
}
