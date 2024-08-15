using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_DecoSelectFrame : UP_DecoratePageBase
{
    //[SerializeField]
    //private UC_SelectableContent[] _frameColors;
    //[SerializeField]
    //private RectTransform _frameShapeArea;

    [SerializeField]
    private List<UC_SelectableContent> _frameColors;
    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _printBtn;

    [SerializeField]
    private GameObject stickerContainerParent;
    [SerializeField]
    private GameObject stickerContainer;

    [SerializeField]
    private GameObject _colorPrefab;
    [SerializeField]
    private Transform _colorContainer;

    public override void BindDelegates()
    {
        base.BindDelegates();

        _prevBtn.onClick.AddListener(OnClickPrev);
        _printBtn.onClick.AddListener(OnClickPrint);

        //foreach (var pair in _frameColorDic)
        //{
        //    pair.Value.pointerDownAction += () => OnClickColor(pair.Key);
        //}
        //foreach (var pair in _frameColorDicWhatIf)
        //{
        //    pair.Value.pointerDownAction += () => OnClickColor(pair.Key);
        //}

        //for (int i = 0; i < _frameShapes.Length; i++)
        //{
        //    int index = i;
        //    _frameShapes[i].pointerDownAction += () => OnClickShape(index);
        //}

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
        (_pageController as PC_Main).SetResultTextures(_frameAreaDic[UserDataManager.inst.selectedFrameType].GetResultPics());
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_PRINT);
    }

    /*
    //Tempt Test On Develop
    protected override void OnEnable()
    {
        if (!_pageController)
        {
            return;
        }

        base.OnEnable();


        //if(UserDataManager.inst.selectedContent == CONTENT_TYPE.WHAT_IF)
        //{
        //    foreach (var pair in _frameColorDic)
        //    {
        //        pair.Value.gameObject.SetActive(false);
        //    }
        //    foreach (var pair in _frameColorDicWhatIf)
        //    {
        //        pair.Value.gameObject.SetActive(true);
        //    }

        //    _frameColorDicWhatIf[UserDataManager.inst.selectedFrameColor].Select(true);
        //}
        //else
        //{
        //    foreach (var pair in _frameColorDic)
        //    {
        //        pair.Value.gameObject.SetActive(true);
        //    }
        //    foreach (var pair in _frameColorDicWhatIf)
        //    {
        //        pair.Value.gameObject.SetActive(false);
        //    }

        //    _frameColorDic[UserDataManager.inst.selectedFrameColor].Select(true);
        //}

        _frameColors[0].Select(true);

        if ((_pageController as PC_Main).timeLimitDone == true)
        {
            _prevBtn.interactable = false;
        }
        else
        {
            _prevBtn.interactable = true;
        }

        //FrameShapeBtnSelectCheck();
    }
    */

    protected override void OnTimeLimitDone()
    {
        if (gameObject.activeInHierarchy)
        {
            OnClickPrint();
        } 
    }

    //private void FrameShapeBtnSelectCheck()
    //{
    //    switch (UserDataManager.inst.selectedFrame)
    //    {
    //        case FRAME_TYPE.FRAME_2:
    //            OnClickShape(0);
    //            break;
    //        case FRAME_TYPE.FRAME_2_1:
    //            OnClickShape(1);
    //            break;
    //        case FRAME_TYPE.FRAME_2_2:
    //            OnClickShape(2);
    //            break;
    //    }
    //}

    private void OnClickColor(string key)
    {
        UserDataManager.inst.SetSelectedFrameColor(key);

        foreach (var color in _frameColors)
        {
            color.Select(key == color.Key);
        }

        (_pageController as PC_Main).UpdateFrame();
    }

    //private void OnClickShape(int index)
    //{
    //    for (int i = 0; i < _frameShapes.Length; i++)
    //    {
    //        _frameShapes[i].Select(index == i);
    //    }

    //    switch (index)
    //    {
    //        case 1:
    //            UserDataManager.inst.SelectFrame(FRAME_TYPE.FRAME_2_1);
    //            break;
    //        case 2:
    //            UserDataManager.inst.SelectFrame(FRAME_TYPE.FRAME_2_2);
    //            break;
    //        default:
    //            UserDataManager.inst.SelectFrame(FRAME_TYPE.FRAME_2);
    //            break;
    //    }

    //    FrameEnable();
    //    UpdateFrame();
    //}

    private void CreateContent()
    {
        // Create Frame Color
        if(AdminManager.Instance.FrameData.Theme.Sorting.ToLower() == StringCacheManager.inst.SortingSpecified)
        {
            for (int i = 1; i <= AdminManager.Instance.FrameData.Theme.OrderedColorCode.Count; i++)
            {
                FrameData.ColorCodeEntry colorCode = AdminManager.Instance.FrameData.Theme.OrderedColorCode[i];
                GameObject colorObj = Instantiate(_colorPrefab, _colorContainer);
                UC_SelectableContent color = colorObj.GetComponent<UC_SelectableContent>();
                color.SetThumbnail(colorCode.Thumbnail_data);
                color.SetNameText(colorCode.korean);
                color.SetKey(colorCode.key);
                color.pointerDownAction += () => OnClickColor(colorCode.key);

                _frameColors.Add(color);
            }
        }
        else
        {
            foreach (var item in AdminManager.Instance.FrameData.Theme.ColorCode)
            {
                if (item.Value.Use)
                {
                    GameObject colorObj = Instantiate(_colorPrefab, _colorContainer);
                    UC_SelectableContent color = colorObj.GetComponent<UC_SelectableContent>();
                    color.SetThumbnail(item.Value.Thumbnail_data);
                    color.SetNameText(item.Value.korean);
                    color.SetKey(item.Value.key);
                    color.pointerDownAction += () => OnClickColor(item.Value.key);

                    _frameColors.Add(color);
                }
            }
        }
        _isContentCreated = true;
    }

    public override void OnPageEnable()
    {
        if (!_isContentCreated)
        {
            CreateContent();
        }
        if (!_pageController)
        {
            return;
        }

        _frameColors[0].Select(true);

        if ((_pageController as PC_Main).timeLimitDone == true)
        {
            _prevBtn.interactable = false;
        }
        else
        {
            _prevBtn.interactable = true;
        }
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
