using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_DecoSelectEffects : UP_DecoratePageBase
{
    [SerializeField]
    private UC_SelectableContent[] _contents;
    [SerializeField]
    private RectTransform _skinTransform;

    [SerializeField]
    private Toggle _skinToggle;
    [SerializeField]
    private TextMeshProUGUI _skinDescription;

    [SerializeField]
    private Button _prevBtn;
    [SerializeField]
    private Button _nextBtn;

    protected const int DISABLE_STROKE_SIZE = 2;

    public override void InitPage()
    {
        base.InitPage();

        _contents = GetComponentsInChildren<UC_SelectableContent>(true);
    }

    public override void BindDelegates()
    {
        base.BindDelegates();

        _prevBtn.onClick.AddListener(OnClickPrev);
        _nextBtn.onClick.AddListener(OnClickNext);

        for (int i = 0; i < _contents.Length; i++)
        {
            int index = i;
            _contents[i].pointerDownAction += () => OnClickFilter(index);
        }

        _skinToggle.onValueChanged.AddListener(OnChangeSkin);
    }

    private void OnClickPrev()
    {
        switch (UserDataManager.inst.selectedContent)
        {

            case CONTENT_TYPE.AI_CARTOON:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_STICKER);
                break;
            case CONTENT_TYPE.AI_PROFILE:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_PROFILE);
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_BEAUTY);
                break;
            case CONTENT_TYPE.WHAT_IF:
                _pageController.ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_PICS_WHAT_IF);
                break;
        }

    }

    private void OnClickNext()
    {
        (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_FRAME);
    }

    protected override void OnEnable()
    {
        if(!_pageController)
        {
            return;
        }

        base.OnEnable();

        _contents[(int)UserDataManager.inst.selectedLut].Select(true);

        bool originalContains = false;
        switch (UserDataManager.inst.selectedContent)
        {
            case CONTENT_TYPE.AI_CARTOON:
                foreach (var elem in PhotoDataManager.inst.selectedPhoto.Values)
                {
                    if (elem == PHOTO_TYPE.REAL)
                    {
                        originalContains = true;
                    }
                }
                if (originalContains)
                {
                    _skinTransform.gameObject.SetActive(true);
                    _skinToggle.isOn = (_pageController as PC_Main).isSkinFilterOn;
                    _skinDescription.text = StringCacheManager.inst.GetFilterDescription(CONTENT_TYPE.AI_CARTOON);
                }
                else
                {
                    _skinTransform.gameObject.SetActive(false);
                }
                break;
            case CONTENT_TYPE.AI_PROFILE:
                (_pageController as PC_Main).SkinFilterOn(false);
                _skinToggle.isOn = (_pageController as PC_Main).isSkinFilterOn;
                _skinTransform.gameObject.SetActive(false);
                break;
            case CONTENT_TYPE.AI_TIME_MACHINE:
                break;
            case CONTENT_TYPE.AI_BEAUTY:
                foreach (var elem in PhotoDataManager.inst.selectedPhoto.Values)
                {
                    if (elem == PHOTO_TYPE.REAL)
                    {
                        originalContains = true;
                    }
                }
                if (originalContains)
                {
                    _skinTransform.gameObject.SetActive(true);
                    _skinToggle.isOn = (_pageController as PC_Main).isSkinFilterOn;
                    _skinDescription.text = StringCacheManager.inst.GetFilterDescription(CONTENT_TYPE.AI_BEAUTY);
                }
                else
                {
                    _skinTransform.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }

        if ((_pageController as PC_Main).timeLimitDone == true)
        {
            _prevBtn.interactable = false;
        }
        else
        {
            _prevBtn.interactable = true;
        }
    }

    private void OnClickFilter(int index)
    {
        UserDataManager.inst.SetLutEffect(index);

        for (int i = 0; i < _contents.Length; i++)
        {
            _contents[i].Select(index == i);
        }

        (_pageController as PC_Main).UpdateFrame();
    }

    private void OnChangeSkin(bool isOn)
    {
        (_pageController as PC_Main).SkinFilterOn(isOn);
        (_pageController as PC_Main).UpdateFrame();
    }

    protected override void OnTimeLimitDone()
    {
        if (gameObject.activeInHierarchy)
        {
            (_pageController as PC_Main).StopTimeLimit();
            (_pageController as PC_Main).StartTimeLimit(10);
            (_pageController as PC_Main).ChangePage(PAGE_TYPE.PAGE_DECO_SELECT_FRAME);
        }
    }

    public override void OnPageEnable()
    {
    }

    public override void OnPageDisable()
    {
    }

    protected override void OnPageReset()
    {
    }
}
