using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_DecoSelectPicsBase : UP_DecoratePageBase
{
    [SerializeField]
    protected Button _nextBtn;
    [SerializeField]
    protected TextMeshProUGUI _countText;
    [SerializeField]
    protected List<UC_SelectablePic> _contents;

    [SerializeField]
    protected int _maxSelectNum;
    [SerializeField]
    protected int _curSelectNum;

    [SerializeField]
    protected GameObject _touchIgnoreArea;

   // protected Dictionary<int, int> _selectedPhotoIndexDic = new Dictionary<int, int>();
    [SerializeField]
    protected SelectPhotoIndexDic _selectedPhotoIndexDic;

    protected Coroutine TimeLimitDoneCoroutine = null;

    [Serializable]
    protected class SelectPhotoIndexDic : SerializableDictionaryBase<int, int> { }

    public override void InitPage()
    {
        base.InitPage();
    }

    public override void BindDelegates()
    {
        base.BindDelegates();

        for (int i = 0; i < _contents.Count; i++)
        {
            int index = i;
            _contents[i].pointerDownAction += () => SelectPic(_contents[index], index);
        }

        _nextBtn.onClick.AddListener(OnClickNext);

        foreach (var elem in _frameAreaDic.Keys)
        {
            if (elem == FRAME_TYPE.FRAME_8)
            {
                _frameAreaDic[elem].OnClickFrameAction += (index) => DeselectPic(index >= 4 ? index - 4 : index);
            }
            else
            {
                _frameAreaDic[elem].OnClickFrameAction += (index) => DeselectPic(index);
            }
        }
    }

    protected void OnClickNext()
    {
        switch(UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_STICKER);
                break;
            default:
                (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_EFFECT);
                break;
        }
    }

    protected void SelectPic(UC_SelectablePic content, int photoIndex = 0, PHOTO_TYPE type = PHOTO_TYPE.CONVERTED)
    {//content 2, photo 0
        if (PhotoDataManager.inst.selectedPicDic.ContainsValue(content))
        {
            int deleteContentIndex = -1;
            foreach (var elem in PhotoDataManager.inst.selectedPicDic.Keys)
            {
                if (PhotoDataManager.inst.selectedPicDic[elem] == content)
                {
                    PhotoDataManager.inst.selectedPicDic[elem].Select(false);
                    deleteContentIndex = elem;
                }
            }

            if (deleteContentIndex != -1)
            {
                PhotoDataManager.inst.RemoveSelectedPhoto(PhotoDataManager.inst.selectedPicDic[deleteContentIndex].thumbnailTex);
                PhotoDataManager.inst.selectedPicDic[deleteContentIndex] = null;
                if (_selectedPhotoIndexDic.Keys.Contains(deleteContentIndex))
                {
                    _selectedPhotoIndexDic[deleteContentIndex] = -1;
                }
            }
        }
        else
        {
            bool contentSelected = false;
            for (int i = 0; i < PhotoDataManager.inst.selectedPicDic.Count; i++)
            {
                if (PhotoDataManager.inst.selectedPicDic[i] == null)
                {
                    int index = i + 1;
                    PhotoDataManager.inst.selectedPicDic[i] = content;
                    content.SetNum(index);
                    content.Select(true);
                    PhotoDataManager.inst.AddSelectedPhoto(content.thumbnailTex, type);
                    _selectedPhotoIndexDic[index - 1] = photoIndex;

                    contentSelected = true;
                    break;
                }
            }

            if (!contentSelected)
            {
                if (PhotoDataManager.inst.selectedPicDic.Count < _maxSelectNum)
                {
                    PhotoDataManager.inst.selectedPicDic.Add(PhotoDataManager.inst.selectedPicDic.Count, content);
                    content.SetNum(PhotoDataManager.inst.selectedPicDic.Count);
                    PhotoDataManager.inst.AddSelectedPhoto(content.thumbnailTex, type);
                    if (_selectedPhotoIndexDic.Keys.Contains(PhotoDataManager.inst.selectedPicDic.Count - 1))
                    {
                        _selectedPhotoIndexDic[PhotoDataManager.inst.selectedPicDic.Count - 1] = photoIndex;
                    }
                    else
                    {
                        _selectedPhotoIndexDic.Add(PhotoDataManager.inst.selectedPicDic.Count - 1, photoIndex);
                    }

                    content.Select(true);
                }
                else
                {
                    content.Select(false);
                }
            }
        }

        SetSelectNumText();
        NextBtnUpdate();

        (_pageController as PC_Main).SetResultIndexs(_selectedPhotoIndexDic.Values.ToList());
        (_pageController as PC_Main).UpdateFrame();
    }
    private void DeselectPic(int index)
    {
        if (PhotoDataManager.inst.selectedPicDic.ContainsKey(index))
        {
            if (PhotoDataManager.inst.selectedPicDic[index] != null)
            {
                PhotoDataManager.inst.selectedPicDic[index].Select(false);
                PhotoDataManager.inst.RemoveSelectedPhoto(PhotoDataManager.inst.selectedPicDic[index].thumbnailTex);
                PhotoDataManager.inst.selectedPicDic[index] = null;

                if (_selectedPhotoIndexDic.Keys.Contains(index))
                {
                    _selectedPhotoIndexDic[index] = -1;
                }

                SetSelectNumText();
                NextBtnUpdate();

                (_pageController as PC_Main).SetResultIndexs(_selectedPhotoIndexDic.Values.ToList());
                (_pageController as PC_Main).UpdateFrame();
            }
        }
    }

    private void SetSelectNumText()
    {
        _curSelectNum = 0;

        foreach (var elem in PhotoDataManager.inst.selectedPicDic.Values)
        {
            if (elem != null)
            {
                _curSelectNum++;
            }
        }

        _countText.text = $"{_curSelectNum}/{_maxSelectNum}";
    }

    private void NextBtnUpdate()
    {
        _nextBtn.interactable = _maxSelectNum <= _curSelectNum;
    }

    protected override void OnTimeLimitDone()
    {
        if (transform.gameObject.activeInHierarchy)
        {
            TimeLimitDoneCoroutine = StartCoroutine(TimeLimitDoneRoutine());
        }
    }

    protected virtual IEnumerator TimeLimitDoneRoutine()
    {
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

        (_pageController as PC_Main).SetResultTextures(_frameAreaDic[UserDataManager.inst.selectedFrame].GetResultPics());
        yield return new WaitForEndOfFrame();

        TimeLimitDoneCoroutine = null;

        (_pageController as PC_Main).StopTimeLimit();
        (_pageController as PC_Main).StartTimeLimit(10);
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_EFFECT);
        _touchIgnoreArea.SetActive(false);
    }

    //protected void ForceAddPicture(int index)
    //{
    //    SelectPic(_contents[index], index);
    //}

    protected void ForceAddPictureAuto(int index)
    {
        for (int i = 0; i < _contents.Count; i++)
        {
            int selectablePhotoIndex = i;
            if (!PhotoDataManager.inst.selectedPicDic.ContainsValue(_contents[i]))
            {
                //선택순서, 선택한 사진 인덱스
                SelectPic(_contents[selectablePhotoIndex], selectablePhotoIndex);
                break;
            }
        }
    }

    public override void OnPageEnable()
    {
        
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnEnable()
    {
        if(!_pageController)
        {
            return;
        }

        base.OnEnable();

        _touchIgnoreArea.SetActive(false);

        switch (UserDataManager.inst.selectedFrame)
        {
            case FRAME_TYPE.FRAME_1:
                _maxSelectNum = 1;
                break;
            case FRAME_TYPE.FRAME_2:
                _maxSelectNum = 2;
                break;
            case FRAME_TYPE.FRAME_4:
                _maxSelectNum = 4;
                break;
            case FRAME_TYPE.FRAME_8:
                _maxSelectNum = 4;
                break;
            default:
                _maxSelectNum = 2;
                break;
        }

        for (int i = 0; i < PhotoDataManager.inst.photoConverted.Count && i < _contents.Count; i++)
        {
            _contents[i].SetThumbnail(PhotoDataManager.inst.photoConverted[i]);
        }

        SetSelectNumText();
        UpdateFrame();
    }

    protected void OnDisable()
    {
        if (TimeLimitDoneCoroutine != null)
        {
            StopCoroutine(TimeLimitDoneCoroutine);
            TimeLimitDoneCoroutine = null;
        }
    }

    protected override void OnPageReset()
    {
        _frameAreaDic[UserDataManager.inst.selectedFrame].SetPics(new List<Texture2D>());
        //_selectedPhotoIndexDic = new Dictionary<int, int>();
        _selectedPhotoIndexDic = new SelectPhotoIndexDic();

        foreach (var elem in _contents)
        {
            elem.Select(false);
        }

        for (int i = 0; i < _contents.Count; i++)
        {
            _contents[i].SetThumbnail(null);
        }
        CustomLogger.Log($"[CLEAR UI] {name} pictures - base");

        UpdateFrame();
        _nextBtn.interactable = false;
    }
}
