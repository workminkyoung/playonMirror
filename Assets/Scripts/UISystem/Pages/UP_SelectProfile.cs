using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vivestudios.UI;

public class UP_SelectProfile : UP_BaseSelectContent, IPageTimeLimit
{
    [SerializeField]
    private string _popupTitle;
    [SerializeField]
    private string _popupDescription;

    [SerializeField]
    private Button _prevBtn;

    [SerializeField]
    private PROFILE_TYPE[] _profileContents;

    private bool _requirePopup = true;
    public int MaxTime { get => _maxTime; set => _maxTime = value; }
    public TextMeshProUGUI timeText { get => _timeText; set => _timeText = value; }

    public override void InitPage()
    {
        base.InitPage();

        _maxTime = ConfigData.config.profileSelectTime;

        for (int i = 0; i < _contents.Length; i++)
        {
            if (i < _profileContents.Length)
            {
                (_contents[i] as UC_ProfileContent).SetThumbnail(ResourceCacheManager.inst.GetProfileThumbnailSprite(_profileContents[i]));
                (_contents[i] as UC_ProfileContent).SetTitle(StringCacheManager.inst.GetProfileTitle(_profileContents[i]));
                (_contents[i] as UC_ProfileContent).SetDescription(StringCacheManager.inst.GetProfileDescription(_profileContents[i]));
                (_contents[i] as UC_ProfileContent).SetGender(ResourceCacheManager.inst.GetProfileGenderType(_profileContents[i]));
            }
            else
            {
                _contents[i].transform.parent.gameObject.SetActive(false);
            }

        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_pageController && _requirePopup)
        {
            //(_pageController as PC_Main).globalPage.OpenConfirmPopup(_popupTitle, _popupDescription, ResourceCacheManager.inst.profilePopupThumbnailSprite);
        }
    }

    protected override void OnDisable()
    {
        (_pageController as PC_Main).globalPage.CloseConfirmPopup();
        base.OnDisable();
    }

    public override void BindDelegates()
    {
        for(int i = 0; i < _contents.Length; i++)
        {
            int index = i;
            _contents[i].pointerClickAction += () => OnClickContent(index);
        } 
        
        _prevBtn?.onClick.AddListener(() =>
        {
            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_CONTENT);
            _requirePopup = false;
        });

        GameManager.OnGameResetAction += () =>
        {
            _requirePopup = true;
        };
    }

    protected override void OnClickContent(int index)
    {
        _requirePopup = false;
        UserDataManager.inst.SelectProfile(_profileContents[index]);
        (_pageController as PC_Main)?.globalPage?.OpenAIProfileAlert(() =>
        {
            _pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
        });

        //UserDataManager.inst.SelectSubContent(index);
        //_pageController.ChangePage(PAGE_TYPE.PAGE_SELECT_FRAME);
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
