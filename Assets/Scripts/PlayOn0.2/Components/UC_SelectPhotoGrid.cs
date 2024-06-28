using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UC_SelectPhotoGrid : UC_SelectPhoto
{
    List<UC_HighlightText> _highlightTexts = new List<UC_HighlightText>();
    List<LayoutGroup> _layoutGroups = new List<LayoutGroup>();
    List<Button> _btns = new List<Button>();
    bool _isReal = true;

    public override void InitComponent()
    {
        _highlightTexts.AddRange(GetComponentsInChildren<UC_HighlightText>());
        _texts.AddRange(GetComponentsInChildren<TextMeshProUGUI>());
        _layoutGroups.AddRange(GetComponentsInChildren<LayoutGroup>());
        _btns.AddRange(GetComponentsInChildren<Button>());

        for (int i = 0; i < _highlightTexts.Count; i++)
        {
            _highlightTexts[i].Setting();
        }

        _btns[(int)BODY.Cartoon].onClick.AddListener(SetToCartoon);
        _btns[(int)BODY.Real].onClick.AddListener(SetToReal);
    }

    public override void SetActivate(bool state)
    {
        base.SetActivate(state);
        if(state)
        {
            SetToCartoon();
            InitDictSelect();
            // Cartoon photo 생성
            _body = _layoutGroups[(int)BODY.Cartoon];
            for (int i = 0; i < _photoCount; i++)
                CreatePhoto(i, _photoConverted[i]);

            // Real photo 생성
            _body = _layoutGroups[(int)BODY.Real];
            for (int i = 0; i < _photoCount; i++)
                CreatePhoto(i, _photoOrigin[i], PHOTO_TYPE.REAL);
        }
        else
        {

        }
    }

    //void SetPhotoType()
    //{
    //    _isReal = _isReal ? false : true;

    //    if(_isReal )
    //    {
    //        _layoutGroups[(int)BODY.Cartoon].gameObject.SetActive(false);
    //        _layoutGroups[(int)BODY.Real].gameObject.SetActive(true);

    //        _highlightTexts[(int)BODY.Cartoon].SetActive(false);
    //        _highlightTexts[(int)BODY.Real].SetActive(true);
    //    }
    //    else
    //    {
    //        _layoutGroups[(int)BODY.Cartoon].gameObject.SetActive(true);
    //        _layoutGroups[(int)BODY.Real].gameObject.SetActive(false);

    //        _highlightTexts[(int)BODY.Cartoon].SetActive(true);
    //        _highlightTexts[(int)BODY.Real].SetActive(false);
    //    }
    //}

    void SetToCartoon()
    {
        _layoutGroups[(int)BODY.Cartoon].gameObject.SetActive(true);
        _layoutGroups[(int)BODY.Real].gameObject.SetActive(false);

        _highlightTexts[(int)BODY.Cartoon].SetActive(true);
        _highlightTexts[(int)BODY.Real].SetActive(false);
    }

    void SetToReal()
    {
        _layoutGroups[(int)BODY.Cartoon].gameObject.SetActive(false);
        _layoutGroups[(int)BODY.Real].gameObject.SetActive(true);

        _highlightTexts[(int)BODY.Cartoon].SetActive(false);
        _highlightTexts[(int)BODY.Real].SetActive(true);
    }


    enum TEXT
    {
        Cartoon,
        Real,
        Count
    }

    enum BODY
    {
        Cartoon,
        Real
    }
}
