using JetBrains.Annotations;
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

public class UP_DecoSelectCartoonPics : UP_DecoSelectPicsBase
{
    [SerializeField]
    private RectTransform _cartoonArea;
    [SerializeField]
    private RectTransform _profileArea;

    [SerializeField]
    private TMP_FontAsset _regularFont;
    [SerializeField]
    private TMP_FontAsset _boldFont;

    [SerializeField]
    private Button _cartoonBtn, _originalBtn;
    [SerializeField]
    private TextMeshProUGUI _cartoonText, _originalText;
    [SerializeField]
    private Image _cartoonUnderline, _originalUnderline;
    [SerializeField]
    private RectTransform _cartoonContentsArea, _originalContentsArea;

    [SerializeField]
    private List<UC_SelectablePic> _originalContents;

    public override void BindDelegates()
    {
        base.BindDelegates();

        _cartoonBtn.onClick.AddListener(OpenCartoon);
        _originalBtn.onClick.AddListener(OpenOriginal);


        for (int i = 0; i < _originalContents.Count; i++)
        {
            int index = i;
            _originalContents[i].pointerDownAction += () => SelectPic(_originalContents[index], index, PHOTO_TYPE.REAL);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        OpenCartoon();

        for (int i = 0; i < PhotoDataManager.inst.photoOrigin.Count && i < _originalContents.Count; i++)
        {
            _originalContents[i].SetThumbnail(PhotoDataManager.inst.photoOrigin[i]);
        }
    }

    protected override IEnumerator TimeLimitDoneRoutine()
    {
        OpenCartoon();
        _touchIgnoreArea.SetActive(true);

        for (int i = 0; i < _maxSelectNum; i++)
        {
            int index = i;
            if (_selectedPhotoIndexDic.Keys.Contains(index))
            {
                if (_selectedPhotoIndexDic[index] == -1)
                {
                    ForceAddPictureAuto(index);
                    yield return new WaitForSecondsRealtime(1);
                }
                else if (PhotoDataManager.inst.selectedPicDic[index] == _originalContents[index])
                {
                    if(!_selectedPhotoIndexDic.Values.Contains(index))
                    {
                        ForceAddPictureAuto(index);
                        yield return new WaitForSecondsRealtime(1);
                    }
                }
            }
            else
            {
                for (int j = 0; j < _maxSelectNum; j++)
                {
                    int indexJ = j;
                    if (!_selectedPhotoIndexDic.Values.Contains(indexJ))
                    {
                        ForceAddPictureAuto(indexJ);
                        yield return new WaitForSecondsRealtime(1);
                        break;
                    }
                }
                yield return new WaitForSecondsRealtime(1);
            }
            if (_maxSelectNum <= _curSelectNum)
            {
                break;
            }
        }

        (_pageController as PC_Main).SetResultTextures(_frameAreaDic[UserDataManager.inst.selectedFrameType].GetResultPics());
        yield return new WaitForEndOfFrame();

        TimeLimitDoneCoroutine = null;

        (_pageController as PC_Main).StopTimeLimit();
        (_pageController as PC_Main).StartTimeLimit(10);
        _touchIgnoreArea.SetActive(false);

        OnClickNext();
        //(_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_EFFECT);
        
    }

    private void OpenCartoon()
    {
        _cartoonText.font = _boldFont;
        _originalText.font = _regularFont;

        _cartoonUnderline.gameObject.SetActive(true);
        _originalUnderline.gameObject.SetActive(false);

        _cartoonContentsArea.gameObject.SetActive(true);
        _originalContentsArea.gameObject.SetActive(false);
    }

    private void OpenOriginal()
    {
        _cartoonText.font = _regularFont;
        _originalText.font = _boldFont;

        _cartoonUnderline.gameObject.SetActive(false);
        _originalUnderline.gameObject.SetActive(true);

        _cartoonContentsArea.gameObject.SetActive(false);
        _originalContentsArea.gameObject.SetActive(true);
    }

    public override void OnPageEnable()
    {
        base.OnPageEnable();
    }

    public override void OnPageDisable()
    {
        base.OnPageDisable();
    }

    protected override void OnPageReset()
    {
        base.OnPageReset();

        foreach (var elem in _originalContents)
        {
            elem.Select(false);
        }

        for (int i = 0; i < _originalContents.Count; i++)
        {
            _originalContents[i].SetThumbnail(null);
        }
        CustomLogger.Log($"[CLEAR UI] {name} pictures - override");

    }
}
