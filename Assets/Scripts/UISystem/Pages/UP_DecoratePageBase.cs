using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using Vivestudios.UI;

public abstract class UP_DecoratePageBase : UP_BasePage
{
    [SerializeField]
    protected FrameAreaDicBase _frameAreaDic;

    [SerializeField]
    protected TextMeshProUGUI _timeText;

    public override void InitPage()
    {
    }

    public override void BindDelegates()
    {
        (_pageController as PC_Main).OnFrameUpdateAction += UpdateFrame;
        (_pageController as PC_Main).OnTimeUpdateAction += OnTimeUpdate;
        (_pageController as PC_Main).OnTimeLimitDoneAction += OnTimeLimitDone;
    }

    protected void OnTimeUpdate(int time)
    {
        //_timeText.text = string.Format("{0}분 {1:D2}초", time / 60, time % 60);

        if (time / 60 <= 0)
        {
            _timeText.text = string.Format("{0}초", time);
        }
        else
        {
            _timeText.text = string.Format("{0}분{1}초", time / 60, time % 60);
        }
    }

    protected virtual void OnTimeLimitDone()
    {
        //if (this.gameObject.activeInHierarchy)
        //{
        //    (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_PRINT);
        //}
    }

    protected virtual void OnEnable()
    {
        if (!_pageController)
        {
            return;
        }

        (_pageController as PC_Main).StartTimeLimit();

        FrameEnable();

        UpdateFrame();
    }

    protected void FrameEnable()
    {
        foreach (var elem in _frameAreaDic.Keys)
        {
            _frameAreaDic[elem].gameObject.SetActive(UserDataManager.inst.selectedFrame == elem);
        }
    }
    protected void UpdateFrame()
    {
        List<Texture2D> texs = new List<Texture2D>();

        for (int i = 0; i < PhotoDataManager.inst.selectedPicDic.Count; i++)
        {
            if (PhotoDataManager.inst.selectedPicDic[i] == null)
            {
                texs.Add(null);
            }
            else
            {
                texs.Add(PhotoDataManager.inst.selectedPicDic[i].thumbnailTex);
            }
        }

        List<PHOTO_TYPE> types = new List<PHOTO_TYPE>();

        int index = 0;
        for (int i = 0; i < texs.Count; i++)
        {
            if (texs[i] == null)
            {
                types.Add(PHOTO_TYPE.NONE);
                continue;
            }
            types.Add(PhotoDataManager.inst.selectedPhoto[PhotoDataManager.inst.selectedPhoto.Keys.ToList()[index]]);
            index++;
        }

        _frameAreaDic[UserDataManager.inst.selectedFrame].SetSkinFilterOn((_pageController as PC_Main).isSkinFilterOn);
        _frameAreaDic[UserDataManager.inst.selectedFrame].SetRatioType(UserDataManager.inst.frameRatioType);
        _frameAreaDic[UserDataManager.inst.selectedFrame].SetFrameColor(UserDataManager.inst.selectedFrameColor);
        _frameAreaDic[UserDataManager.inst.selectedFrame].SetPics(texs, types);
        _frameAreaDic[UserDataManager.inst.selectedFrame].SetLutEffect(UserDataManager.inst.selectedLut);
        _frameAreaDic[UserDataManager.inst.selectedFrame].UpdateFrame();
    }

    [Serializable]
    protected class FrameAreaDicBase : SerializableDictionaryBase<FRAME_TYPE, UC_FrameArea> { };
}
