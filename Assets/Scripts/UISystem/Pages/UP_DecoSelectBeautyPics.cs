using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vivestudios.UI;

public class UP_DecoSelectBeautyPics : UP_DecoSelectPicsBase
{
    [SerializeField]
    private List<UC_SelectablePic> _originalContents;

    public override void BindDelegates()
    {
        base.BindDelegates();

        for (int i = 0; i < _originalContents.Count; i++)
        {
            int index = i;
            _originalContents[i].pointerDownAction += () => SelectPic(_originalContents[index], index, PHOTO_TYPE.REAL);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        for (int i = 0; i < PhotoDataManager.inst.photoOrigin.Count && i < _originalContents.Count; i++)
        {
            _originalContents[i].SetThumbnail(PhotoDataManager.inst.photoOrigin[i]);
        }
    }

    protected override IEnumerator TimeLimitDoneRoutine()
    {
        _touchIgnoreArea.SetActive(true);

        for (int i = 0; i < _maxSelectNum; i++)
        {
            int index = i;
            if (_selectedPhotoIndexDic.Keys.Contains(index))
            {
                if (_selectedPhotoIndexDic[index] == -1)
                {
                    ForceAddPictureOriginAuto(index);
                    yield return new WaitForSecondsRealtime(1);
                }
                else if (PhotoDataManager.inst.selectedPicDic[index] == _originalContents[index])
                {
                    if (!_selectedPhotoIndexDic.Values.Contains(index))
                    {
                        ForceAddPictureOriginAuto(index);
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
                        ForceAddPictureOriginAuto(indexJ);
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
        Debug.LogFormat("[CLEAR UI] {0} pictures - override", name);
    }


    //protected void ForceAddPictureOrigin(int index)
    //{
    //    SelectPic(_originalContents[index], index);
    //}

    protected void ForceAddPictureOriginAuto(int index)
    {
        for (int i = 0; i < _originalContents.Count; i++)
        {
            int selectablePhotoIndex = i;
            if (!PhotoDataManager.inst.selectedPicDic.ContainsValue(_originalContents[i]))
            {
                //선택순서, 선택한 사진 인덱스
                SelectPic(_originalContents[selectablePhotoIndex], selectablePhotoIndex);
                break;
            }
        }
    }
}
