using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_DecoSelectFrame : UP_DecoratePageBase
{
    //[SerializeField]
    //private UC_SelectableContent[] _frameColors;
    [SerializeField]
    private UC_SelectableContent[] _frameShapes;

    [SerializeField]
    private RectTransform _frameShapeArea;

    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _printBtn;

    [SerializeField]
    private SelectableColorTypeDicBase _frameColorDic;
    [SerializeField]
    private SelectableColorTypeDicBase _frameColorDicWhatIf;

    [SerializeField]
    private GameObject stickerContainerParent;
    [SerializeField]
    private GameObject stickerContainer;

    [Serializable]
    private class SelectableColorTypeDicBase : SerializableDictionaryBase<FRAME_COLOR_TYPE, UC_SelectableContent> { }

    public override void BindDelegates()
    {
        base.BindDelegates();

        _prevBtn.onClick.AddListener(OnClickPrev);
        _printBtn.onClick.AddListener(OnClickPrint);

        //for (int i = 0; i < _frameColors.Length; i++)
        //{
        //    int index = i;
        //    _frameColors[i].pointerDownAction += () => OnClickColor(index);
        //}

        foreach (var pair in _frameColorDic)
        {
            pair.Value.pointerDownAction += () => OnClickColor(pair.Key);
        }
        foreach (var pair in _frameColorDicWhatIf)
        {
            pair.Value.pointerDownAction += () => OnClickColor(pair.Key);
        }

        for (int i = 0; i < _frameShapes.Length; i++)
        {
            int index = i;
            _frameShapes[i].pointerDownAction += () => OnClickShape(index);
        }

        (pageController as PC_Main).StickerUpdateAction += UpdateSticker;
    }

    private void UpdateSticker ()
    {
        if(stickerContainer != null)
        {
            Destroy(stickerContainer);
        }
        stickerContainer = GameObject.Instantiate((pageController as PC_Main).stickerContainerPrefab, stickerContainerParent.transform);
        foreach(var elem in stickerContainer.GetComponentsInChildren<UC_StickerController>())
        {
            elem.HideController();
            Destroy(elem);
        }
    }

    private void OnClickPrev()
    {
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_EFFECT);
    }

    private void OnClickPrint()
    {
        (_pageController as PC_Main).StopTimeLimit();
        (_pageController as PC_Main).SetResultTextures(_frameAreaDic[UserDataManager.inst.selectedFrame].GetResultPics());
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_PRINT);
    }

    protected override void OnEnable()
    {
        if (!_pageController)
        {
            return;
        }

        base.OnEnable();


        _frameShapeArea.gameObject.SetActive(false);

        /*
         * 2컷 세부 선택 삭제
        if (UserDataManager.inst.selectedFrame == FRAME_TYPE.FRAME_2 ||
            UserDataManager.inst.selectedFrame == FRAME_TYPE.FRAME_2_1 ||
            UserDataManager.inst.selectedFrame == FRAME_TYPE.FRAME_2_2)
        {
            _frameShapeArea.gameObject.SetActive(true);
        }
        else
        {
            _frameShapeArea.gameObject.SetActive(false);
        }
        */

        if(UserDataManager.inst.selectedContent == CONTENT_TYPE.WHAT_IF)
        {
            foreach (var pair in _frameColorDic)
            {
                pair.Value.gameObject.SetActive(false);
            }
            foreach (var pair in _frameColorDicWhatIf)
            {
                pair.Value.gameObject.SetActive(true);
            }

            _frameColorDicWhatIf[UserDataManager.inst.selectedFrameColor].Select(true);
        }
        else
        {
            foreach (var pair in _frameColorDic)
            {
                pair.Value.gameObject.SetActive(true);
            }
            foreach (var pair in _frameColorDicWhatIf)
            {
                pair.Value.gameObject.SetActive(false);
            }

            _frameColorDic[UserDataManager.inst.selectedFrameColor].Select(true);
        }


        if ((_pageController as PC_Main).timeLimitDone == true)
        {
            _prevBtn.interactable = false;
        }
        else
        {
            _prevBtn.interactable = true;
        }

        FrameShapeBtnSelectCheck();
    }

    protected override void OnTimeLimitDone()
    {
        if (gameObject.activeInHierarchy)
        {
            OnClickPrint();
        } 
    }

    private void FrameShapeBtnSelectCheck()
    {
        switch (UserDataManager.inst.selectedFrame)
        {
            case FRAME_TYPE.FRAME_2:
                OnClickShape(0);
                break;
            case FRAME_TYPE.FRAME_2_1:
                OnClickShape(1);
                break;
            case FRAME_TYPE.FRAME_2_2:
                OnClickShape(2);
                break;
        }
    }

    private void OnClickColor(FRAME_COLOR_TYPE type)
    {
        UserDataManager.inst.SetSelectedFrameColor(type);

        foreach (var pair in _frameColorDic)
        {
            pair.Value.Select(type == pair.Key);
        }
        foreach (var pair in _frameColorDicWhatIf)
        {
            pair.Value.Select(type == pair.Key);
        }

        (_pageController as PC_Main).UpdateFrame();
    }

    private void OnClickShape(int index)
    {
        for (int i = 0; i < _frameShapes.Length; i++)
        {
            _frameShapes[i].Select(index == i);
        }

        switch (index)
        {
            case 1:
                UserDataManager.inst.SelectFrame(FRAME_TYPE.FRAME_2_1);
                break;
            case 2:
                UserDataManager.inst.SelectFrame(FRAME_TYPE.FRAME_2_2);
                break;
            default:
                UserDataManager.inst.SelectFrame(FRAME_TYPE.FRAME_2);
                break;
        }

        FrameEnable();
        UpdateFrame();
    }

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
        if(stickerContainer != null)
        {
            Destroy(stickerContainer);
        }
    }
}
